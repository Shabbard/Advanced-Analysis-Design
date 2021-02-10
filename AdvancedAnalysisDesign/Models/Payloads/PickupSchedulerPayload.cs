using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class PickupSchedulerPayload
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string CategoryColor { get; set; }
        public bool IsPickedUp { get; set; }
        public bool IsPrepared { get; set; }
    }
}
