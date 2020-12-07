using System;
using AdvancedAnalysisDesign.Enums;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class UserDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}