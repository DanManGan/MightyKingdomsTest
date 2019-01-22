using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // The position that the camera will be following
    public float smoothing = 5f;    // The speed with which the camera will be following

    private Vector3 m_Offset;       // The initial offset from the target       

    // Start is called before the first frame update
    void Start()
    {
        // Calculate the initial offset
        m_Offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        // Create a position the camera is aiming for based on the offset from the target
        Vector3 targetCamPos = target.position + m_Offset;

        // Smoothly interpolate between the camera's current position and it's target position
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
