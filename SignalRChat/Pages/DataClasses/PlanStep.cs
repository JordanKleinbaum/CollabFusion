namespace SignalRChat.Pages.DataClasses
{
    public class PlanStep
    {
        public int PlanStepID { get; set; }
        public string StepTitle { get; set; }
        public string StepDescription { get; set; }
        public int? PlanID { get; set; }
    }
}
