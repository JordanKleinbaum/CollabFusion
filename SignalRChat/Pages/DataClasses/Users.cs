namespace SignalRChat.Pages.DataClasses
{
    public class Users
    {
        public int UserID { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public String Street { get; set; }
        public String City { get; set; }
        public String? State { get; set; }
        public String Country { get; set; }
        public String? ZipCode { get; set; }

        public String Admin { get; set; }
    }
}
