using System;
using UnityEngine;

/// <summary>
/// Helper class
/// </summary>
[Serializable]
public class Utils
{
    private const float _epsilon = 0.001f;
    public static bool IsFloatEpsilonZero(float value, float epsilon = _epsilon )
    {
        return value > -epsilon && value < epsilon;
    }
    public float Epsilon { get { return _epsilon; } }
}

public static class ExtensionMethods
{
    public static bool IsAnalyzable(this GameObject gameObject)
    {
        return gameObject.GetComponent<Analyzable>() ?? false;
    }
}