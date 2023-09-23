using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamControl : MonoBehaviour
{
    public Camera mainCamera;
    public bool isOn;

    public float moveSpeed=0.1f;
    public float turnSpeed=0.1f;

    private Vector3 defaultPos;
    private Quaternion defaultRot;

    Vector3 up;
    Vector3 right;
    Vector3 forward;

    private void Start()
    {
        if (mainCamera == null) return;

        defaultRot = mainCamera.transform.rotation;
        defaultPos = mainCamera.transform.position;
        //forward = mainCamera.transform.forward;
        //up = mainCamera.cameraToWorldMatrix.MultiplyVector(new Vector3(0, 1, 0));
        up = mainCamera.transform.up;
        right = mainCamera.transform.right;
    }

    void Update()
    {
        if (isOn && mainCamera != null)
        {

            if (Input.GetKey("w"))
                mainCamera.transform.position += mainCamera.transform.forward * moveSpeed;
            if (Input.GetKey("s"))
                mainCamera.transform.position -= mainCamera.transform.forward * moveSpeed;
            if (Input.GetKey("d"))
                mainCamera.transform.position += mainCamera.transform.right * moveSpeed;
            if (Input.GetKey("a"))
                mainCamera.transform.position -= mainCamera.transform.right * moveSpeed;
            if (Input.GetKey("e"))
            {
                //mainCamera.transform.forward = Quaternion.AngleAxis(turnSpeed, up) * mainCamera.transform.forward;
                var q = mainCamera.transform.rotation;
                q.SetLookRotation(Quaternion.AngleAxis(turnSpeed, up) * mainCamera.transform.forward, up);
                mainCamera.transform.rotation = q;
            }
            if (Input.GetKey("q")) {
                //mainCamera.transform.forward = Quaternion.AngleAxis(-turnSpeed, up) * mainCamera.transform.forward;
                var q = mainCamera.transform.rotation;
                q.SetLookRotation(Quaternion.AngleAxis(-turnSpeed, up) * mainCamera.transform.forward, up);
                mainCamera.transform.rotation = q;
            }

            if (Input.GetKey("z"))
            {
                //mainCamera.transform.forward = Quaternion.AngleAxis(turnSpeed, mainCamera.transform.right) * mainCamera.transform.forward;
                var q = mainCamera.transform.rotation;
                q.SetLookRotation(Quaternion.AngleAxis(turnSpeed, right) * mainCamera.transform.forward, up);
                mainCamera.transform.rotation = q;
            }
            if (Input.GetKey("c"))
            {
                //mainCamera.transform.forward = Quaternion.AngleAxis(turnSpeed, mainCamera.transform.right) * mainCamera.transform.forward;
                var q = mainCamera.transform.rotation;
                q.SetLookRotation(Quaternion.AngleAxis(-turnSpeed, right) * mainCamera.transform.forward, up);
                mainCamera.transform.rotation = q;
            }

            if (Input.GetKey("r"))
                mainCamera.transform.position += mainCamera.transform.up * moveSpeed;
            if (Input.GetKey("f"))
                mainCamera.transform.position -= mainCamera.transform.up * moveSpeed;

            if (Input.GetKey("x"))
            {
                mainCamera.transform.position = defaultPos;
                mainCamera.transform.rotation = defaultRot;
            }
        }
    }
}
