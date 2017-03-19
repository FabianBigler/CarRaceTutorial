using UnityEngine;
using System.Collections;

public class WheelBehaviour : MonoBehaviour {
    public WheelCollider wheelCol;
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
}
