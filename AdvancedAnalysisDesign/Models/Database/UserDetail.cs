using System;
using AdvancedAnalysisDesign.Enums;
using Microsoft.AspNetCore.Identity;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class UserDetail
    {
        public int Id { get; set; }
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public DateTimeOffset DateOfBirth { get; set; }
        [PersonalData]
        public Gender Gender { get; set; }
    }
}