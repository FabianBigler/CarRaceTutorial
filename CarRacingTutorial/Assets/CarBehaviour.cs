﻿using UnityEngine;
using System.Collections;
using Assets;
using Assets.GhostCar;

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
    public bool thrustEnabled;
    public bool recording;
    public float maxBrakeTorque;

    public WheelBehaviour[] wheelBehaviours = new WheelBehaviour[4];

    public Texture2D guiSpeedDisplay;
    public Texture2D guiSpeedPointer;    
    public AudioClip engineSingleRPMSoundClip;
    public ParticleSystem smokeL;
    public ParticleSystem smokeR;
    public AudioClip brakeAudioClip;
    

    private ParticleSystem.EmissionModule _smokeLEmission;
    private ParticleSystem.EmissionModule _smokeREmission;
    private AudioSource _engineAudioSource;
    private float _currentSpeedKMH;
    private bool _doSkidmarking = false;
    private AudioSource _brakeAudioSource;
    private float _pastTime;

    void Start() {
        GetComponentInChildren<Renderer>().material.color = BuggyConfiguration.BodyColor;
        //var animator = GetComponent<Animator>();
        //animator.Play("Platformanimation");        
        body = GetComponent<Rigidbody>();
        var transformCenterOfMass = centerOfmass.GetComponent<Transform>();
            
        body.centerOfMass = new Vector3(
            transformCenterOfMass.localPosition.x,
            transformCenterOfMass.localPosition.y,
            transformCenterOfMass.localPosition.z);

        SetFriction(forwardFriction, sidewaysFriction);

        // Configure AudioSource component by program
        _engineAudioSource = gameObject.AddComponent<AudioSource>();
        _engineAudioSource.clip = engineSingleRPMSoundClip;
        _engineAudioSource.loop = true;
        _engineAudioSource.volume = 0.7f;
        _engineAudioSource.playOnAwake = true;
        _engineAudioSource.Play();

        // Configure AudioSource component by program
        _brakeAudioSource = gameObject.AddComponent<AudioSource>();
        _brakeAudioSource.clip = brakeAudioClip;
        _brakeAudioSource.loop = true;        
        _brakeAudioSource.playOnAwake = false;        

        _smokeLEmission = smokeL.emission;
        _smokeREmission = smokeR.emission;
        _smokeLEmission.enabled = true;
        _smokeREmission.enabled = true;
        
    }

    // OnGUI is called on every frame when the orthographic GUI is rendered
    void OnGUI()
    {
        if(guiSpeedDisplay != null)
        {
            // Scale everything to the screen height.
            float scale = 3.0f;
            int sh = Screen.height;
            int size = (int)(sh / scale); // size of speed meter

            int lenN = (int)(size * 0.7777f); // length of needle
            int offN = (int)(size / 8.2f); // offset of needle
                                           // Draw speed meter
            GUI.DrawTexture(new Rect(0, sh - size, size, size),
            guiSpeedDisplay,
            ScaleMode.StretchToFill);
            // Rotate the the coordinate system around a point
            //290 degree / 140 kmh
            float degPerKMH = (float)290 / 140;
            //Debug.Log("kmh:" + _currentSpeedKMH + "," + "degPerKMH: " + degPerKMH);
            GUIUtility.RotateAroundPivot(Mathf.Abs(_currentSpeedKMH) * degPerKMH + 36,
                    new Vector2(lenN / 2 + offN, sh - size + lenN / 2 + offN));
            // Draw the speed pointer
            GUI.DrawTexture(new Rect(offN, sh - size + offN, lenN, lenN),
            guiSpeedPointer,
            ScaleMode.StretchToFill);

        }
    }

    void FixedUpdate()        
    {
        if (!thrustEnabled) return;
       
        var motorTorque = maxTorque * Input.GetAxis("Vertical");
        var steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");

        Debug.Log(Application.persistentDataPath);

        if(recording)
        {
            _pastTime += Time.deltaTime;            
            GhostCarRecorder.Instance.Record(_pastTime, transform.position, transform.rotation, 
                                            wheelFL.transform.rotation, wheelFR.transform.rotation,
                                            wheelBL.transform.rotation, wheelBR.transform.rotation);
        }

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


        if ((_currentSpeedKMH * steerAngleFactor) > 1)
        {
            steerAngle = steerAngle / (_currentSpeedKMH * steerAngleFactor);
        }

        // Evaluate ground under front wheels
        string groundTagFL = string.Empty;
        string groundTagFR = string.Empty;
        int groundTextureFL = 0;
        int groundTextureFR = 0;
        bool carIsOnDrySand;
        bool carIsNotOnSand;
       
        WheelHit hitFL = GetGroundInfos(ref wheelFL, ref groundTagFL, ref groundTextureFL);
        WheelHit hitFR = GetGroundInfos(ref wheelFR, ref groundTagFR, ref groundTextureFR);
        carIsOnDrySand = groundTagFL.CompareTo("Terrain") == 0 && groundTextureFL == 1;
        carIsNotOnSand = !(groundTagFL.CompareTo("Terrain") == 0 && (groundTextureFL <= 1));
                
        SetMotorTorque(motorTorque);
        SetSteerAngle(steerAngle);

        // Determine if the car is driving forwards or backwards
        bool velocityIsForeward = Vector3.Angle(transform.forward,
        body.velocity) < 50f;
        // Determine if the cursor key input means braking
        bool doBraking = _currentSpeedKMH > 0.5f &&
        (Input.GetAxis("Vertical") < 0 && velocityIsForeward ||
        Input.GetAxis("Vertical") > 0 && !velocityIsForeward);

        bool doFullBrake = Input.GetKey("space");
        _doSkidmarking = carIsNotOnSand && doFullBrake && _currentSpeedKMH > 20.0f;
        Debug.Log("DoSkidmark? " + _doSkidmarking.ToString());
        SetBrakeSound(_doSkidmarking);
        SetSkidmarking(_doSkidmarking);

        if (doBraking || doFullBrake)
        {
            float brakeTorque = doFullBrake ? maxBrakeTorque : 3000;
            wheelFL.brakeTorque = brakeTorque;
            wheelFR.brakeTorque = brakeTorque;
            wheelBL.brakeTorque = brakeTorque;
            wheelBR.brakeTorque = brakeTorque;
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

        int gearNum = 0;
        float engineRPM = kmh2rpm(_currentSpeedKMH, out gearNum);
        //Debug.Log("Gear number: " + gearNum);
        SetEngineSound(engineRPM);

        SetParticleSystems(engineRPM);
    }

    void SetSkidmarking(bool doSkidmarking)
    {
        foreach (var wheel in wheelBehaviours)
            wheel.DoSkidmarking(doSkidmarking);
    }

    void SetEngineSound(float engineRPM)
    {
        if (_engineAudioSource == null) return;
        float minRPM = 800;
        float maxRPM = 8000;
        float minPitch = 0.3f;
        float maxPitch = 3.0f;

        var rpmInPercent = (engineRPM - minRPM) / (maxRPM - minRPM);
        float pitch = Mathf.Lerp(minPitch, maxPitch, rpmInPercent);         
        _engineAudioSource.pitch = pitch;
    }

    void SetParticleSystems(float engineRPM)
    {
        float smokeRate = engineRPM / 10.0f;
        _smokeLEmission.rateOverDistance = new ParticleSystem.MinMaxCurve(smokeRate);
        _smokeREmission.rateOverDistance = new ParticleSystem.MinMaxCurve(smokeRate);
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

    class gear
    {
        public gear(float minKMH, float minRPM, float maxKMH, float maxRPM)
        {
            _minRPM = minRPM;
            _minKMH = minKMH;
            _maxRPM = maxRPM;
            _maxKMH = maxKMH;
        }
        private float _minRPM;
        private float _minKMH;
        private float _maxRPM;
        private float _maxKMH;
        public bool speedFits(float kmh)
        {
            return kmh >= _minKMH && kmh <= _maxKMH;
        }
        public float interpolate(float kmh)
        {            
            var kmhInPercent = (kmh - _minKMH) / (_maxKMH - _minKMH);
            var interpolatedRPM = Mathf.Lerp(_minRPM, _maxRPM, kmhInPercent);            
            return interpolatedRPM;
        }
    }

    float kmh2rpm(float kmh, out int gearNum)
    {
        gear[] gears = new gear[]
        {   new gear( 1, 900, 12, 1400),
            new gear( 12, 900, 25, 2000),
            new gear( 25, 1350, 45, 2500),
            new gear( 45, 1950, 70, 3500),
            new gear( 70, 2500, 112, 4000),
            new gear(112, 3100, 180, 5000)
        };
        for (int i = 0; i < gears.Length; ++i)
        {
            if (gears[i].speedFits(kmh))
            {
                gearNum = i + 1;
                return gears[i].interpolate(kmh);
            }
        }
        gearNum = 1;
        return 800;
    }

    // Returns the wheel hit collider, the tag and main texture of the passed wheel c
     WheelHit GetGroundInfos(ref WheelCollider wheelCol,ref string groundTag, ref int groundTextureIndex)
    {
        // Default values
        groundTag = "InTheAir";
        groundTextureIndex = -1;
        // Query ground by ray shoot on the front left wheel collider
        WheelHit wheelHit;
        wheelCol.GetGroundHit(out wheelHit);
        // If not in the air query collider
        if (wheelHit.collider)
        {
            groundTag = wheelHit.collider.tag;
            if (wheelHit.collider.CompareTag("Terrain"))
                groundTextureIndex = TerrainSurface.GetMainTexture(transform.position);
        }
        return wheelHit;
    }

    void SetBrakeSound(bool doBrakeSound)
    {
        if (doBrakeSound)
        {
            _brakeAudioSource.volume = _currentSpeedKMH / 100.0f;
            _brakeAudioSource.Play();
        }
        else
            _brakeAudioSource.Stop();
    }
}