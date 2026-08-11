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

    [Header("Controles")]
    [SerializeField] private KeyCode saltarA = KeyCode.W;
    [SerializeField] private KeyCode saltarB = KeyCode.UpArrow;
    [SerializeField] private KeyCode derechaA = KeyCode.D;
    [SerializeField] private KeyCode derechaB = KeyCode.RightArrow;
    [SerializeField] private KeyCode izquierdaA = KeyCode.A;
    [SerializeField] private KeyCode izquierdaB = KeyCode.LeftArrow;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        horizontalInput = LeerInputHorizontal();

        bool quiereSaltar = Input.GetKeyDown(saltarA) || Input.GetKeyDown(saltarB);

        if (quiereSaltar && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimer = 0f;

            if (jumpSound != null && audioSource != null)
                audioSource.PlayOneShot(jumpSound);
        }

        if (horizontalInput > 0 && !facingRight) Flip();
        else if (horizontalInput < 0 && facingRight) Flip();

        UpdateAnimation();
    }

    private float LeerInputHorizontal()
    {
        float valor = 0f;

        if (Input.GetKey(derechaA) || Input.GetKey(derechaB)) valor += 1f;
        if (Input.GetKey(izquierdaA) || Input.GetKey(izquierdaB)) valor -= 1f;

        return valor;
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