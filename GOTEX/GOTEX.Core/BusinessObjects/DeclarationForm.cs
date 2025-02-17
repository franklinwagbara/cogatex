﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GOTEX.Core.BusinessObjects
{
    public class DeclarationForm
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuarterId { get; set; }
        public int Year { get; set; }
        public decimal ExportVolume { get; set; }
        public int CrudeTheft { get; set; }
        public int ExportProceedings { get; set; }
        public int Bribe { get; set; }
        public string StaffBribe { get; set; }
        public int OutstandingFee { get; set; }
        public int Offence { get; set; }
        public int Violation { get; set; }
        [ForeignKey(nameof(QuarterId))]
        public Quarter Quarter { get; set; }
    }
}
