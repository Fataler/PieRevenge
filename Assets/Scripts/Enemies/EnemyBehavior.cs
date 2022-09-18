using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour, IDamageToEnemy
{
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    protected bool _isGrounded;

    [SerializeField] protected Transform _point1;
    [SerializeField] protected Transform _point2;
    protected Transform _myPoint;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _rangeToChasing;
    [SerializeField] protected float _rangeToSrandingNearPlayer;

    [SerializeField] public float HealthEnemy;
    [SerializeField] private float _framesDuration;
    [SerializeField] private int _flashesCount;
    private bool _isDead = false;

    private bool _isFacingRight;
    protected Vector2 _moveDistance;

    protected Rigidbody2D _rb;
    protected Transform _target;
    protected Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRend;

    protected bool _canAttack = true;
    protected bool _isAttacking = false;
    [SerializeField] private float _deathTimer;

    private EnemySound _enemySound;

    private void Awake()
    {
        // ��������� �����, ���� ������ �������������.
        _myPoint = _point1;
    }

    protected void Start()
    {
        // ��������� ��������� ������
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _enemySound = GetComponent<EnemySound>();
    }

    private void Update()
    {
        
        // �������� �� �����
        _isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, 0.2f, _groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                _isGrounded = true;
            }
        }

        // ������� ������� �� ����������� ��������
        if (_isFacingRight && _moveDistance.x > 0f || !_isFacingRight && _moveDistance.x < 0f)
        {
            Vector3 _localScale = transform.localScale;
            _isFacingRight = !_isFacingRight;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
        }

        _animator.SetFloat("Speed", Mathf.Abs(_moveDistance.x));

    }

    private void FixedUpdate()
    {
        if (!_isDead)
        {
            if (!_isAttacking)
            {
                Move();
            }

            if (_canAttack)
            {
                Attack();
            }
        }
    }

    public virtual void Move()
    {
        // ���� ��������� �� ������ ������ ��������� ��������,
        if (Vector2.Distance(transform.position, _target.position) > _rangeToChasing)
        {
            // �� ���� ����� ������������� ����� 2 �������:
            // ������� ��������� �� ��������� �����. ���� ��� ������ ������-�� ��������,
            // �� ������ ���� �� ������ ����� � ���� � ���. � ��� ����������.
            if (Vector2.Distance(transform.position, _point1.position) < 0.5f)
            {
                _myPoint = _point2;
            }
            if (Vector2.Distance(transform.position, _point2.position) < 0.5f)
            {
                _myPoint = _point1;
            }
            // ��������� �� ����� �������� � �������� �������� � 1.
            // ��� ����� ���� ������������� � �������������.
            // ����� ������� �������� ����������� �������� �� ��� �.
            Vector3 _range = (_myPoint.position - transform.position).normalized;
            _moveDistance = _range;
            // � ���������� �� ��������.
            _rb.velocity = new Vector2(_moveDistance.x, 0f) * _speed;
        }
    }

    // ����� ����������� ��������� �� ����� ����� � �������� ������� �����
    public abstract void Attack();

    // ���������� ���������� IDamageToEnemy. ��������� ��� ��������� ����� �� ������
    public void EnemyGetDamaged(CharacterControl player, float damage)
    {
        Debug.Log(HealthEnemy);
        if (HealthEnemy > 0)
        {
            HealthEnemy -= damage;
            StartCoroutine(HurtCoroutine());
            if (HealthEnemy == 0)
            {
                StartCoroutine(Death());
            }
        }
    }

    private IEnumerator HurtCoroutine()
    {
        for (int i = 0; i < _flashesCount; i++)
        {
            _spriteRend.color = Color.red;
            _enemySound.PlayEnemyHurtSound();
            yield return new WaitForSeconds(_framesDuration / (_flashesCount * 2));
            _spriteRend.color = Color.white;
            yield return new WaitForSeconds(_framesDuration / (_flashesCount * 2));
        }
    }

    private IEnumerator Death()
    {
        _isDead = true;
        _animator.SetTrigger("Death");
        _rb.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(_deathTimer);
        gameObject.SetActive(false);
    }
}
