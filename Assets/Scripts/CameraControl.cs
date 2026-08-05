using UnityEngine;

public class LoopUltimaImagen : MonoBehaviour
{
    public Transform img4;
    public Transform img4Clone;
    public float speed = 2f;

    float height;

    void Start()
    {
        height = img4.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        // Mover cámara
        transform.position += Vector3.up * speed * Time.deltaTime;

        // Cuando la cámara pasa img4 → moverla arriba
        if (transform.position.y >= img4.position.y + height)
        {
            img4.position = new Vector3(
                img4.position.x,
                img4Clone.position.y + height,
                img4.position.z
            );
        }

        // Lo mismo para el clon
        if (transform.position.y >= img4Clone.position.y + height)
        {
            img4Clone.position = new Vector3(
                img4Clone.position.x,
                img4.position.y + height,
                img4Clone.position.z
            );
        }
    }
}