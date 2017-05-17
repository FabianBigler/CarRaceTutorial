using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GhostCar
{
    [Serializable()]
    public class GhostCarRecord
    {
        public float TimePast { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
    }
}
