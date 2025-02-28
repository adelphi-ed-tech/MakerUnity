using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    public Transform debugPos;
    private Vector3 _prevPos;
    private Quaternion _prevRot;
    public bool debug;
    public float debugCamSpeed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!debug)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        else
        {
            Vector3 pos = debugPos.position;
            pos.x += Input.GetAxisRaw("Horizontal") * Time.deltaTime * debugCamSpeed;
            pos.z += Input.GetAxisRaw("Vertical") * Time.deltaTime * debugCamSpeed;
            debugPos.position = pos;

            debugPos.position += transform.forward * (Input.mouseScrollDelta.y * 5);
            
            transform.position = debugPos.position;
            transform.rotation = debugPos.rotation;

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            debug = !debug;
            if (debug)
            {
                _prevPos = transform.position;
                _prevRot = transform.rotation;
                transform.position = debugPos.position;
                transform.rotation = debugPos.rotation;
            }
            else
            {
                transform.position = _prevPos;
                transform.rotation = _prevRot;
            }
        }
    }
}
