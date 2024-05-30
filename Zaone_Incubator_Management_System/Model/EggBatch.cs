using System;
using System.Collections.Generic;
using System.Text;

namespace Zaone_Incubator_Management_System.Model
{
    public class EggBatch
    {
        public int EggBatchID { get; set; }
        public int IncubationDays { get; set; }
        public int NumberofEggs { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime HatchDate { get; set; }
        public int IncubatorID { get; set; }
        public int Status { get; set; }
    }
}
