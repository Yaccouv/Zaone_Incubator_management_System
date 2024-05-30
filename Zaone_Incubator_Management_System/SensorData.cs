using System;
using System.Collections.Generic;
using System.Text;

namespace Zaone_Incubator_Management_System.Model
{
    public class SensorData
    {
        public int SensorDataId { get; set; }
        public double TemperatureValue { get; set; }
        public double HumidityValue { get; set; }
        public int IncubatorID { get; set; }
        public int EggRotationTimer { get; set; }
    }

}
