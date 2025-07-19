namespace IMAP_ContentProcessor_BlazorWebApp.Domain.Models
{
    public class MailContent
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public DateTime MailDateTime { get; set; }
        public List<string> Attachments { get; set; }
        public string MsgID { get; set; }
    }
}
