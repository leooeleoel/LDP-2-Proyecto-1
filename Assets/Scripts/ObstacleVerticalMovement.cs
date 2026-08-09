using UnityEngine;

public class ObstacleVerticalMovement : MonoBehaviour
{
    [Header("Movimiento vertical")]
    public float movementHeight = 0.75f;
    public float movementSpeed = 2f;

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        float offset =
            Mathf.Sin(
                Time.time * movementSpeed
            ) * movementHeight;

        transform.position =
            new Vector3(
                transform.position.x,
                startY + offset,
                transform.position.z
            );
    }
}