using System;

namespace AdvancedAnalysisDesign.Models.ViewModels
{
    public class PickupSchedulerViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CategoryColor { get; set; }
        public bool IsPickedUp { get; set; }
        public bool IsPrepared { get; set; }
    }
}