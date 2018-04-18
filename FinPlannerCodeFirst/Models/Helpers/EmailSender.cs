using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FinPlannerCodeFirst.Models.Helpers
{
    public class EmailSender
    {
        public async Task<ActionResult> SendInviteNoti(string senderName, string callbackUrl, string targetName, string targetEmail, Guid token)
        {
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = new ApplicationUser();

                msg.Subject = "Financial Planner: You've been invited to join by " + senderName + "!";
                msg.Destination = targetEmail;
                msg.Body =
                    "Hey" + targetName + "!" + Environment.NewLine +
                    "You have been invited to join a household by " + senderName + "!" + Environment.NewLine + "Please click the following link to sign up and review your household budget: " + "<a href=\"" + callbackUrl + "\">Join Household Now!</a>";

                await ems.SendMailAsync(msg);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
            return new EmptyResult();
        }
    }
}