using UnityEngine;

// ใช้ Rigidbody (3D) กับ CapsuleCollider (3D)
// ล็อค Movement ให้อยู่แค่แกน X และ Y เท่านั้น (ไม่ขยับแกน Z)
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public int maxJumpCount = 2;           // 2 = Double Jump

    [Header("Ground Check")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("Slide Settings")]
    public float slideTime = 0.8f;
    [Range(0.1f, 0.9f)]
    public float slideCrouchRatio = 0.5f;  // ย่อ Collider เหลือกี่ % ตอน Slide

    // ---- Private ----
    private Rigidbody rb;
    private CapsuleCollider col;
    private Animator animator;

    private bool isGrounded;
    private int jumpCount;
    private bool isSliding;
    private float slideTimer;
    private bool isDead;

    private float originalColHeight;
    private Vector3 originalColCenter;

    void Start()
    {
        rb  = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>(); // ไม่บังคับ

        // ล็อคไม่ให้หมุน และไม่ให้ขยับแกน Z
        rb.constraints = RigidbodyConstraints.FreezeRotation
                       | RigidbodyConstraints.FreezePositionZ;

        originalColHeight = col.height;
        originalColCenter = col.center;
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();
        HandleJump();
        HandleSlide();
        UpdateAnimator();
    }

    // ================================================================
    //  Ground Check — ยิง Raycast ลงจากใต้ Capsule
    // ================================================================
    void CheckGround()
    {
        Vector3 origin = transform.position + col.center;
        float   rayLen = (col.height * 0.5f) + groundCheckDistance;

        isGrounded = Physics.Raycast(origin, Vector3.down, rayLen, groundLayer);
        if (isGrounded) jumpCount = 0;

        // Debug line ให้เห็นใน Scene View
        Debug.DrawRay(origin, Vector3.down * rayLen,
                      isGrounded ? Color.green : Color.red);
    }

    // ================================================================
    //  Jump
    // ================================================================
    void HandleJump()
    {
        bool jumped = Input.GetKeyDown(KeyCode.Space)
                   || Input.GetKeyDown(KeyCode.UpArrow)
                   || Input.GetKeyDown(KeyCode.W);

        // Mobile — แตะครึ่งบนจอ
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began && t.position.y > Screen.height * 0.5f)
                jumped = true;
        }

        if (jumped && jumpCount < maxJumpCount && !isSliding)
        {
            // Reset velocity Y ก่อนเพื่อให้ Double Jump รู้สึก Responsive
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
            animator?.SetTrigger("Jump");
        }
    }

    // ================================================================
    //  Slide
    // ================================================================
    void HandleSlide()
    {
        bool slid = Input.GetKeyDown(KeyCode.DownArrow)
                 || Input.GetKeyDown(KeyCode.S);

        // Mobile — แตะครึ่งล่างจอ
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began && t.position.y < Screen.height * 0.5f)
                slid = true;
        }

        if (slid && isGrounded && !isSliding)
            StartSlide();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f) StopSlide();
        }
    }

    void StartSlide()
    {
        isSliding  = true;
        slideTimer = slideTime;

        // ย่อ Collider ลงและดัน Center ลงมาด้วย ไม่งั้นจะลอย
        float newHeight   = originalColHeight * slideCrouchRatio;
        float centerDelta = (originalColHeight - newHeight) * 0.5f;

        col.height = newHeight;
        col.center = originalColCenter - new Vector3(0f, centerDelta, 0f);

        animator?.SetBool("IsSliding", true);
    }

    void StopSlide()
    {
        isSliding  = false;
        col.height = originalColHeight;
        col.center = originalColCenter;
        animator?.SetBool("IsSliding", false);
    }

    // ================================================================
    //  Animator
    // ================================================================
    void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetBool ("IsGrounded",    isGrounded);
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    // ================================================================
    //  Die / Reset
    // ================================================================
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        animator?.SetTrigger("Die");
        GameManager.Instance?.OnPlayerDied();
    }

    public void ResetPlayer()
    {
        isDead     = false;
        jumpCount  = 0;
        isSliding  = false;
        StopSlide();
        rb.linearVelocity = Vector3.zero;
        animator?.Rebind();
    }

    // ================================================================
    //  Gizmos
    // ================================================================
    void OnDrawGizmosSelected()
    {
        if (col == null) return;
        Vector3 origin = transform.position + col.center;
        float   rayLen = (col.height * 0.5f) + groundCheckDistance;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayLen);
    }
}