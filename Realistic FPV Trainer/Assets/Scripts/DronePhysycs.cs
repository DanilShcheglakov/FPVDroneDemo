using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DronePhysycs : MonoBehaviour
{
    [Header("Gravity settings")]
    [SerializeField] private float _gravityMultiplier = 2f;

    [Header("Thrust settings")]
    [SerializeField] private float _maxThrust = 20f;        // ћаксимальна€ т€га всех моторов
   // [SerializeField] private float _hoverThrottle = 0.5f;   // ѕроцент газа при котором дрон висит на месте

    [Header("Rotation settings")]
    [SerializeField] private float _pitchTorque = 8f;       // —ила наклона вперЄд/назад
    [SerializeField] private float _rollTorque = 8f;        // —ила крена влево/вправо
    [SerializeField] private float _yawTorque = 1.5f;       // —ила поворота по оси Y

    [Header("Drag settings")]
    [SerializeField] private float _linearDrag = 2f;        // —опротивление движению
    [SerializeField] private float _angularDrag = 3f;

    private Rigidbody _rb;

    private readonly Vector3[] _motorPositions = new Vector3[]
    {
        new Vector3(-0.2f,  0f,  0.2f),  // M1 передний левый
        new Vector3( 0.2f,  0f,  0.2f),  // M2 передний правый
        new Vector3( 0.2f,  0f, -0.2f),  // M3 задний правый
        new Vector3(-0.2f,  0f, -0.2f),  // M4 задний левый
    };

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.drag = 0f;
        _rb.angularDrag = 0f;
        _rb.constraints = RigidbodyConstraints.None;
    }

    private void FixedUpdate()
    {
        if (DroneInput.Instance == null) return;

        float throttle = DroneInput.Instance.Throttle;
        float yaw = DroneInput.Instance.Yaw;
        float pitch = DroneInput.Instance.Pitch;
        float roll = DroneInput.Instance.Roll;

        // ¬ычисл€ем т€гу каждого мотора.
        // Ѕазова€ т€га Ч это throttle.
        // Pitch, Roll, Yaw добавл€ют или убирают т€гу у конкретных моторов.
        // —хема моторов:
        //   M1(FL) M2(FR)
        //   M4(BL) M3(BR)
        //
        // Pitch вперЄд Ч передние (M1,M2) сбавл€ют, задние (M3,M4) прибавл€ют
        // Roll вправо  Ч левые (M1,M4) прибавл€ют, правые (M2,M3) сбавл€ют
        // Yaw по часовой Ч M1,M3 прибавл€ют, M2,M4 сбавл€ют

        float pitchFactor = pitch * _pitchTorque / _maxThrust;
        float rollFactor = roll * _rollTorque / _maxThrust;

        float[] motorThrusts = new float[4];
        motorThrusts[0] = throttle - pitchFactor + rollFactor;  // M1 передний левый
        motorThrusts[1] = throttle - pitchFactor - rollFactor;  // M2 передний правый
        motorThrusts[2] = throttle + pitchFactor - rollFactor;  // M3 задний правый
        motorThrusts[3] = throttle + pitchFactor + rollFactor;  // M4 задний левый

        // ѕримен€ем силу от каждого мотора
        for (int i = 0; i < 4; i++)
        {
            // Clamp Ч ограничиваем т€гу от 0 до 1, мотор не может т€нуть вниз
            float thrust = Mathf.Clamp(motorThrusts[i], 0f, 1f) * _maxThrust;

            // ѕереводим локальную позицию мотора в мировые координаты
            Vector3 worldMotorPos = transform.TransformPoint(_motorPositions[i]);

            // —ила направлена вверх относительно дрона (transform.up),
            // приложена в точке расположени€ мотора Ч это создаЄт торк автоматически
            _rb.AddForceAtPosition(transform.up * thrust, worldMotorPos, ForceMode.Force);
        }
        _rb.AddRelativeTorque(Vector3.up * yaw * _yawTorque, ForceMode.Force);

        ApplyDrag();
        _rb.AddForce(Vector3.down * Physics.gravity.magnitude * _gravityMultiplier * _rb.mass, ForceMode.Force);
    }

    private void ApplyDrag()
    {
        _rb.AddForce(-_rb.velocity * _linearDrag, ForceMode.Force);

        _rb.AddTorque(-_rb.angularVelocity * _angularDrag, ForceMode.Force);
    }
}
