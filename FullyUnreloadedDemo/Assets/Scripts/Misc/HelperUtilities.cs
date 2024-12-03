using System.Collections;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;

    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);
        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        Vector3 directionVector =
            new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    public static Vector3 GetClosetSpawnPosition(Vector3 prefPosition)
    {
        Room currentRoom = GameManager.Instance.CurrentRoom;

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 closetSpawnPosition = new Vector3(float.MaxValue, float.MaxValue, 0.0f);

        // Loop through room spawn positions
        foreach (Vector2Int spawnPosition in currentRoom.spawnPositionArray)
        {
            // convert the spawn position to world position
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPosition);

            if (Vector3.Distance(spawnPositionWorld, prefPosition) < Vector3.Distance(closetSpawnPosition, prefPosition))
                closetSpawnPosition = spawnPositionWorld;
        }

        return closetSpawnPosition;
    }

    public static bool IsIntervalOverlapping(int min1, int max1, int min2, int max2)
    {
        if (Mathf.Max(min1, min2) <= Mathf.Min(max1, max2))
            return true;

        return false;
    }

    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;

        // formula to convert from the linear scale to the logarithmic decibel scale
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    /// Get the camera viewport lower and upper bounds
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds, out Vector2Int cameraWorldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }

    //===========================================================================
    public static bool ValidateCheckNullValue(Object obj, string fieldName, Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.LogWarning($"{fieldName} is null and must contain a value in object {obj.name}");
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEmptyString(Object obj, string fieldName, string stringToCheck)
    {
        if (stringToCheck == string.Empty)
        {
            Debug.LogWarning($"{obj.name}: {fieldName} is empty and must contain a value");
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object obj, string fieldName, IEnumerable enumerableObjects)
    {
        if (enumerableObjects == null)
        {
            Debug.LogWarning($"{fieldName} is null in object {obj.name}");
            return true;
        }

        bool error = false;
        int count = 0;

        foreach (var item in enumerableObjects)
        {
            if (item == null)
            {
                Debug.LogWarning($"{fieldName} has null value(s) in object {obj.name}");
                error = true;
            }
            else
            {
                ++count;
            }
        }

        if (count == 0)
        {
            Debug.LogWarning($"{fieldName} has no values in object {obj.name}");
            error = true;
        }

        return error;
    }

    public static bool ValidateCheckPositiveValue(Object obj, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;
        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.LogWarning($"{fieldName} must contain a positive value or zero in object {obj.name}");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.LogWarning($"{fieldName} must contain a positive value in object {obj.name}");
                error = true;
            }
        }
        return error;
    }

    public static bool ValidateCheckPositiveValue(Object obj, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;
        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.LogWarning($"{fieldName} must contain a positive value or zero in object {obj.name}");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.LogWarning($"{fieldName} must contain a positive value in object {obj.name}");
                error = true;
            }
        }
        return error;
    }

    public static bool ValidateCheckPositiveRange(Object obj,
         string fieldNameMin, float valueToCheckMin, string fieldNameMax, float valueToCheckMax, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMin > valueToCheckMax)
        {
            Debug.LogWarning($"{obj.name}: {fieldNameMin} must be less than or equal to {fieldNameMax}");
            error = true;
        }

        if (ValidateCheckPositiveValue(obj, fieldNameMin, valueToCheckMin, isZeroAllowed))
            error = true;

        if (ValidateCheckPositiveValue(obj, fieldNameMin, valueToCheckMin, isZeroAllowed))
            error = true;

        return error;
    }
}