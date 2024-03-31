namespace SignalRChat.Pages.DataClasses
{
    public class PreviousSpendingAnalysis
    {
        public int SpendingAnalysisID { get; set; }
        public string SpendingAnalysisName { get; set; }
        public string? SpendingAnalysisDescription { get; set; }
        public string? BasedOffOf { get; set; }
        public DateTime? SpendingAnalysisDate { get; set; }
    }
}
