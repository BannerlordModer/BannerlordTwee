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
                    nameStart = i;
                    continue;
                }
                if (TitleString[i].Equals('[') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    tagStart = i;
                    title.Name = TitleString.Substring(nameStart, i - 1);
                    continue;
                }
                if (TitleString[i].Equals(']') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    var tagEnd = i;
                    var tags = string.Concat(TitleString.ToList().GetRange(tagStart + 1, tagEnd - 1));
                    title.Tags = tags.Split(' ').ToList();
                    continue;
                }
                if (TitleString[i].Equals('{') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    psStart = i;
                    if (tagStart == 0) {
                        title.Name = TitleString.Substring(nameStart, i - 1);
                    }
                    continue;
                }
                if (TitleString[i].Equals('}') && i - 1 > 0 && !TitleString[i - 1].Equals('\\')) {
                    var psEnd = i;
                    var ps = string.Concat(TitleString.ToList().GetRange(psStart, psEnd));
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
        public Story ParseToStory() {
            var story = new Story();
            var tagStart = 0;
            for (int i = 0; i < TweeDataList.Count; i++) {
                if (TweeDataList[i].StartsWith("::")) {
                    tagStart = i;
                    var title = ParseTitle(TweeDataList[i]);

                }
            }
            return story;
        }
    }
}
