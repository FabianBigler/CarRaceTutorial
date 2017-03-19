using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelBL;
    public WheelCollider wheelBR;
    public float forwardFriction;
    public float sidewaysFriction;
    public float maxTorque = 500;
    public float maxSteerAngle = 45;
    public GameObject centerOfmass;
    public Rigidbody body;
    public float maxSpeedKMH;
    public float maxSpeedBackwardKMH;
    public float steerAngleFactor;

    private float _currentSpeedKMH;
    

    void Start() {
        body = GetComponent<Rigidbody>();
        var transformCenterOfMass = centerOfmass.GetComponent<Transform>();
            
        body.centerOfMass = new Vector3(
            transformCenterOfMass.localPosition.x,
            transformCenterOfMass.localPosition.y,
            transformCenterOfMass.localPosition.z);

        SetFriction(forwardFriction, sidewaysFriction);
    }

    void FixedUpdate()        
    {
        var motorTorque = maxTorque * Input.GetAxis("Vertical");
        var steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");

        _currentSpeedKMH = body.velocity.magnitude * 3.6f;
        //buggy drives backwards, if motor torque force is negative
        if (motorTorque < 0)
        {
            if(_currentSpeedKMH > maxSpeedBackwardKMH)            
                motorTorque = 0;            
        } else
        {
            if (_currentSpeedKMH > maxSpeedKMH)
                motorTorque = 0;                    
        }
        
        if((_currentSpeedKMH * steerAngleFactor) > 1 )
        {
            steerAngle = steerAngle / (_currentSpeedKMH * steerAngleFactor);
        }
        
        SetMotorTorque(motorTorque);
        SetSteerAngle(steerAngle);

        // Determine if the car is driving forwards or backwards
        bool velocityIsForeward = Vector3.Angle(transform.forward,
        body.velocity) < 50f;
        // Determine if the cursor key input means braking
        bool doBraking = _currentSpeedKMH > 0.5f &&
        (Input.GetAxis("Vertical") < 0 && velocityIsForeward ||
        Input.GetAxis("Vertical") > 0 && !velocityIsForeward);
        if (doBraking)
        {
            wheelFL.brakeTorque = 3000;
            wheelFR.brakeTorque = 3000;
            wheelBL.brakeTorque = 3000;
            wheelBR.brakeTorque = 3000;
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
        else
        {
            wheelFL.brakeTorque = 0;
            wheelFR.brakeTorque = 0;
            wheelBL.brakeTorque = 0;
            wheelBR.brakeTorque = 0;
            wheelFL.motorTorque = maxTorque * Input.GetAxis("Vertical");
            wheelFR.motorTorque = wheelFL.motorTorque;
        }
    }
    void SetSteerAngle(float angle)
    {
        wheelFL.steerAngle = angle;
        wheelFR.steerAngle = angle;
    }
    void SetMotorTorque(float amount)
    {
        wheelFL.motorTorque = amount;
        wheelFR.motorTorque = amount;
    }

    void SetFriction(float forwardFriction, float sidewaysFriction)
    {
        WheelFrictionCurve f_fwWFC = wheelFL.forwardFriction;
        WheelFrictionCurve f_swWFC = wheelFL.sidewaysFriction;
        f_fwWFC.stiffness = forwardFriction;
        f_swWFC.stiffness = sidewaysFriction;
        wheelFL.forwardFriction = f_fwWFC;
        wheelFL.sidewaysFriction = f_swWFC;
        wheelFR.forwardFriction = f_fwWFC;
        wheelFR.sidewaysFriction = f_swWFC;
        
        wheelBL.forwardFriction = f_fwWFC;
        wheelBL.sidewaysFriction = f_swWFC;
        wheelBR.forwardFriction = f_fwWFC;
        wheelBR.sidewaysFriction = f_swWFC;    
    }
}
