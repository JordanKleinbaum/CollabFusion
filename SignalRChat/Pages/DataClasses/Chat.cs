namespace SignalRChat.Pages.DataClasses
{
    public class Chat
    {
        public int ChatID { get; set; }
        public int UserID { get; set; }
        public string? ChatText { get; set; }
        public DateTime? TimeStamp { get; set; }
        public int? CollabID { get; set; }
    }
}
