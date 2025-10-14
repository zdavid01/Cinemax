namespace Email.API.Models
{
    public class EmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? From { get; set; }
        public bool IsHtml { get; set; } = true;
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public Dictionary<string, string> Attachments { get; set; } = new Dictionary<string, string>();
        public int Priority { get; set; } = 1; // 1 = Normal, 2 = High, 0 = Low
    }
}


