using BannerlordTwee.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BannerlordTwee.Parser {
    public class TweeParser {
        private readonly string TweeData;
        private readonly List<string> TweeDataList;
        public TweeParser(string TweeData) {
            this.TweeData = TweeData;
            TweeDataList = TweeData.Split('\n').ToList();
        }
        public Title ParseTitle(string TitleString) {
            var title = new Title();
            var TitleList = new List<string>();
            var nameStart = 0;
            var tagStart = 0;
            var psStart = 0;
            for (int i = 0; i < TitleString.Length; i++) {
                if (TitleString[i].Equals(':') && 
                    i + 1 < TitleString.Length && 
                    TitleString[i + 1].Equals(':') && 
                    i + 2 <TitleString.Length && 
                    TitleString[i + 2].Equals(' ')) {
                    i += 2;
                    nameStart = i + 1;
                    continue;
                }
                if (TitleString[i].Equals('[') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    tagStart = i;
                    title.Name = TitleString.Substring(nameStart, i - 1 - nameStart);
                    continue;
                }
                if (TitleString[i].Equals(']') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    var tagEnd = i;
                    var tags = string.Concat(TitleString.ToList().GetRange(tagStart + 1, tagEnd - tagStart - 1));
                    title.Tags = tags.Split(' ').ToList();
                    continue;
                }
                if (TitleString[i].Equals('{') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    psStart = i;
                    if (tagStart == 0) {
                        title.Name = TitleString.Substring(nameStart, i - 1 - nameStart);
                    }
                    continue;
                }
                if (TitleString[i].Equals('}') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    var psEnd = i;
                    var ps = string.Concat(TitleString.ToList().GetRange(psStart, psEnd - psStart + 1));
                    var psDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(ps);
                    if (psDict.TryGetValue("position", out var pos)) {
                        title.Position = pos;
                    }
                    if (psDict.TryGetValue("size", out var size)) {
                        title.Size = size;
                    }
                    continue;
                }
            }
            if (string.IsNullOrWhiteSpace(title.Name)) {
                title.Name = TitleString.Substring(3);
            }

            return title;
        }
        public Passage ParsePassage(string passage) {
            return new Passage();
        }
        public List<Paragraph> ParseParagraphs(List<string> DataList) {
            var paragraphs = new List<Paragraph>();
            var tagStart = 0;
            string title = null;
            for (int i = 0; i < DataList.Count; i++) {
                if (DataList[i].StartsWith("::")) {
                    if (tagStart < i) {
                        var content = string.Join("\n", DataList.GetRange(tagStart + 1, i - tagStart - 1));
                        paragraphs.Add(new Paragraph() { Title = title, Content = content });
                    }
                    tagStart = i;
                    title = DataList[i];
                }
            }
            paragraphs.Add(
                new Paragraph() { 
                    Title = title, 
                    Content = string.Join(
                        "\n", 
                        DataList.GetRange(
                            tagStart + 1, 
                            DataList.Count - tagStart - 1
                            )
                        ) 
                });
            return paragraphs;
        }
        public Story ParseToStory() {
            var story = new Story();
            var tagStart = 0;
            Title title = null;
            for (int i = 0; i < TweeDataList.Count; i++) {
                if (TweeDataList[i].StartsWith("::")) {
                    if (tagStart < i) {
                        var content = string.Join("\n", TweeDataList.GetRange(tagStart + 1, i - tagStart - 1));
                        if (title is not null) {
                            if (title.IsStoryTitle) {
                                story.StoryTitle = content.Replace("\n", "");
                            } else if (title.IsStoryData) {
                                story.StoryData = JsonConvert.DeserializeObject<StoryData>(content);
                            } else {
                                var passage = ParsePassage(content);
                                passage.Title = title;
                                story.Passages.Add(passage);
                            }
                            title = null;
                        }

                    }
                    title = ParseTitle(TweeDataList[i]);
                    tagStart = i;
                }
            }
            return story;
        }
    }
}
