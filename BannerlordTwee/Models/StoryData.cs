using System.Collections.Generic;
using System.Text;

namespace BannerlordTwee.Models {
    public class StoryData {
        public string IfId { get; set; }
        public string Format { get; set; }
        public string FormatVersion { get; set; }
        public string Start { get; set; }
        public Dictionary<string, string> TagColors { get; set; }
        public int Zoom { get; set; }
    }
}
