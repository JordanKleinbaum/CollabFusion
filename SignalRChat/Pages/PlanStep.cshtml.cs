using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace SignalRChat.Pages
{
    public class PlanStepModel : PageModel
    {
        [BindProperty]
        [Required]
        public PlanStep NewPlanStep { get; set; }

        public List<PlanStep> PlanStepList { get; set; } = new List<PlanStep>();
        public List<Plans> PlanList { get; set; } = new List<Plans>();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                SqlDataReader planReader = DBClass.GetAllPlans();
                while (planReader.Read())
                {
                    PlanList.Add(new Plans
                    {
                        PlanID = Convert.ToInt32(planReader["planid"]),
                        PlanName = planReader["PlanName"].ToString()
                    });
                }
                planReader.Close();

                SqlDataReader planStepReader = DBClass.GetAllPlanSteps();
                while (planStepReader.Read())
                {
                    PlanStepList.Add(new PlanStep
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

        /*POPULATE AND CLEAR METHODS*/
        public IActionResult OnPostPopulateButton()
        {
            PlanList.Clear();
            SqlDataReader planReader = DBClass.GetAllPlans();
            while (planReader.Read())
            {
                PlanList.Add(new Plans
                {
                    PlanID = Convert.ToInt32(planReader["planid"]),
                    PlanName = planReader["PlanName"].ToString()
                });
            }
            planReader.Close();
            DBClass.CollabFusionDBConnection.Close();

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                NewPlanStep.PlanID = 1;
                NewPlanStep.StepTitle = "Discuss with team";
                NewPlanStep.StepDescription = "Go into more details";
            }
            return Page();
        }

        /*CLEAR BUTTON*/
        public IActionResult OnPostClearButton()
        {
            if (HttpContext.Request.Method == "POST")
            {
                ModelState.Clear();
                NewPlanStep.PlanID = null;
                NewPlanStep.StepTitle = "";
                NewPlanStep.StepDescription = "";

                // Repopulate UsersList for the form to be able to display the dropdown properly
                PopulatePlanList();
            }
            return Page();
        }

        private void PopulatePlanList()
        {
            PlanList.Clear();
            using (var command = new SqlCommand("SELECT PlanID, PlanName FROM Plans", DBClass.CollabFusionDBConnection))
            {
                DBClass.CollabFusionDBConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PlanList.Add(new Plans
                        {
                            PlanID = reader.GetInt32(reader.GetOrdinal("PlanID")),
                            PlanName = reader.GetString(reader.GetOrdinal("PlanName"))
                        });

                    }
                }
                DBClass.CollabFusionDBConnection.Close();
            }
        }


        public IActionResult OnPost()
        {
            DBClass.InsertPlanStep(NewPlanStep);

            DBClass.CollabFusionDBConnection.Close();

            return RedirectToPage();
        }
    }
}
