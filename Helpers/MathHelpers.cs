using System.Collections;
using System.Security.Policy;
using UnityEngine;

public static class MathHelpers
{
    public static bool Approximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

    public static bool Approximately(Vector2 a, Vector2 b, float threshold)
    {
        return ((a.x - b.x) < 0 ? ((a.x - b.x) * -1) : (a.x - b.x)) <= threshold &&
               ((a.y - b.y) < 0 ? ((a.y - b.y) * -1) : (a.y - b.y)) <= threshold;
    }

    public static float Sin(float angleInDegrees)
    {
        return Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
    }

    public static float Cos(float angleInDegrees)
    {
        return Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
    }

    public static float AbsSin(float angleInDegrees)
    {
        return Mathf.Abs(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }

    public static float AbsCos(float angleInDegrees)
    {
        return Mathf.Abs(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}