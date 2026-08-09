using UnityEngine;

public class BeeAttackDamage : MonoBehaviour
{
    public Transform player;
    public float attackDistance = 1f;
    public float yTolerance = 0.5f;
    public int damage = 1;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;
    public float knockbackDuration = 0.2f;

    [Header("Audio")]
    public AudioClip attackHitSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void DealDamage()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
            return;

        float xDistance = Mathf.Abs(player.position.x - transform.position.x);
        float yDistance = Mathf.Abs(player.position.y - transform.position.y);

        if (xDistance <= attackDistance && yDistance <= yTolerance)
        {
            IDamageable target = player.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);

                // Sonido cuando la abeja golpea al Player
                if (attackHitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(attackHitSound);
                }

                // Knockback
                PlayerController playerController = player.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    float direction = player.position.x > transform.position.x ? 1f : -1f;

                    playerController.ApplyKnockback(
                        knockbackForce,
                        knockbackUpForce,
                        knockbackDuration,
                        direction
                    );
                }

                // Pausa de la abeja después de golpear
                EnemyBee bee = GetComponent<EnemyBee>();

                if (bee != null)
                {
                    bee.StartAttackPause();
                }
            }
        }
    }
}