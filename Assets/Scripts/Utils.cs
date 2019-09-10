using System;

[Serializable]
public class Utils
{
    private const float _epsilon = 0.001f;
    public static bool IsFloatSmallerThanEpsilon(float value, float epsilon = _epsilon )
    {
        return value > -epsilon && value < epsilon;
    }

    public static bool IsFloatGreaterThanEpsilon(float value, float epsilon = _epsilon )
    {
        return value > epsilon;
    }
}
