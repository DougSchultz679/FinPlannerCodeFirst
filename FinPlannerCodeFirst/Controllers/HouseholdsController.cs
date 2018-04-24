using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinPlannerCodeFirst.Models;
using FinPlannerCodeFirst.Models.Helpers;
using FinPlannerCodeFirst.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace FinPlannerCodeFirst.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        [AuthorizeHouseholdRequired]
        public ActionResult CreateJoinHousehold(Guid? code)
        {
            //If the current user accessing this page already has a HouseholdId, send them to their dashboard
            if (User.Identity.IsInHousehold())
            {
                return RedirectToAction("Details", new { id = User.Identity.GetHouseholdId() });
            }

            HouseholdViewModel vm = new HouseholdViewModel();

            //Determine whether the user has been sent an invite and set property values 
            if (code != null)
            {
                string msg = "";
                if (ValidInvite(code, ref msg))
                {
                    Invite result = db.Invites.FirstOrDefault(i => i.HHToken == code);

                    vm.IsJoinHouse = true;
                    vm.HHId = result.HouseholdId;
                    vm.HHName = result.Household.Name;

                    result.HasBeenUsed = true;

                    // FIX THIS - why is this done 
                    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                    user.InviteEmail = result.Email;
                    db.SaveChanges();
                } else
                {
                    //FIX THIS - does not exist
                    return RedirectToAction("InviteError", new { errMsg = msg });
                }
            } else
            {
                vm.IsJoinHouse = false;
                return View(vm);
            }
            //fix this - needs route param for household details
            return RedirectToAction("Details", new { id = vm.HHId });
        }

        private bool ValidInvite(Guid? code, ref string message)
        {

            if ((DateTime.Now - db.Invites.FirstOrDefault(i => i.HHToken == code)
                .InviteDate).TotalDays < 6)
            {
                bool result = db.Invites.FirstOrDefault(i => i.HHToken == code).HasBeenUsed;
                if (result)
                {
                    message = "invalid";
                } else
                {
                    message = "valid";
                }
                return !result;
            } else
            {
                message = "expired";
                return false;
            }
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            //CHECK THIS - that household IDs never equal to 0 due to database scheme
            if (id == 0 || id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hh = db.Households.Find(id);

            HouseholdViewModel hhvm = new HouseholdViewModel {
                HHObj = hh,
                HHId = (int)id,
                HHName = hh.Name,
                IsJoinHouse = false,
                HHInvites = db.Invites.Where(i => i.HouseholdId == id && i.HasBeenUsed == false).ToList()
            };

            return View(hhvm);
        }

        //POST: Households/LeaveHousehold
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LeaveHousehold(int hhId, string uId)
        //{
        //    var helper = new HouseholdHelper();
        //    helper.RemoveUserFromHousehold(uId, hhId);
        //}

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(string HHName)
        {
            Household hh = new Household {
                Id = 0,
                Name = HHName
            };
            HouseholdHelper helper = new HouseholdHelper();
            helper.AddUserToHousehold(User.Identity.GetUserId(), hh.Id);

            db.Households.Add(hh);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = hh.Id });
        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult JoinHousehold(int HHId)
        {
            HouseholdHelper helper = new HouseholdHelper();
            helper.AddUserToHousehold(User.Identity.GetUserId(), HHId);

            return RedirectToAction("Details", new { id = HHId });
        }


        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
