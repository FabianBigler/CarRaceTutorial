using Assets.GhostCar;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets
{
    public class GhostCarRecorder
    {
        private static readonly GhostCarRecorder instance = new GhostCarRecorder();

        public float HighscoreTime { get; private set; }

        private List<GhostCarRecord> currentRecords;        
        private string highscorePath;

        //// Explicit static constructor to tell C# compiler
        //// not to mark type as beforefieldinit
        //static GhostCarRecorder()
        //{
            
        //}

        private GhostCarRecorder()
        {
            currentRecords = new List<GhostCarRecord>();
            //C:\Users\FabianBigler\AppData\LocalLow\DefaultCompany\CarRacingTutorial
            highscorePath = Path.Combine(Application.persistentDataPath, "ghostcar.dat");           
            HighscoreTime = PlayerPrefs.GetFloat("HighscoreTime");
            Debug.Log(HighscoreTime);
        }

        public static GhostCarRecorder Instance
        {
            get
            {
                return instance;
            }
        }

        public void Record(float pastTime, Vector3 buggyPosition, Quaternion buggyRotation,
            Quaternion wheelFrontLeftRotation, Quaternion wheelFrontRightRotation, 
            Quaternion wheelBackLeftRotation, Quaternion wheelBackRightRotation)
        {            
            var record = new Assets.GhostCar.GhostCarRecord()
            {
                PastTime = pastTime,
                BuggyPosition = new SerializableVector3
                {
                    x = buggyPosition.x,
                    y = buggyPosition.y,
                    z = buggyPosition.z
                },
                BuggyRotation = new SerializableQuaternion
                {                    
                    x = buggyRotation.x,
                    y = buggyRotation.y,
                    z = buggyRotation.z,
                    w = buggyRotation.w
                },
                WheelFrontLeftRotation = new SerializableQuaternion
                {
                    x = wheelFrontLeftRotation.x,
                    y = wheelFrontLeftRotation.y,
                    z = wheelFrontLeftRotation.z,
                    w = wheelFrontLeftRotation.w
                },
                WheelFrontRightRotation = new SerializableQuaternion
                {
                    x = wheelFrontRightRotation.x,
                    y = wheelFrontRightRotation.y,
                    z = wheelFrontRightRotation.z,
                    w = wheelFrontRightRotation.w
                },
                WheelBackLeftRotation = new SerializableQuaternion
                {
                    x = wheelBackLeftRotation.x,
                    y = wheelBackLeftRotation.y,
                    z = wheelBackLeftRotation.z,
                    w = wheelBackLeftRotation.w
                },
                WheelBackRightRotation = new SerializableQuaternion
                {
                    x = wheelBackRightRotation.x,
                    y = wheelBackRightRotation.y,
                    z = wheelBackRightRotation.z,
                    w = wheelBackRightRotation.w
                }
            };

            if (record.PastTime < HighscoreTime || HighscoreTime == 0)
            {                
                currentRecords.Add(record);
            }                        
        }

        public void SaveHighscore(float newTime)
        {            
            if(newTime < HighscoreTime || HighscoreTime == 0f)
            {
                PlayerPrefs.SetFloat("HighscoreTime", newTime);
                //HighscoreTime = newTime;
                Serializer.Save(highscorePath, currentRecords);
            }                   
        }


        public List<GhostCarRecord> LoadHighScoreRecords()
        {
            if (File.Exists(highscorePath))
            {
                return Serializer.Load<List<GhostCarRecord>>(highscorePath);
            } else
            {
                PlayerPrefs.SetFloat("HighscoreTime", 0);
                HighscoreTime = 0;
                return null;
            }           
        }
    }
}
