using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookAround : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }
    float rotationX = 0f;
    float rotationY = 0f;
    public float sensitivity = 2f;
    float moveSpeed = 10f;

    public Camera cam;
    // Update is called once per frame
    void Update()
    {
        rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX += Input.GetAxis("Mouse Y") * -1 * sensitivity;
        transform.localEulerAngles = new Vector3(rotationX,rotationY,0);

        // WASD/Arrows: move cam
        transform.position += transform.forward * moveSpeed * Input.GetAxisRaw("Vertical") * Time.deltaTime;
        transform.position += transform.right * moveSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime;

        // Zoom out Camera with Mouse Scroll Wheel
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cam.fieldOfView <= 100)
                cam.fieldOfView += 2;
            if (cam.orthographicSize <= 20)
                cam.orthographicSize += 0.5F;
        }
        // Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (cam.fieldOfView > 2)
                cam.fieldOfView -= 2;
            if (cam.orthographicSize >= 1)
                cam.orthographicSize -= 0.5F;
        }

    }

}

    
