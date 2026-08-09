using UnityEngine;

public class PlatformSelector : MonoBehaviour
{
    [System.Serializable]
    public class NormalPlatform
    {
        public GameObject prefab;

        [Min(0f)]
        public float priority = 1f;
    }

    [System.Serializable]
    public class RarePlatform
    {
        public GameObject prefab;

        [Min(0f)]
        public float priority = 1f;
    }

    [Header("PLATAFORMAS NORMALES")]
    public NormalPlatform[] normalPlatforms;

    [Header("PLATAFORMAS RARAS")]
    public RarePlatform[] rarePlatforms;

    [Range(0f, 1f)]
    [Header("Probabilidad de plataforma rara")]
    public float rarePlatformChance = 0.05f;

    public GameObject ChoosePlatformPrefab()
    {
        bool hasNormal =
            normalPlatforms != null &&
            normalPlatforms.Length > 0;

        bool hasRare =
            rarePlatforms != null &&
            rarePlatforms.Length > 0;

        if (!hasNormal && hasRare)
            return ChooseRarePlatform();

        if (hasNormal && !hasRare)
            return ChooseNormalPlatform();

        if (Random.value < rarePlatformChance)
            return ChooseRarePlatform();

        return ChooseNormalPlatform();
    }

    public GameObject ChooseNormalPlatform()
    {
        if (
            normalPlatforms == null ||
            normalPlatforms.Length == 0
        )
        {
            return null;
        }

        float totalPriority = 0f;

        foreach (NormalPlatform platform in normalPlatforms)
        {
            if (platform.prefab != null)
            {
                totalPriority += Mathf.Max(
                    0f,
                    platform.priority
                );
            }
        }

        if (totalPriority <= 0f)
            return normalPlatforms[0].prefab;

        float randomValue =
            Random.Range(0f, totalPriority);

        float accumulated = 0f;

        foreach (NormalPlatform platform in normalPlatforms)
        {
            if (platform.prefab == null)
                continue;

            accumulated += Mathf.Max(
                0f,
                platform.priority
            );

            if (randomValue <= accumulated)
                return platform.prefab;
        }

        return normalPlatforms[
            normalPlatforms.Length - 1
        ].prefab;
    }

    public GameObject ChooseRarePlatform()
    {
        if (
            rarePlatforms == null ||
            rarePlatforms.Length == 0
        )
        {
            return null;
        }

        float totalPriority = 0f;

        foreach (RarePlatform platform in rarePlatforms)
        {
            if (platform.prefab != null)
            {
                totalPriority += Mathf.Max(
                    0f,
                    platform.priority
                );
            }
        }

        if (totalPriority <= 0f)
            return rarePlatforms[0].prefab;

        float randomValue =
            Random.Range(0f, totalPriority);

        float accumulated = 0f;

        foreach (RarePlatform platform in rarePlatforms)
        {
            if (platform.prefab == null)
                continue;

            accumulated += Mathf.Max(
                0f,
                platform.priority
            );

            if (randomValue <= accumulated)
                return platform.prefab;
        }

        return rarePlatforms[
            rarePlatforms.Length - 1
        ].prefab;
    }
}