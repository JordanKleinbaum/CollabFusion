using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class CreatePlanModel : PageModel
    {
        [BindProperty]
        [Required]
        public Plans NewPlan { get; set; }

        public List<Plans> PlanList { get; set; } = new List<Plans>();

        /*POPULATE AND CLEAR METHODS*/
        public IActionResult OnPostPopulateButton()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                NewPlan.PlanName = "Plan 1";
                NewPlan.PlanContents = "This is our plan for Plan 1";
                NewPlan.DateCreated = DateTime.Now;
            }
            return Page();
        }

        /*CLEAR BUTTON*/
        public IActionResult OnPostClearButton()
        {
            if (HttpContext.Request.Method == "POST")
            {
                ModelState.Clear();
                NewPlan.PlanName = "";
                NewPlan.PlanContents = "";
                NewPlan.DateCreated = null;
            }
            return Page();
        }


        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                SqlDataReader planReader = DBClass.GetAllPlans();
                while (planReader.Read())
                {
                    PlanList.Add(new Plans
                    {
                        PlanID = Convert.ToInt32(planReader["PlanID"]),
                        PlanName = planReader["PlanName"].ToString(),
                        PlanContents = planReader["PlanContents"].ToString(),
                        DateCreated = planReader["DateCreated"] as DateTime?,
                        CollabID = HttpContext.Session.GetInt32("collabid")
                    });
                }

                // Close your connection in DBClass
                DBClass.CollabFusionDBConnection.Close();
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("Index");
            }
        }

        public IActionResult OnPost()
        {
            NewPlan.CollabID = HttpContext.Session.GetInt32("collabid");

            DBClass.InsertPlan(NewPlan);

            DBClass.CollabFusionDBConnection.Close();

            return RedirectToPage("EnteredCollaboration", new { collaborationId = NewPlan.CollabID });
        }

    }
}

