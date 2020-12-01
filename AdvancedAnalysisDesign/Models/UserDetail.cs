using System;

namespace AdvancedAnalysisDesign.Models
{
    public class UserDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}