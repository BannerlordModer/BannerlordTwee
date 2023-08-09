using System;
using System.Collections.Generic;
using System.Text;

namespace BannerlordTwee.Models {
    public class Title {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Size { get; set; }
        public List<string> Tags { get; set; }
        public bool OtherIsEmpty { get => string.IsNullOrWhiteSpace(Position) && string.IsNullOrWhiteSpace(Size) && (Tags?.Count == 0 || Tags is null); }
        public bool IsStoryData { get => Name.Equals("StoryData") && OtherIsEmpty; }
        public bool IsStoryTitle { get => Name.Equals("StoryTitle") && OtherIsEmpty; }
    }
}
