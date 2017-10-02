using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothing;
    Vector3 offset;
    private const float timeToZoomCamera = 2f;
    private static float initialSize;
    private Vector3 velocity;
    private Vector3 cameraOldPosition;
    private Vector3 targetOldPosition;


    // Use this for initialization
    void Start()
    {
        initialSize = GetComponent<Camera>().orthographicSize;
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        var playerCamPos = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, playerCamPos, ref velocity, smoothing);
    }

    public static IEnumerator ZoomCameraCoroutine(float zoomSize, float steps, bool zoomOut)
    {
        float f = 0;

        while (f <= 1)
        {
            Camera.main.orthographicSize =
                zoomOut ? Mathf.Lerp(initialSize, zoomSize, f) : Mathf.Lerp(zoomSize, initialSize, f);

            f += 1f / steps;

            yield return new WaitForSeconds(timeToZoomCamera / steps);
        }

        CoroutineManager.DeleteCoroutine("ZoomCameraCoroutine");
    }

   /* Vector3 SuperSmoothLerp(Vector3 x0, Vector3 y0, Vector3 yt, float t, float k)
    {
        Vector3 f = x0 - y0 + (yt - y0) / (k * t);
        return yt - (yt - y0) / (k * t) + f * Mathf.Exp(-k * t);
    }

     Vector3 SmoothApproach(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float speed)
     {
         float t = Time.deltaTime * speed;
         Vector3 v = (targetPosition - pastTargetPosition) / t;
         Vector3 f = pastPosition - pastTargetPosition + v;
         return targetPosition - v + f * Mathf.Exp(-t);
     }*/
}