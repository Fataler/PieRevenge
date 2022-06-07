using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Rigidbody2D _rb;
    private CapsuleCollider2D _collider;
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private LayerMask groundLayer;

    private float runHorizontal;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private float coyoteTime = 0.2f; //�����, ������� ������ �� ��, ��� �� ������ ������ ����� � ���������
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f; //����� ������, ������� ��������� ��������, ����� �������� ��� �� �����������
    private float jumpBufferCounter;

    private float jumpCurrent;
    [SerializeField] private float jumpMax;

    private bool isJumping;
    private bool isFacingRight;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Run()
    {
        runHorizontal = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector2(runHorizontal * speed, _rb.velocity.y);
    }

    private bool IsGrounded() //��� �������� ��������� �� �������� �� �����, ������ � ����� ������� ���������
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void Jump()
    {
        if (IsGrounded()) 
        {
            coyoteTimeCounter = coyoteTime;
            jumpCurrent = jumpMax;
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

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            StartCoroutine(JumpCooldown());
        }
    }

    public void HighJump()
    {
        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    public void DoubleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCurrent > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            jumpCurrent -= 1f;
        }
    }

    public void Flip() //����� �������� ������������� �� ����������� ��������
    {
        if (isFacingRight && runHorizontal < 0f || !isFacingRight && runHorizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 _localScale = transform.localScale;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
        }
    }

    private IEnumerator JumpCooldown() //���� ����� ������ ���� ������, �� ������ ������� ������, ��� �� ����
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }
}
