using BannerlordTwee.Models;
using BannerlordTwee.Parser;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordTwee.Test {
    [TestClass]
    public class ParseParagraphTest {
        private List<Paragraph> TitleParagraphs { get; set; }

        private List<Paragraph> DataParagraphs { get; set; }
        private List<Paragraph> PassageParagraphs { get; set; }
        private Random random { get; set; }
        private Paragraph GetParagraph(Paragraph paragraph) {
            var title = Encoding.UTF8.GetString(Convert.FromBase64String(paragraph.Title));
            var content = Encoding.UTF8.GetString(Convert.FromBase64String(paragraph.Content));
            return new Paragraph() { Title = title, Content = content };
        }
        public ParseParagraphTest() {
            random = new Random();
            TitleParagraphs = new List<Paragraph>();
            DataParagraphs = new List<Paragraph>();
            PassageParagraphs = new List<Paragraph>();
            var title = JsonConvert.DeserializeObject<List<Paragraph>>(File.ReadAllText("TestFiles/TestParagraph/title.json"));
            foreach(var paragraph in title) {
                TitleParagraphs.Add(GetParagraph(paragraph));
            }
            var data = JsonConvert.DeserializeObject<List<Paragraph>>(File.ReadAllText("TestFiles/TestParagraph/data.json"));
            foreach(var paragraph in data) {
                DataParagraphs.Add(GetParagraph(paragraph));
            }
            var passage = JsonConvert.DeserializeObject<List<Paragraph>>(File.ReadAllText("TestFiles/TestParagraph/passage.json"));
            foreach (var paragraph in passage) {
                PassageParagraphs.Add(GetParagraph(paragraph));
            }
        }
        private List<Paragraph> ConstructParagraphs(int count) {
            var paragraphs = new List<Paragraph> {
                TitleParagraphs[random.Next(0, TitleParagraphs.Count)],
                DataParagraphs[random.Next(0, DataParagraphs.Count)]
            };
            for (int i = 0; i < count; i++) {
                paragraphs.Add(PassageParagraphs[random.Next(0, PassageParagraphs.Count)]);
            }
            return paragraphs;
        }
        private List<string> GenerateParagraphs(List<Paragraph> paragraphs) {
            var list = new List<string>();
            foreach (var paragraph in paragraphs) {
                list.Add(paragraph.Title);
                list.AddRange(paragraph.Content.Split('\n'));
            }
            return list;
        }
        [DataTestMethod]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1000)]
        public void Test_Parse_Paragraph(int count) {
            var parser = new TweeParser("");
            var paragraphs = ConstructParagraphs(count);
            var list = GenerateParagraphs(paragraphs);
            var ParsedParagraph = parser.ParseParagraphs(list);
            Assert.AreEqual(paragraphs.Count, ParsedParagraph.Count);
            for (int i = 0; i < paragraphs.Count; i++) {
                Assert.AreEqual(paragraphs[i].Title, ParsedParagraph[i].Title);
                Assert.AreEqual(paragraphs[i].Content, ParsedParagraph[i].Content);
            }
        }
        [TestMethod]
        public void Test_Parse_Paragraph_1000_1000() {
            for (int i = 0; i < 1000; i++) {
                Test_Parse_Paragraph(1000);
            }
        }
    }
}
