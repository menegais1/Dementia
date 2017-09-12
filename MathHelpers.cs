using System.Collections;
using UnityEngine;

public static class MathHelpers
{
    public static bool Approximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

   /* public static void Lerp(Vector3 initialPosition, Vector3 finalPosition, float timeToLerp,
        float steps, Transform position)
    {
        CoroutineManager.insertNewCoroutine(LerpCoroutine(initialPosition, finalPosition, timeToLerp, steps, position),
            "LerpCoroutine");
    }

    public static IEnumerator LerpCoroutine(Vector3 initialPosition, Vector3 finalPosition, float timeToLerp,
        float steps, Transform position)
    {
        float f = 0;

        while (f <= 1)
        {
            position.position = Vector3.Lerp(initialPosition, finalPosition, f);

            f += 1f / steps;

            yield return new WaitForSeconds(timeToLerp / steps);
        }

        CoroutineManager.deleteCoroutine("LerpCoroutine");
    }*/
}