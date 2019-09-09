using System;

[Serializable]
public class Utils
{
    public static bool IsFloatEpsilon(float value, float epsilon = 0.01f )
    {
        return value > -epsilon && value < epsilon;
    }

    public static bool IsFloatGreaterThanEpsilon(float value, float epsilon = 0.01f )
    {
        return value > epsilon;
    }
}
