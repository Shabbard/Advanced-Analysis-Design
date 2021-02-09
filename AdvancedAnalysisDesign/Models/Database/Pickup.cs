using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class Pickup
    {
        public int Id { get; set; }
        public bool IsPickedUp { get; set; }
        public bool IsPrepared { get; set; }
        public DateTimeOffset DatePickedUp { get; set; }
    }
}
