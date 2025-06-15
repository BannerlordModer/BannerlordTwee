using System.Collections.Generic;

namespace BannerlordTwee.SourceGenerator.Models
{
    public class Story
    {
        public Story()
        {
            Passages = new List<Passage>();
        }
        public string StoryTitle { get; set; }
        public StoryData StoryData { get; set; }
        public List<Passage> Passages { get; set; }

    }
} 