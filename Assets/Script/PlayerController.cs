using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce    = 8f;
    public int   maxJumpCount = 2;

    [Header("HP")]
    public int maxHp = 3;

    [Header("Ground Check")]
    public float     groundCheckDist = 0.15f;
    public LayerMask groundLayer;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    [HideInInspector] public bool isInvincible;
    [HideInInspector] public int  currentHp;

    private Rigidbody       _rb;
    private CapsuleCollider _col;
    private Animator        _anim;

    private bool  _isGrounded;
    private int   _jumpCount;
    private bool  _isDead;

    private bool  _isInvincibleFrame;
    private float _invincibleTimer;
    public  float invincibleDuration = 1f;

    private float   _originalColHeight;
    private Vector3 _originalColCenter;
    private Vector3 _originalScale;

    // Animator Hash
    private static readonly int JumpHash  = Animator.StringToHash("Jump");
    private static readonly int HitHash   = Animator.StringToHash("Hit");
    private static readonly int DieHash   = Animator.StringToHash("Die");

    void Start()
    {
        _rb   = GetComponent<Rigidbody>();
        _col  = GetComponent<CapsuleCollider>();
        _anim = GetComponent<Animator>();

        _rb.constraints = RigidbodyConstraints.FreezeRotationX
                        | RigidbodyConstraints.FreezeRotationY
                        | RigidbodyConstraints.FreezeRotationZ
                        | RigidbodyConstraints.FreezePositionZ;

        _originalColHeight = _col.height;
        _originalColCenter = _col.center;
        _originalScale     = transform.localScale;

        currentHp = maxHp;
        RefreshHpUI();
    }

    void Update()
    {
        if (_isDead) return;

        if (_isInvincibleFrame)
        {
            _invincibleTimer -= Time.deltaTime;
            if (_invincibleTimer <= 0f)
                _isInvincibleFrame = false;
        }

        CheckGround();
        HandleJump();
    }

    private void CheckGround()
    {
        Vector3 origin    = transform.position + _col.center;
        float   len       = _col.height * 0.5f + groundCheckDist;
        bool    wasOnGround = _isGrounded;
        _isGrounded = Physics.Raycast(origin, Vector3.down, len, groundLayer);

        // Reset jumpCount เฉพาะตอนที่เพิ่ง Landing
        if (_isGrounded && !wasOnGround)
            _jumpCount = 0;

        Debug.DrawRay(origin, Vector3.down * len, _isGrounded ? Color.green : Color.red);
    }

    private void HandleJump()
    {
        bool jumped = false;

        var kb = Keyboard.current;
        if (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame))
            jumped = true;

        var touch = Touchscreen.current;
        if (touch != null)
            foreach (var f in touch.touches)
                if (f.press.wasPressedThisFrame) jumped = true;

        if (jumped && _jumpCount < maxJumpCount)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, 0f);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpCount++;
            if (_anim != null) _anim.SetTrigger(JumpHash);
        }
    }

    public void TakeDamage(int amount = 1)
    {
        if (_isDead || isInvincible || _isInvincibleFrame) return;

        currentHp -= amount;
        Debug.Log($"TakeDamage! HP: {currentHp}/{maxHp}");
        RefreshHpUI();

        if (currentHp <= 0)
        {
            currentHp = 0;
            RefreshHpUI();
            Die();
        }
        else
        {
            _isInvincibleFrame = true;
            _invincibleTimer   = invincibleDuration;
            if (_anim != null) _anim.SetTrigger(HitHash);
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        Debug.Log("Player died!");
        _rb.linearVelocity = Vector3.zero;
        if (_anim != null) _anim.SetTrigger(DieHash);
        if (GameplayManager.Instance != null)
            GameplayManager.Instance.OnPlayerDied();
    }

    public void Heal(int amount = 1)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        RefreshHpUI();
    }

    public void SetSmallSize(bool small)
    {
        if (small)
        {
            transform.localScale = _originalScale * 0.5f;
            _col.height = _originalColHeight * 0.5f;
            float delta = (_originalColHeight - _col.height) * 0.5f;
            _col.center = _originalColCenter - new Vector3(0f, delta, 0f);
        }
        else
        {
            transform.localScale = _originalScale;
            _col.height          = _originalColHeight;
            _col.center          = _originalColCenter;
        }
    }

    public void ResetPlayer()
    {
        _isDead            = false;
        _jumpCount         = 0;
        isInvincible       = false;
        _isInvincibleFrame = false;
        _invincibleTimer   = 0f;
        currentHp          = maxHp;
        SetSmallSize(false);
        _rb.linearVelocity = Vector3.zero;
        if (_anim != null) _anim.Rebind();
        RefreshHpUI();
    }

    private void RefreshHpUI()
    {
        if (hpText == null) return;
        string hp = "";
        for (int i = 0; i < maxHp; i++)
            hp += i < currentHp ? "[ O ] " : "[ X ] ";
        hpText.SetText(hp);
    }

    private void OnDrawGizmosSelected()
    {
        if (_col == null) return;
        Vector3 o = transform.position + _col.center;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(o, o + Vector3.down * (_col.height * 0.5f + groundCheckDist));
    }
}