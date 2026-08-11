using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    public int damage = 1;

    [Header("Empuje")]
    public float knockbackHorizontal = 8f;
    public float knockbackVertical = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource =
            GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(
        Collision2D collision)
    {
        if (
            !collision.gameObject.CompareTag(
                "Player"
            )
        )
        {
            return;
        }

        IDamageable damageable =
            collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Rigidbody2D playerRb =
            collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            Vector2 direction =
                collision.transform.position -
                transform.position;

            if (direction.x > 0f)
            {
                direction.x = 1f;
            }
            else
            {
                direction.x = -1f;
            }

            playerRb.linearVelocity =
                new Vector2(
                    direction.x *
                    knockbackHorizontal,
                    knockbackVertical
                );
        }

        if (
            damageSound != null &&
            audioSource != null
        )
        {
            audioSource.PlayOneShot(
                damageSound
            );
        }
    }
}