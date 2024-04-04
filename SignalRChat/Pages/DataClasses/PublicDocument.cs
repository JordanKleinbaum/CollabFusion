namespace SignalRChat.Pages.DataClasses
{
    public class PublicDocument
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public DateTime DateAdded { get; set; }
        public string AnalysisType { get; set; }
    }
}
