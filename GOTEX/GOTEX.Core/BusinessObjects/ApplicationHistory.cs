﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTEX.Core.BusinessObjects
{
    public class ApplicationHistory
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime DateAssigned { get; set; }
        public int WorkFlowId { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public bool IsAssigned { get; set; }
        public string Action { get; set; }
        public string CurrentUserRole { get; set; }
        public string CurrentUser { get; set; }
        public string ProcessingUserRole { get; set; }
        public string ProcessingUser { get; set; }
        public bool AutoPushed { get; set; }
        public DateTime? DateTreated { get; set; }
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }
    }
}