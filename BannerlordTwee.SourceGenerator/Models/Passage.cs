using System.Collections.Generic;

namespace BannerlordTwee.SourceGenerator.Models
{
    public class Passage
    {
        public Title Title { get; set; }
        public string RawContent { get; set; }
        public string Content { get; set; }
        public List<Link> Links { get; set; }

        public Passage()
        {
            Content = string.Empty;
            RawContent = string.Empty;
            Links = new List<Link>();
        }
    }
} 