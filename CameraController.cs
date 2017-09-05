using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player;
    public float smoothing;
    Vector3 offset;
    private const float timeToTakeOf = 2f;
    private static float initialSize;


    // Use this for initialization
    void Start()
    {
        initialSize = GetComponent<Camera>().orthographicSize;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        Vector3 playerCamPos = player.position + offset;

        transform.position = Vector3.Lerp(transform.position, playerCamPos, smoothing);

    }

    public static IEnumerator takeOfCamera(float zoomSize, float steps, bool zoomOut)
    {
        float f = 0;

        while (f <= 1)
        {
            if (zoomOut)
            {
                Camera.main.orthographicSize = Mathf.Lerp(initialSize, zoomSize, f);
            }
            else
            {
                Camera.main.orthographicSize = Mathf.Lerp(zoomSize, initialSize, f);
            }

            f += 1f / steps;

            yield return new WaitForSeconds(timeToTakeOf / steps);
        }
    }



}
