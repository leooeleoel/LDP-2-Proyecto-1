using UnityEngine;

public class Flotante : MonoBehaviour
{
    public float altura = 0.2f;
    public float velocidad = 2f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float movimiento = Mathf.Sin(Time.time * velocidad) * altura;

        transform.position = posicionInicial + new Vector3(0, movimiento, 0);
    }
}