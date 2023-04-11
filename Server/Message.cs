using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Message
    {
        private User author;
        private string subject;
        private string content;
        private DateTime? createdAt;

        public Message(User author, string subject, string content) 
        {
            this.author = author;
            this.subject = subject;
            this.content = content;
            createdAt = DateTime.Now;
        }
    }
}
