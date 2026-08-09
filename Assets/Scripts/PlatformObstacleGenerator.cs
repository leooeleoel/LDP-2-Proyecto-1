using UnityEngine;
using System.Collections.Generic;

public class PlatformObstacleGenerator : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    [Range(0f, 1f)]
    public float obstacleChance = 0.25f;

    public float obstacleSafeSpace = 1.2f;
    public float obstacleHeightFromPath = 0f;
    public float obstacleVerticalOffset = -0.4f;

    public float minVerticalDistance = 1.5f;
    public float preferredMinHorizontalDistance = 1.5f;

    public float maxReachX = 3f;
    public float maxReachY = 2.5f;

    public float minX = -5f;
    public float maxX = 5f;

    public float obstaclePositionMinDistance = 2.5f;

    public int obstaclePositionAttempts = 30;

    public void TryCreateObstacle(
        Vector3 newPosition,
        float newWidth,
        List<PlatformData> generatedPlatforms,
        List<GameObject> generatedObstacles)
    {
        if (
            obstaclePrefabs == null ||
            obstaclePrefabs.Length == 0
        )
        {
            return;
        }

        if (generatedPlatforms.Count < 2)
            return;

        if (Random.value > obstacleChance)
            return;

        PlatformData previous =
            generatedPlatforms[
                generatedPlatforms.Count - 2
            ];

        PlatformData current =
            generatedPlatforms[
                generatedPlatforms.Count - 1
            ];

        float verticalDistance =
            Mathf.Abs(
                current.position.y -
                previous.position.y
            );

        float horizontalDistance =
            Mathf.Abs(
                current.position.x -
                previous.position.x
            );

        if (
            verticalDistance <
            minVerticalDistance
        )
        {
            return;
        }

        if (
            horizontalDistance <
            preferredMinHorizontalDistance
        )
        {
            return;
        }

        GameObject obstaclePrefab =
            obstaclePrefabs[
                Random.Range(
                    0,
                    obstaclePrefabs.Length
                )
            ];

        if (obstaclePrefab == null)
            return;

        SpriteRenderer renderer =
            obstaclePrefab.GetComponentInChildren<SpriteRenderer>();

        float obstacleWidth = 0.5f;

        if (renderer != null)
        {
            obstacleWidth =
                renderer.bounds.size.x;
        }

        for (
            int attempt = 0;
            attempt < obstaclePositionAttempts;
            attempt++
        )
        {
            float t =
                Random.Range(
                    0.25f,
                    0.75f
                );

            float obstacleX =
                Mathf.Lerp(
                    previous.position.x,
                    current.position.x,
                    t
                );

            float obstacleY =
                Mathf.Lerp(
                    previous.position.y,
                    current.position.y,
                    0.5f
                );

            obstacleY +=
                obstacleHeightFromPath +
                obstacleVerticalOffset;

            if (
                IsTooCloseToPlatform(
                    obstacleX,
                    previous.position.x,
                    previous.width,
                    obstacleWidth
                )
            )
            {
                continue;
            }

            if (
                IsTooCloseToPlatform(
                    obstacleX,
                    current.position.x,
                    current.width,
                    obstacleWidth
                )
            )
            {
                continue;
            }

            if (
                IsTooCloseToAnotherObstacle(
                    obstacleX,
                    obstacleY,
                    obstacleWidth,
                    generatedObstacles
                )
            )
            {
                continue;
            }

            if (
                !HasSafeRoute(
                    previous,
                    current,
                    obstacleX,
                    obstacleWidth
                )
            )
            {
                continue;
            }

            Vector3 position =
                new Vector3(
                    obstacleX,
                    obstacleY,
                    0f
                );

            GameObject obstacle =
                Instantiate(
                    obstaclePrefab,
                    position,
                    Quaternion.identity
                );

            generatedObstacles.Add(
                obstacle
            );

            return;
        }
    }

    public void ValidateLastObstacle(
        List<PlatformData> generatedPlatforms,
        List<GameObject> generatedObstacles)
    {
        if (
            generatedObstacles == null ||
            generatedObstacles.Count == 0
        )
        {
            return;
        }

        if (
            generatedPlatforms == null ||
            generatedPlatforms.Count < 3
        )
        {
            return;
        }

        GameObject obstacle =
            generatedObstacles[
                generatedObstacles.Count - 1
            ];

        if (obstacle == null)
        {
            generatedObstacles.RemoveAt(
                generatedObstacles.Count - 1
            );

            return;
        }

        int obstacleIndex =
            generatedPlatforms.Count - 2;

        if (
            obstacleIndex < 1 ||
            obstacleIndex >=
            generatedPlatforms.Count - 1
        )
        {
            return;
        }

        PlatformData previous =
            generatedPlatforms[
                obstacleIndex - 1
            ];

        PlatformData current =
            generatedPlatforms[
                obstacleIndex
            ];

        PlatformData next =
            generatedPlatforms[
                obstacleIndex + 1
            ];

        SpriteRenderer renderer =
            obstacle.GetComponentInChildren<SpriteRenderer>();

        float obstacleWidth = 0.5f;

        if (renderer != null)
        {
            obstacleWidth =
                renderer.bounds.size.x;
        }

        bool firstJumpSafe =
            HasSafeRoute(
                previous,
                current,
                obstacle.transform.position.x,
                obstacleWidth
            );

        bool secondJumpSafe =
            HasSafeRoute(
                current,
                next,
                obstacle.transform.position.x,
                obstacleWidth
            );

        if (
            !firstJumpSafe ||
            !secondJumpSafe
        )
        {
            Destroy(obstacle);

            generatedObstacles.RemoveAt(
                generatedObstacles.Count - 1
            );
        }
    }

    bool HasSafeRoute(
        PlatformData previous,
        PlatformData current,
        float obstacleX,
        float obstacleWidth)
    {
        float leftRoute =
            obstacleX -
            obstacleWidth / 2f -
            obstacleSafeSpace;

        float rightRoute =
            obstacleX +
            obstacleWidth / 2f +
            obstacleSafeSpace;

        bool leftSafe =
            IsRouteReachable(
                previous,
                current,
                leftRoute
            );

        bool rightSafe =
            IsRouteReachable(
                previous,
                current,
                rightRoute
            );

        return leftSafe || rightSafe;
    }

    bool IsRouteReachable(
        PlatformData previous,
        PlatformData current,
        float routeX)
    {
        if (
            routeX <
            minX
        )
        {
            return false;
        }

        if (
            routeX >
            maxX
        )
        {
            return false;
        }

        float previousDistance =
            GetPlatformEdgeDistance(
                previous,
                routeX
            );

        float currentDistance =
            GetPlatformEdgeDistance(
                current,
                routeX
            );

        if (
            previousDistance >
            maxReachX
        )
        {
            return false;
        }

        if (
            currentDistance >
            maxReachX
        )
        {
            return false;
        }

        float verticalDistance =
            Mathf.Abs(
                current.position.y -
                previous.position.y
            );

        if (
            verticalDistance >
            maxReachY
        )
        {
            return false;
        }

        return true;
    }

    bool IsTooCloseToPlatform(
        float obstacleX,
        float platformX,
        float platformWidth,
        float obstacleWidth)
    {
        float distance =
            Mathf.Abs(
                obstacleX -
                platformX
            );

        float minimumDistance =
            platformWidth / 2f +
            obstacleWidth / 2f +
            0.3f;

        return distance <
               minimumDistance;
    }

    bool IsTooCloseToAnotherObstacle(
        float obstacleX,
        float obstacleY,
        float obstacleWidth,
        List<GameObject> generatedObstacles)
    {
        foreach (
            GameObject obstacle
            in generatedObstacles)
        {
            if (obstacle == null)
                continue;

            float horizontalDistance =
                Mathf.Abs(
                    obstacleX -
                    obstacle.transform.position.x
                );

            float verticalDistance =
                Mathf.Abs(
                    obstacleY -
                    obstacle.transform.position.y
                );

            float minimumDistance =
                obstacleWidth +
                obstaclePositionMinDistance;

            if (
                horizontalDistance <
                minimumDistance &&
                verticalDistance <
                maxReachY
            )
            {
                return true;
            }
        }

        return false;
    }

    float GetPlatformEdgeDistance(
        PlatformData platform,
        float targetX)
    {
        float left =
            platform.position.x -
            platform.width / 2f;

        float right =
            platform.position.x +
            platform.width / 2f;

        if (
            targetX >= left &&
            targetX <= right
        )
        {
            return 0f;
        }

        if (
            targetX < left
        )
        {
            return left -
                   targetX;
        }

        return targetX -
               right;
    }
}