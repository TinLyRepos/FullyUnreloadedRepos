using System.Collections;
using UnityEngine;

public static class HelperUtils
{
    public static bool IsIntervalOverlapping(int min1, int max1, int min2, int max2)
    {
        if (Mathf.Max(min1, min2) <= Mathf.Min(max1, max2))
            return true;

        return false;
    }

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
}