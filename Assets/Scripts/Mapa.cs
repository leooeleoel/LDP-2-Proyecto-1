using UnityEngine;

public class Mapa : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MapaCounter contador = FindFirstObjectByType<MapaCounter>();

            if (contador != null)
            {
                contador.RecogerObjeto();
            }

            Destroy(gameObject);
        }
    }
}