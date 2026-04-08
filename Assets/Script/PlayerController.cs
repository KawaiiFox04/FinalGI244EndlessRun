// PlayerController.cs
// ติดไว้ที่ Player (Capsule) ใน Scene: Gameplay
// - กระโดดได้ 2 ครั้ง (Double Jump)
// - รับ Damage จาก Obstacle
// - เก็บ Item (Heal / SpeedBoost)
// - รองรับ Invincible state
// - รองรับ Size change (ตัวเล็ก)

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
    public TextMeshProUGUI hpText;      // แสดง HP ใน Canvas

    // ---- Runtime State ----
    [HideInInspector] public bool IsInvincible;   // ตอน SpeedBoost
    [HideInInspector] public int  CurrentHp;

    private Rigidbody       rb;
    private CapsuleCollider col;
    private Animator        anim;

    private bool  isGrounded;
    private int   jumpCount;
    private bool  isDead;

    private float originalColHeight;
    private Vector3 originalColCenter;
    private Vector3 originalScale;

    // ================================================================
    void Start()
    {
        rb   = GetComponent<Rigidbody>();
        col  = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();

        // ล็อคแกน Z และป้องกันหมุน
        rb.constraints = RigidbodyConstraints.FreezeRotation
                       | RigidbodyConstraints.FreezePositionZ;

        originalColHeight = col.height;
        originalColCenter = col.center;
        originalScale     = transform.localScale;

        CurrentHp = maxHp;
        RefreshHpUI();
    }

    void Update()
    {
        if (isDead) return;
        CheckGround();
        HandleJump();
    }

    // ================================================================
    //  Ground Check
    // ================================================================
    void CheckGround()
    {
        Vector3 origin = transform.position + col.center;
        float   len    = col.height * 0.5f + groundCheckDist;
        isGrounded = Physics.Raycast(origin, Vector3.down, len, groundLayer);
        if (isGrounded) jumpCount = 0;
        Debug.DrawRay(origin, Vector3.down * len, isGrounded ? Color.green : Color.red);
    }

    // ================================================================
    //  Jump (New Input System)
    // ================================================================
    void HandleJump()
    {
        bool jumped = false;

        var kb = Keyboard.current;
        if (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame))
            jumped = true;

        var touch = Touchscreen.current;
        if (touch != null)
            foreach (var f in touch.touches)
                if (f.press.wasPressedThisFrame) jumped = true;

        if (jumped && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
            anim?.SetTrigger("Jump");
        }
    }

    // ================================================================
    //  Damage
    // ================================================================
    public void TakeDamage(int amount = 1)
    {
        if (IsInvincible || isDead) return;
        CurrentHp -= amount;
        RefreshHpUI();

        if (CurrentHp <= 0)
            Die();
        else
            anim?.SetTrigger("Hit");
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        anim?.SetTrigger("Die");
        GameplayManager.Instance?.OnPlayerDied();
    }

    // ================================================================
    //  Heal
    // ================================================================
    public void Heal(int amount = 1)
    {
        CurrentHp = Mathf.Min(CurrentHp + amount, maxHp);
        RefreshHpUI();
    }

    // ================================================================
    //  Size Change (เล็กลง / กลับปกติ)
    // ================================================================
    public void SetSmallSize(bool small)
    {
        if (small)
        {
            transform.localScale = originalScale * 0.5f;
            col.height = originalColHeight * 0.5f;
            float delta = (originalColHeight - col.height) * 0.5f;
            col.center = originalColCenter - new Vector3(0, delta, 0);
        }
        else
        {
            transform.localScale = originalScale;
            col.height = originalColHeight;
            col.center = originalColCenter;
        }
    }

    // ================================================================
    //  Reset
    // ================================================================
    public void ResetPlayer()
    {
        isDead       = false;
        jumpCount    = 0;
        IsInvincible = false;
        CurrentHp    = maxHp;
        SetSmallSize(false);
        rb.linearVelocity = Vector3.zero;
        anim?.Rebind();
        RefreshHpUI();
    }

    // ================================================================
    //  UI
    // ================================================================
    void RefreshHpUI()
    {
        if (hpText == null) return;
        // แสดงเป็นหัวใจ ❤ ตามจำนวน HP
        string hearts = "";
        for (int i = 0; i < maxHp; i++)
            hearts += i < CurrentHp ? "❤️" : "🖤";
        hpText.SetText(hearts);
    }

    // ================================================================
    //  Gizmos
    // ================================================================
    void OnDrawGizmosSelected()
    {
        if (col == null) return;
        var o = transform.position + col.center;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(o, o + Vector3.down * (col.height * 0.5f + groundCheckDist));
    }
}