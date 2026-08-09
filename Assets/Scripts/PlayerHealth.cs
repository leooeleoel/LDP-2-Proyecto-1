using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float respawnScreenHeight = 0.5f;
    [SerializeField] private float respawnInvulnerabilityTime = 2f;

    private int currentLives;
    private float invulnerabilityTimer;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private Vector3 lastSafePosition;

    public int CurrentLives => currentLives;
    public int MaxLives => maxLives;
    public bool IsInvulnerable => invulnerabilityTimer > 0f;

    private void Awake()
    {
        currentLives = maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        lastSafePosition = transform.position;
    }

    private void Update()
    {
        if (playerController != null && playerController.IsGrounded && !IsInvulnerable)
            lastSafePosition = transform.position;

        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
            spriteRenderer.enabled = Mathf.FloorToInt(invulnerabilityTimer * 10f) % 2 == 0;

            if (invulnerabilityTimer <= 0f)
                spriteRenderer.enabled = true;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsInvulnerable) return;

        currentLives -= amount;
        invulnerabilityTimer = invulnerabilityTime;

        if (currentLives <= 0) Die();
    }

    public void FallDamage()
    {
        currentLives--;

        if (currentLives <= 0)
        {
            Die();
            return;
        }

        invulnerabilityTimer = respawnInvulnerabilityTime;
        Respawn();
    }

    private void Respawn()
    {
        rb.linearVelocity = Vector2.zero;

        if (mainCamera == null)
        {
            transform.position = lastSafePosition;
            return;
        }

        float camBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
        float camHeight = mainCamera.orthographicSize * 2f;
        float targetY = camBottom + camHeight * respawnScreenHeight;

        transform.position = new Vector3(mainCamera.transform.position.x, targetY, transform.position.z);
    }

    public void Die()
    {
        PlayerPrefs.SetInt("LastResult", 0);
        SceneManager.LoadScene("GameEnd");
    }
}