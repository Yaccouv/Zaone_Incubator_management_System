using System;
using System.Collections.Generic;
using System.Text;

namespace Zaone_Incubator_Management_System.Model
{
    public class Hatchery
    {
        public int HatcheryID { get; set; }
        public int EggBatchID { get; set; }
        public int HatchedEggs { get; set; }
        public int UnhatchedEggs { get; set; }
        public int IncubatorID { get; set; }
    }
}
