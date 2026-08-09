using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(SpriteAnimator))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask hazardLayer;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 0.4f;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;

    private PlayerController playerController;
    private SpriteAnimator spriteAnimator;
    private AudioSource audioSource;

    private float cooldownTimer;
    private bool hasDealtDamage;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteAnimator = GetComponent<SpriteAnimator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.J) &&
            cooldownTimer <= 0f &&
            !playerController.IsAttacking)
        {
            StartAttack();
        }

        if (playerController.IsAttacking)
        {
            if (!hasDealtDamage)
            {
                DealDamage();
                hasDealtDamage = true;
            }

            if (spriteAnimator.IsFinished)
                playerController.IsAttacking = false;
        }
    }

    private void StartAttack()
    {
        playerController.IsAttacking = true;
        hasDealtDamage = false;
        cooldownTimer = attackCooldown;

        spriteAnimator.Play("Attack");

        // Sonido del ataque
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    private void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            hazardLayer
        );

        foreach (Collider2D hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();

            if (target != null)
                target.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}