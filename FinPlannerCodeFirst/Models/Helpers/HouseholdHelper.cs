using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPlannerCodeFirst.Models.Helpers
{
    public class HouseholdHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public Exception AddUserToHousehold(string uid, int hhId)
        {
            try
            {
                ApplicationUser user = db.Users.Find(uid);
                if (!user.HouseholdId.HasValue)
                {
                    user.HouseholdId = hhId;
                    db.SaveChanges();
                }
                return null;
            } catch (Exception ex)
            {
                return ex;
            }
        }



    }
}