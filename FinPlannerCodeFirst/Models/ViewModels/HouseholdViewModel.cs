using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPlannerCodeFirst.Models.ViewModels
{
    public class HouseholdViewModel
    {
        public string HHName { get; set; }
        public int HHId { get; set; }
        public bool IsJoinHouse { get; set; }
        public Household HHObj { get; set; }
        public List<ApplicationUser> HHUsers {get;set;}
    }
}