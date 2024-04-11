namespace SignalRChat.Pages.DataClasses
{
    public class PublicPreviousSpendingAnalysis
    {
        public int PublicSpendingAnalysisID { get; set; }
        public int SpendingAnalysisID { get; set; }
        public string SpendingAnalysisName { get; set; }
        public string? SpendingAnalysisDescription { get; set; }
        public string? BasedOffOf { get; set; }
        public DateTime SpendingAnalysisDate { get; set; }
        public int Column1 { get; set; }
        public int Column2 { get; set; }
    }
}
