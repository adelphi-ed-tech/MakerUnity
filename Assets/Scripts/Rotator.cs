using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public bool rotateParent;
    public float rotateSpeed;

    private Transform rotateTransform;

    // Start is called before the first frame update
    void Start()
    {
        rotateTransform = rotateParent ? transform.parent : transform;
    }

    // Update is called once per frame
    void Update()
    {
        rotateTransform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
