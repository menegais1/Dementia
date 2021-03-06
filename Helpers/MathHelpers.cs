﻿using System.Collections;
using UnityEngine;

public static class MathHelpers
{
    public static bool Approximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

    public static bool Approximately(double a, double b, double threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

    public static bool Approximately(Vector2 a, Vector2 b, float threshold)
    {
        return ((a.x - b.x) < 0 ? ((a.x - b.x) * -1) : (a.x - b.x)) <= threshold &&
               ((a.y - b.y) < 0 ? ((a.y - b.y) * -1) : (a.y - b.y)) <= threshold;
    }

    public static bool Approximately(Vector3 a, Vector3 b, float threshold)
    {
        return ((a.x - b.x) < 0 ? ((a.x - b.x) * -1) : (a.x - b.x)) <= threshold &&
               ((a.y - b.y) < 0 ? ((a.y - b.y) * -1) : (a.y - b.y)) <= threshold &&
               ((a.z - b.z) < 0 ? ((a.z - b.z) * -1) : (a.z - b.z)) <= threshold;
        ;
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