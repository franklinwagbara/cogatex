using System;
using System.Runtime.Serialization;

namespace GOTEX.ViewModels
{
    public class ChartModel
    {
        public string Date { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Processing { get; set; }
    }
}