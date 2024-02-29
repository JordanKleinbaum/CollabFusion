using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class AllStepsModel : PageModel
    {
        [BindProperty]
        [Required]
        public PlanStep NewPlanStep { get; set; }

        public List<PlanStep> PlanStepList { get; set; } = new List<PlanStep>();
        public List<PlanStep> MatchingPlanStepList { get; set; } = new List<PlanStep>();
        public List<Plans> PlanList { get; set; } = new List<Plans>();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                SqlDataReader planReader = DBClass.GeneralReaderQuery($"SELECT * FROM Plans WHERE CollabID = {HttpContext.Session.GetInt32("collabid")}");
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


                SqlDataReader planStepReader = DBClass.GeneralReaderQuery($"SELECT * FROM PlanStep AS ps INNER JOIN Plans AS p ON ps.PlanID = p.PlanID WHERE CollabID = {HttpContext.Session.GetInt32("collabid")}");
                while (planStepReader.Read())
                {
                    MatchingPlanStepList.Add(new PlanStep
                    {
                        PlanStepID = Convert.ToInt32(planStepReader["PlanStepID"]),
                        StepTitle = planStepReader["StepTitle"].ToString(),
                        StepDescription = planStepReader["StepDescription"].ToString(),
                        PlanID = Convert.ToInt32(planStepReader["planid"]),
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
    }
}
