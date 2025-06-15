using BannerlordTwee.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    var ps = string.Concat(TitleString.ToList().GetRange(psStart + 1, psEnd - psStart - 1));
                    var posMatch = Regex.Match(ps, "\"position\"\\s*:\\s*\"(.*?)\"");
                    if (posMatch.Success) {
                        title.Position = posMatch.Groups[1].Value;
                    }
                    var sizeMatch = Regex.Match(ps, "\"size\"\\s*:\\s*\"(.*?)\"");
                    if (sizeMatch.Success) {
                        title.Size = sizeMatch.Groups[1].Value;
                    }
                    continue;
                }
            }
            if (string.IsNullOrWhiteSpace(title.Name)) {
                title.Name = TitleString.Substring(3);
            }

            // 去除首尾空白及换行符
            if (title.Name is not null) {
                title.Name = title.Name.Trim();
            }
            if (title.Tags != null) {
                for (int idx = 0; idx < title.Tags.Count; idx++) {
                    title.Tags[idx] = title.Tags[idx].Trim();
                }
            }

            return title;
        }
        public Passage ParsePassage(string passage) {
            // 原始内容保留，用于调试或其他用途
            var rawContent = passage;

            // 去除 HTML 注释内容 <!-- ... -->
            var contentWithoutComments = System.Text.RegularExpressions.Regex.Replace(rawContent, "<!--.*?-->", string.Empty, System.Text.RegularExpressions.RegexOptions.Singleline);

            // 提取链接 [[...]]
            var linkMatches = System.Text.RegularExpressions.Regex.Matches(contentWithoutComments, "\\[\\[(.*?)\\]\\]", System.Text.RegularExpressions.RegexOptions.Singleline);
            var links = new List<Link>();
            foreach (System.Text.RegularExpressions.Match match in linkMatches) {
                if (!match.Success) continue;
                var inner = match.Groups[1].Value.Trim();

                string show;
                string target;

                if (inner.Contains("->")) {
                    var arr = inner.Split(new string[] { "->" }, 2, StringSplitOptions.None);
                    show = arr[0];
                    target = arr[1];
                } else if (inner.Contains("|")) {
                    var arr = inner.Split(new char[] { '|' }, 2);
                    show = arr[0];
                    target = arr[1];
                } else if (inner.Contains("<-")) {
                    var arr = inner.Split(new string[] { "<-" }, 2, StringSplitOptions.None);
                    show = arr[1];
                    target = arr[0];
                } else {
                    show = inner;
                    target = inner;
                }

                links.Add(new Link() { ShowString = show, LinkToPassageName = target });
            }

            // 构造 Passage 对象
            var passageObj = new Passage() {
                RawContent = rawContent,
                Content = contentWithoutComments,
                Links = links
            };

            return passageObj;
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
                                story.StoryTitle = content.Replace("\n", "").Replace("\r", "").Trim();
                            } else if (title.IsStoryData) {
                                story.StoryData = ParseStoryData(content);
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
            // 处理文件最后一个段落
            if (title is not null && tagStart < TweeDataList.Count) {
                var content = string.Join("\n", TweeDataList.GetRange(tagStart + 1, TweeDataList.Count - tagStart - 1));
                if (title.IsStoryTitle) {
                    story.StoryTitle = content.Replace("\n", "").Replace("\r", "").Trim();
                } else if (title.IsStoryData) {
                    story.StoryData = ParseStoryData(content);
                } else {
                    var passage = ParsePassage(content);
                    passage.Title = title;
                    story.Passages.Add(passage);
                }
            }
            return story;
        }

        private StoryData ParseStoryData(string content) {
            var storyData = new StoryData();
            content = content.Trim();
            if (content.StartsWith("{") && content.EndsWith("}")) {
                content = content.Substring(1, content.Length - 2);
            }

            var ifidMatch = Regex.Match(content, "\"ifid\"\\s*:\\s*\"(.*?)\"");
            if (ifidMatch.Success) storyData.IfId = ifidMatch.Groups[1].Value;

            var formatMatch = Regex.Match(content, "\"format\"\\s*:\\s*\"(.*?)\"");
            if (formatMatch.Success) storyData.Format = formatMatch.Groups[1].Value;

            var formatVersionMatch = Regex.Match(content, "\"format-version\"\\s*:\\s*\"(.*?)\"");
            if (formatVersionMatch.Success) storyData.FormatVersion = formatVersionMatch.Groups[1].Value;

            var startMatch = Regex.Match(content, "\"start\"\\s*:\\s*\"(.*?)\"");
            if (startMatch.Success) storyData.Start = startMatch.Groups[1].Value;

            var zoomMatch = Regex.Match(content, "\"zoom\"\\s*:\\s*(\\d+)");
            if (zoomMatch.Success && int.TryParse(zoomMatch.Groups[1].Value, out int zoom)) {
                storyData.Zoom = zoom;
            }

            var tagColorsMatch = Regex.Match(content, "\"tag-colors\"\\s*:\\s*\\{(.*?)\\}", RegexOptions.Singleline);
            if (tagColorsMatch.Success) {
                storyData.TagColors = new Dictionary<string, string>();
                var colorsContent = tagColorsMatch.Groups[1].Value;
                var colorMatches = Regex.Matches(colorsContent, "\"(\\w+)\"\\s*:\\s*\"(\\w+)\"");
                foreach (Match match in colorMatches) {
                    storyData.TagColors[match.Groups[1].Value] = match.Groups[2].Value;
                }
            }

            return storyData;
        }
    }
}
