using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour {

    [SerializeField]
    float mouseSensitivityX = 250f,
    mouseSensitivityY = 250f,
    camClampMax = 60f,
    camClampMin = -60f;

    [SerializeField]
    Transform cameraT;

    private float verticalLookRotation;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, camClampMin, camClampMax);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;
    }
}
