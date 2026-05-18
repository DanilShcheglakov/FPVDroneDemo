using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInput : MonoBehaviour
{
    public static DroneInput Instance { get; private set; }

    public float Throttle { get; private set; }
    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    public float Roll { get; private set; }

    private DroneInputActions _actions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        _actions = new DroneInputActions();
        _actions.Drone.Enable();
    }

    private void Update()
    {
        Throttle = _actions.Drone.Throttle.ReadValue<float>();
        Yaw = _actions.Drone.Yaw.ReadValue<float>();
        Pitch = _actions.Drone.Pitch.ReadValue<float>();
        Roll = _actions.Drone.Roll.ReadValue<float>();

        if (Throttle != 0) Debug.Log($"Throttle: {Throttle}");
    }

    private void OnDestroy()
    {
        _actions.Dispose();
    }
}
