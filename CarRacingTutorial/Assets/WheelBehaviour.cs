using UnityEngine;
using System.Collections;

public class WheelBehaviour : MonoBehaviour {
    public WheelCollider wheelCol;
    public SkidmarkBehaviour skidmarks;
    private int _skidmarkLast;
    private Vector3 _skidmarkLastPos;

    private void Start()
    {
        _skidmarkLast = -1;

    }
    // Update is called once per frame
    void Update()
    {
        // Get the wheel position and rotation from the wheelcolider
        Quaternion quat;
        Vector3 position;
        wheelCol.GetWorldPose(out position, out quat);
        transform.position = position;
        transform.rotation = quat;
    }

    // Creates skidmarks if handbraking
    public void DoSkidmarking(bool doSkidmarking)
    {
        if (doSkidmarking)
        {
            // do nothing if the wheel isn't touching the ground
            WheelHit hit;
            if (!wheelCol.GetGroundHit(out hit)) return;
            // absolute velocity at wheel in world space
            Vector3 wheelVelo =
            wheelCol.attachedRigidbody.GetPointVelocity(hit.point);
            if (Vector3.Distance(_skidmarkLastPos, hit.point) > 0.1f)
            {
                _skidmarkLast = skidmarks.Add(hit.point + wheelVelo * Time.deltaTime, hit.normal, 0.5f, _skidmarkLast);
                _skidmarkLastPos = hit.point;
            }
        }
        else _skidmarkLast = -1;
    }
}
