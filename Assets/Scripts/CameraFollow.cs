using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float autoScrollSpeed = 1f;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float verticalOffset = 1f;
    [SerializeField] private float startDelay = 2f;

    private float highestY;
    private float delayTimer;

    private void Start()
    {
        highestY = transform.position.y;
        delayTimer = startDelay;
    }

    private void LateUpdate()
    {
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        highestY += autoScrollSpeed * Time.deltaTime;

        if (target != null)
        {
            float targetY = target.position.y + verticalOffset;
            if (targetY > highestY) highestY = targetY;
        }

        Vector3 desired = new Vector3(transform.position.x, highestY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}