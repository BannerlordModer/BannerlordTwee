using System;
using System.Collections.Generic;
using System.Text;

namespace BannerlordTwee.Models {
    public class Passage {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Size { get; set; }
        public List<string> Tags { get; set; }
        public string RawHeader { get; set; }
        public string RawContent { get; set; }
        public string Content { get; set; }
        public List<Link> Links { get; set; }
    }
}
