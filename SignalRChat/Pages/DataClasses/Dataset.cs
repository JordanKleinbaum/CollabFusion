namespace SignalRChat.Pages.DataClasses
{
    public class Dataset
    {
        public int DatasetID { get; set; }
        public string? DatasetDetails { get; set; }
        public DateTime UploadDate { get; set; }
        public int UserID { get; set; }
        public int AnalysisID { get; set; }
    }
}
