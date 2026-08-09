using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteAnimator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.15f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float horizontalLimit = 10f;
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimer;

    private Rigidbody2D rb;
    private SpriteAnimator spriteAnimator;
    private float horizontalInput;
    private bool isGrounded;
    private bool facingRight = true;
    private float knockbackTimer = 0f;

    public bool IsAttacking { get; set; }
    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimer = 0f;
        }
        if (horizontalInput > 0 && !facingRight) Flip();
        else if (horizontalInput < 0 && facingRight) Flip();

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.fixedDeltaTime;

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
        }
        else
        {
            rb.linearVelocity = new Vector2(
                horizontalInput * moveSpeed,
                rb.linearVelocity.y
            );
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);
        transform.position = pos;
    }

    private void UpdateAnimation()
    {
        if (IsAttacking) return;

        if (!isGrounded)
        {
            spriteAnimator.Play("Jump");
        }
        else if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            spriteAnimator.Play("Run");
        }
        else
        {
            spriteAnimator.PlayStatic(idleSprite);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        spriteAnimator.SetFacingDirection(facingRight);

        if (attackPoint != null)
        {
            Vector3 pos = attackPoint.localPosition;
            pos.x = Mathf.Abs(pos.x) * (facingRight ? 1f : -1f);
            attackPoint.localPosition = pos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
    public void ApplyKnockback(float force, float upForce, float duration, float direction)
    {
        rb.linearVelocity = new Vector2(
            direction * force,
            upForce
        );

        knockbackTimer = duration;
    }

}