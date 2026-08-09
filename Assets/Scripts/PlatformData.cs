using UnityEngine;

[System.Serializable]
public class PlatformData
{
    public Vector3 position;
    public float width;
    public GameObject prefab;
    public GameObject instance;

    public PlatformData(
        Vector3 position,
        float width,
        GameObject prefab,
        GameObject instance)
    {
        this.position = position;
        this.width = width;
        this.prefab = prefab;
        this.instance = instance;
    }
}