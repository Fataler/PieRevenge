using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private LayerMask groundLayer;
    private TrailRenderer _tr;

    private float runHorizontal;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private float coyoteTime = 0.2f; //�����, ������� ������ �� ��, ��� �� ������ ������ ����� � ���������
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f; //����� ������, ������� ��������� ��������, ����� �������� ��� �� �����������
    private float jumpBufferCounter;

    [SerializeField] private float jumpCurrent;
    [SerializeField] private float stockJumps = 1f;
    //private bool isJumping;
    //[SerializeField] private float jumpCooldown = 0.4f;

    private bool isFacingRight;

    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashingForce = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;

    private bool IsGrounded() //��� �������� ��������� �� �������� �� �����, ������ � ����� ������� ���������
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _tr = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DashCoroutine());
        }

        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing) // ����� �������� ��������� � ����, �������� ���� �� ����� ��� ����������
        {
            return;
        }

        runHorizontal = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector2(runHorizontal * speed, _rb.velocity.y);
    }

    public void Jump()
    {
        if (IsGrounded()) 
        {
            coyoteTimeCounter = coyoteTime;
            jumpCurrent = stockJumps;
            //isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f || Input.GetButtonDown("Jump") && jumpCurrent > 0 && !IsGrounded())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            jumpCurrent--;
            //StartCoroutine(JumpCooldown());
        }

        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    public void Flip() //����� �������� ������������� �� ����������� ��������
    {
        if (isFacingRight && runHorizontal < 0f || !isFacingRight && runHorizontal > 0f)
        {
            Vector3 _localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
        }
    }

    /*private IEnumerator JumpCooldown() //���� ����� ������ ���� ������, �� ������ ������� ������, ��� �� ����
    {
        isJumping = true;
        yield return new WaitForSeconds(jumpCooldown);
        isJumping = false;
    }*/

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f; // ��������� ����������, ����� �������� �� �������� ���������
        _rb.velocity = new Vector2(-_rb.transform.localScale.x * dashingForce, 0f); //����� ����������� ��������� �� �
        _tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        _tr.emitting = false;
        _rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
