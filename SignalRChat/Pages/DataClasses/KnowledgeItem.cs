namespace SignalRChat.Pages.DataClasses
{
    public class KnowledgeItem
    {
        public int KnowledgeID { get; set; }
        public string KnowledgeTitle { get; set; }
        public string KnowledgeSubject { get; set; }
        public string Category { get; set; }
        public string? Information { get; set; }
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string Opportunities { get; set; }
        public string Threats { get; set; }
        public DateTime? KNDate { get; set; }

        // Note: Nullable for testing purposes, change this later
        public int? UserID { get; set; }
        public int? CollabID { get; set; }
        public int? KnowledgeLibID { get; set; }
    }
}