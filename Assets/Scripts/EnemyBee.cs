using UnityEngine;
using System.Collections;

public class EnemyBee : MonoBehaviour, IDamageable
{
    [Header("Movimiento")]
    public float speed = 0.5f;
    public float chaseSpeed = 1f;
    public float range = 0.5f;

    [Header("Escala")]
    public float scale = 1f;

    [Header("Orientación")]
    public bool spriteMiraDerecha = false;

    [Header("Detección")]
    public float detectionDistance = 2f;
    public float attackDistance = 1f;
    public float yTolerance = 0.5f;

    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 3f;
    public float knockbackTime = 0.2f;

    [Header("Invulnerabilidad")]
    public float invulnerabilityTime = 0.5f;
    private float invulnerabilityTimer = 0f;

    [Header("Muerte")]
    public float deathBlinkTime = 1f;
    public float blinkInterval = 0.1f;

    [Header("Ataque")]
    public float attackInterval = 2f;
    public float attackPause = 0.5f;

    private float nextAttackTime = 0f;
    private float attackPauseTimer = 0f;

    [SerializeField] private AudioClip hurtSound;
    private AudioSource audioSource;

    public Transform player;

    private Vector2 startPos;
    private bool movingRight = true;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float knockbackTimer = 0f;

    private bool isDying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        SetDirection(1);
    }

    void FixedUpdate()
    {
        if (isDying)
            return;
        if (attackPauseTimer > 0f)
        {
            attackPauseTimer -= Time.fixedDeltaTime;
            StopMoving();
            return;
        }

        if (knockbackTimer > 0f)
            knockbackTimer -= Time.fixedDeltaTime;

        if (invulnerabilityTimer > 0f)
            invulnerabilityTimer -= Time.fixedDeltaTime;

        // Durante el knockback no persigue
        if (knockbackTimer > 0f)
            return;

        if (player == null)
            return;

        float xDistance = Mathf.Abs(player.position.x - transform.position.x);
        float yDistance = Mathf.Abs(player.position.y - transform.position.y);

        bool playerDetected =
            xDistance <= detectionDistance &&
            yDistance <= yTolerance;

        if (!playerDetected)
        {
            Patrol();
        }
        else
        {
            if (xDistance > attackDistance)
            {
                ChasePlayer();
            }
            else
            {
                StopMoving();
                LookAtPlayer();

                if (Time.time >= nextAttackTime)
                {
                    animator.SetTrigger("isAttacking");
                    
                    nextAttackTime = Time.time + attackInterval;
                }
            }
        }
    }

    void Patrol()
    {
        float dir = movingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * speed, 0);

        if (transform.position.x >= startPos.x + range)
            movingRight = false;

        if (transform.position.x <= startPos.x - range)
            movingRight = true;

        SetDirection(dir);
    }

    void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * chaseSpeed, 0);

        SetDirection(dir);
    }

    void LookAtPlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;

        SetDirection(dir);
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void SetDirection(float dir)
    {
        float visualDirection;

        if (spriteMiraDerecha)
            visualDirection = dir;
        else
            visualDirection = -dir;

        transform.localScale = new Vector3(
            visualDirection * Mathf.Abs(scale),
            Mathf.Abs(scale),
            Mathf.Abs(scale)
        );
    }

    // ==========================================
    // VIDA Y DAÑO
    // ==========================================

    public void TakeDamage(int damage)
    {
        if (isDying)
            return;

        if (invulnerabilityTimer > 0f)
            return;

        currentHealth -= damage;
        
        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }


        invulnerabilityTimer = invulnerabilityTime;

        float knockbackDirection;

        if (player != null)
        {
            knockbackDirection =
                transform.position.x > player.position.x ? 1f : -1f;
        }
        else
        {
            knockbackDirection = 1f;
        }

        rb.linearVelocity = Vector2.zero;

        rb.linearVelocity = new Vector2(
            knockbackDirection * knockbackForce,
            0f
        );

        knockbackTimer = knockbackTime;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ==========================================
    // MUERTE
    // ==========================================

    void Die()
    {
        if (isDying)
            return;

        isDying = true;

        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Death");

        StartCoroutine(DeathBlink());
    }

    IEnumerator DeathBlink()
    {
        float timer = 0f;

        while (timer < deathBlinkTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval;
        }

        Destroy(gameObject);
    }
    public void StartAttackPause()
    {
        attackPauseTimer = attackPause;
        rb.linearVelocity = Vector2.zero;
    }
}

