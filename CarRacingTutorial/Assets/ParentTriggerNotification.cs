using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentTriggerNotification : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        var timingBehaviour = this.transform.parent.gameObject.GetComponent<TimingBehaviour>();
        timingBehaviour.ChildTriggerEntered(this.tag);        
    }
}
