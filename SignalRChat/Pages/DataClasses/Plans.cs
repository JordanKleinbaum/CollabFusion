namespace SignalRChat.Pages.DataClasses
{
    public class Plans
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string PlanContents { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CollabID { get; set; }
    }
}
