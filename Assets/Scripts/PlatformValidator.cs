using UnityEngine;
using System.Collections.Generic;

public class PlatformValidator : MonoBehaviour
{
    public float minHorizontalGap = 1f;
    public float samePlatformMinDistance = 4f;

    public float maxReachX = 3f;
    public float maxReachY = 2.5f;

    public float preferredMinHorizontalDistance = 1.5f;
    public float preferredMaxHorizontalDistance = 3f;

    [Range(0f, 1f)]
    public float verticalPathChance = 0.10f;

    public bool IsPositionValid(
        Vector3 newPosition,
        float newWidth,
        GameObject prefab,
        List<PlatformData> generatedPlatforms)
    {
        foreach (PlatformData platform in generatedPlatforms)
        {
            float yDifference =
                Mathf.Abs(
                    newPosition.y -
                    platform.position.y
                );

            float horizontalGap =
                CalculateHorizontalGap(
                    newPosition.x,
                    newWidth,
                    platform.position.x,
                    platform.width
                );

            if (platform.prefab == prefab)
            {
                if (
                    yDifference <= maxReachY &&
                    horizontalGap < samePlatformMinDistance
                )
                {
                    return false;
                }
            }

            if (
                yDifference <= maxReachY &&
                horizontalGap < minHorizontalGap
            )
            {
                return false;
            }
        }

        if (generatedPlatforms.Count > 0)
        {
            if (
                !HasReachablePlatform(
                    newPosition,
                    newWidth,
                    generatedPlatforms
                )
            )
            {
                return false;
            }
        }

        return true;
    }

    public bool HasReachablePlatform(
        Vector3 newPosition,
        float newWidth,
        List<PlatformData> generatedPlatforms)
    {
        foreach (PlatformData platform in generatedPlatforms)
        {
            float yDifference =
                Mathf.Abs(
                    newPosition.y -
                    platform.position.y
                );

            if (yDifference > maxReachY)
                continue;

            float horizontalDistance =
                CalculateHorizontalGap(
                    newPosition.x,
                    newWidth,
                    platform.position.x,
                    platform.width
                );

            if (
                horizontalDistance >=
                preferredMinHorizontalDistance &&
                horizontalDistance <= maxReachX
            )
            {
                return true;
            }

            if (
                horizontalDistance <
                preferredMinHorizontalDistance &&
                horizontalDistance <= maxReachX
            )
            {
                if (Random.value <= verticalPathChance)
                    return true;
            }
        }

        return false;
    }

    public float CalculateHorizontalGap(
        float firstX,
        float firstWidth,
        float secondX,
        float secondWidth)
    {
        float firstLeft =
            firstX - firstWidth / 2f;

        float firstRight =
            firstX + firstWidth / 2f;

        float secondLeft =
            secondX - secondWidth / 2f;

        float secondRight =
            secondX + secondWidth / 2f;

        if (firstRight < secondLeft)
            return secondLeft - firstRight;

        if (firstLeft > secondRight)
            return firstLeft - secondRight;

        return 0f;
    }
}