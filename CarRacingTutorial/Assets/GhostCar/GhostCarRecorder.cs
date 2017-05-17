using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class GhostCarRecorder
    {
        public void Record()
        {

        }

        public void Save()
        {
            var highScore = PlayerPrefs.GetFloat("Highscore");

           //PlayerPrefs.SetFloat("Highscore");


        }
        
        
    }
}
