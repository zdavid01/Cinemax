using System;
using System.Collections.Generic;

namespace EventBus.Messages.Events
{
    public class SendEmailEvent : IntegrationBaseEvent
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public bool IsHtml { get; set; } = true;
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public Dictionary<string, string> Attachments { get; set; } = new Dictionary<string, string>();
        public int Priority { get; set; } = 1; // 1 = Normal, 2 = High, 0 = Low
    }
}

