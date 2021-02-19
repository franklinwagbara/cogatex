using System;

namespace GOTEX.Core.BusinessObjects
{
    public class Log
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string Error { get; set; }
        public DateTime Date { get; set; }
    }
}