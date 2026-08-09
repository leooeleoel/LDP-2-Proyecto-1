using System.Collections.Generic;
using UnityEngine;
using static PlatformSelector;

public class PlatformGenerator : MonoBehaviour
{
    [Header("Plataformas invertidas")]
    [Range(0f, 1f)]
    public float flippedPlatformChance = 0.25f;

    [Header("Distancia vertical")]
    public float minVerticalDistance = 1.5f;
    public float maxVerticalDistance = 3f;

    [Header("Área de generación")]
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Generación")]
    public float startHeight = 2f;
    public int initialPlatforms = 10;
    public float generationAhead = 10f;

    [Header("Eliminación")]
    public float deleteDistanceBelowCamera = 5f;

    [Header("Plataformas raras invertidas")]
    public bool flipRarePlatforms = false;

    [Header("Referencias")]
    public PlatformSelector platformSelector;
    public PlatformValidator platformValidator;
    public PlatformObstacleGenerator obstacleGenerator;

    private float nextY;

    private readonly List<PlatformData> generatedPlatforms =
        new List<PlatformData>();

    private readonly List<GameObject> generatedObstacles =
        new List<GameObject>();

    void Start()
    {
        nextY =
            transform.position.y +
            startHeight;

        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        if (Camera.main == null)
            return;

        RemoveObjectsBelowCamera();

        while (
            Camera.main.transform.position.y +
            generationAhead >
            nextY
        )
        {
            float previousY = nextY;

            SpawnPlatform();

            if (
                Mathf.Approximately(
                    previousY,
                    nextY
                )
            )
            {
                break;
            }
        }
    }

    void SpawnPlatform()
    {
        if (
            platformSelector == null ||
            platformValidator == null
        )
        {
            return;
        }

        for (
            int attempt = 0;
            attempt < 100;
            attempt++
        )
        {
            GameObject prefab =
                platformSelector.ChoosePlatformPrefab();

            if (prefab == null)
                continue;

            SpriteRenderer renderer =
                prefab.GetComponent<SpriteRenderer>();

            if (renderer == null)
                continue;

            float platformWidth =
                renderer.bounds.size.x;

            float minPositionX =
                minX +
                platformWidth / 2f;

            float maxPositionX =
                maxX -
                platformWidth / 2f;

            if (minPositionX > maxPositionX)
                continue;

            float randomX =
                Random.Range(
                    minPositionX,
                    maxPositionX
                );

            Vector3 newPosition =
                new Vector3(
                    randomX,
                    nextY,
                    0f
                );

            if (
                platformValidator.IsPositionValid(
                    newPosition,
                    platformWidth,
                    prefab,
                    generatedPlatforms
                )
            )
            {
                CreatePlatform(
                    prefab,
                    newPosition,
                    platformWidth,
                    IsRarePlatform(prefab)
                );

                if (obstacleGenerator != null)
                {
                    obstacleGenerator.TryCreateObstacle(
                        newPosition,
                        platformWidth,
                        generatedPlatforms,
                        generatedObstacles
                    );
                }

                AdvanceNextY();

                return;
            }
        }

        CreateSafePlatform();
    }

    GameObject CreatePlatform(
        GameObject prefab,
        Vector3 position,
        float width,
        bool isRarePlatform)
    {
        GameObject instance =
            Instantiate(
                prefab,
                position,
                Quaternion.identity
            );

        if (
            (!isRarePlatform || flipRarePlatforms) &&
            Random.value < flippedPlatformChance
        )
        {
            Vector3 scale =
                instance.transform.localScale;

            scale.x *= -1f;

            instance.transform.localScale =
                scale;
        }

        generatedPlatforms.Add(
            new PlatformData(
                position,
                width,
                prefab,
                instance
            )
        );

        return instance;
    }

    bool IsRarePlatform(GameObject prefab)
    {
        if (platformSelector == null)
            return false;

        if (
            platformSelector.rarePlatforms == null
        )
        {
            return false;
        }

        foreach (
            RarePlatform rare
            in platformSelector.rarePlatforms)
        {
            if (
                rare != null &&
                rare.prefab == prefab
            )
            {
                return true;
            }
        }

        return false;
    }

    void AdvanceNextY()
    {
        if (Random.value >= 0.3f)
        {
            nextY +=
                Random.Range(
                    minVerticalDistance,
                    maxVerticalDistance
                );
        }
        else
        {
            nextY +=
                Random.Range(
                    0.4f,
                    minVerticalDistance
                );
        }
    }

    void CreateSafePlatform()
    {
        if (generatedPlatforms.Count == 0)
            return;

        PlatformData previous =
            generatedPlatforms[
                generatedPlatforms.Count - 1
            ];

        GameObject prefab =
            platformSelector.ChooseNormalPlatform();

        if (prefab == null)
        {
            prefab =
                platformSelector.ChooseRarePlatform();
        }

        if (prefab == null)
            return;

        SpriteRenderer renderer =
            prefab.GetComponent<SpriteRenderer>();

        if (renderer == null)
            return;

        float width =
            renderer.bounds.size.x;

        float direction =
            Random.value < 0.5f
            ? -1f
            : 1f;

        float newX =
            previous.position.x +
            direction *
            Random.Range(
                platformValidator.preferredMinHorizontalDistance,
                platformValidator.preferredMaxHorizontalDistance
            );

        newX =
            Mathf.Clamp(
                newX,
                minX + width / 2f,
                maxX - width / 2f
            );

        Vector3 safePosition =
            new Vector3(
                newX,
                nextY,
                0f
            );

        CreatePlatform(
            prefab,
            safePosition,
            width,
            IsRarePlatform(prefab)
        );

        AdvanceNextY();
    }

    void RemoveObjectsBelowCamera()
    {
        if (Camera.main == null)
            return;

        float cameraBottom =
            Camera.main.transform.position.y -
            Camera.main.orthographicSize;

        float deleteHeight =
            cameraBottom -
            deleteDistanceBelowCamera;

        for (
            int i = generatedPlatforms.Count - 1;
            i >= 0;
            i--
        )
        {
            if (
                generatedPlatforms[i].position.y <
                deleteHeight
            )
            {
                if (
                    generatedPlatforms[i].instance !=
                    null
                )
                {
                    Destroy(
                        generatedPlatforms[i].instance
                    );
                }

                generatedPlatforms.RemoveAt(i);
            }
        }

        for (
            int i = generatedObstacles.Count - 1;
            i >= 0;
            i--
        )
        {
            if (
                generatedObstacles[i] == null
            )
            {
                generatedObstacles.RemoveAt(i);
                continue;
            }

            if (
                generatedObstacles[i].transform.position.y <
                deleteHeight
            )
            {
                Destroy(
                    generatedObstacles[i]
                );

                generatedObstacles.RemoveAt(i);
            }
        }
    }
}