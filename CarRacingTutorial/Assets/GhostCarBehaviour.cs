using Assets;
using Assets.GhostCar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCarBehaviour : MonoBehaviour {
    private List<GhostCarRecord> currentHighscoreRecords;
    private int currentIndex;
    public GameObject wheelFL;
    public GameObject wheelFR;
    public GameObject wheelBL;
    public GameObject wheelBR;    
    public bool startReplay;

    // Use this for initialization
    void Start () {
        SetVisible(false);

        currentHighscoreRecords = GhostCarRecorder.Instance.LoadHighScoreRecords();
        //Debug.Log("highscore records loaded: " + currentHighscoreRecords.Count.ToString());
    }

    public void SetVisible(bool isVisible)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = isVisible;
        }
    }

	
	// Update is called once per frame
	void FixedUpdate () {
        if(startReplay)
        {
            if (currentHighscoreRecords != null && currentHighscoreRecords.Count > currentIndex)
            {
                var record = currentHighscoreRecords[currentIndex];
                var buggyPosition = new Vector3(record.BuggyPosition.x, record.BuggyPosition.y, record.BuggyPosition.z);
                var buggyRotation = new Quaternion(record.BuggyRotation.x, record.BuggyRotation.y, record.BuggyRotation.z, record.BuggyRotation.w);                
                var wheelFLRotation = new Quaternion(record.WheelFrontLeftRotation.x, record.WheelFrontLeftRotation.y, record.WheelFrontLeftRotation.z, record.WheelFrontLeftRotation.w);
                var wheelFRRotation = new Quaternion(record.WheelFrontRightRotation.x, record.WheelFrontRightRotation.y, record.WheelFrontRightRotation.z, record.WheelFrontRightRotation.w);
                var wheelBLRotation = new Quaternion(record.WheelBackLeftRotation.x, record.WheelBackLeftRotation.y, record.WheelBackLeftRotation.z, record.WheelBackLeftRotation.w);
                var wheelBRRotation = new Quaternion(record.WheelBackRightRotation.x, record.WheelBackRightRotation.y, record.WheelBackRightRotation.z, record.WheelBackRightRotation.w);

                transform.position = buggyPosition;
                transform.rotation = buggyRotation;
                wheelBL.transform.rotation = wheelBLRotation;
                wheelFR.transform.rotation = wheelFRRotation;
                wheelFL.transform.rotation = wheelFLRotation;
                wheelBR.transform.rotation = wheelBRRotation;

                currentIndex++;
            } else
            {
                SetVisible(false);
            }
        }         
    }
}
