using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVCamera : MonoBehaviour
{
    [Header("Camera settings")]
    [SerializeField] private float _cameraAngle = 20f;
    [SerializeField] private float _fieldOfView = 90f;

    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
        cam.fieldOfView = _fieldOfView;

        transform.localRotation = Quaternion.Euler(_cameraAngle, 0f, 0f);
    }
}
