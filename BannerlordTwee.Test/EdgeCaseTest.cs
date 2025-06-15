using BannerlordTwee.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace BannerlordTwee.Test {
    [TestClass]
    public class EdgeCaseTest {
        [TestMethod]
        public void Test_Parse_Empty_File() {
            var data = File.ReadAllText("TestFiles/empty.twee");
            var parser = new TweeParser(data);
            var story = parser.ParseToStory();

            Assert.IsNull(story.StoryTitle);
            Assert.IsNull(story.StoryData);
            Assert.AreEqual(0, story.Passages.Count);
        }

        [TestMethod]
        public void Test_Parse_File_With_No_Passages() {
            var data = File.ReadAllText("TestFiles/no_passages.twee");
            var parser = new TweeParser(data);
            var story = parser.ParseToStory();

            Assert.AreEqual("This is a story title.", story.StoryTitle);
            Assert.IsNotNull(story.StoryData);
            Assert.AreEqual("Start Passage", story.StoryData.Start);
            Assert.AreEqual(0, story.Passages.Count);
        }

        [TestMethod]
        public void Test_Parse_File_With_Edge_Cases() {
            var data = File.ReadAllText("TestFiles/edge_cases.twee");
            var parser = new TweeParser(data);
            var story = parser.ParseToStory();

            // Total 6 passages
            Assert.AreEqual(6, story.Passages.Count);

            // Empty Passage
            var emptyPassage = story.Passages.FirstOrDefault(p => p.Title.Name == "Empty Passage");
            Assert.IsNotNull(emptyPassage);
            Assert.IsTrue(string.IsNullOrWhiteSpace(emptyPassage.RawContent));

            // Only Links
            var onlyLinksPassage = story.Passages.FirstOrDefault(p => p.Title.Name == "Only Links");
            Assert.IsNotNull(onlyLinksPassage);
            Assert.AreEqual(2, onlyLinksPassage.Links.Count);
            Assert.AreEqual("Link1", onlyLinksPassage.Links[0].LinkToPassageName);
            Assert.AreEqual("Link2", onlyLinksPassage.Links[1].LinkToPassageName);

            // Special Chars in Title
            var specialCharsPassage = story.Passages.FirstOrDefault(p => p.Title.Name == "Special Chars in Title");
            Assert.IsNotNull(specialCharsPassage);
            CollectionAssert.AreEqual(new[] { "!@#$%^&*()" }, specialCharsPassage.Title.Tags);

            // Malformed Link
            var malformedLinkPassage = story.Passages.FirstOrDefault(p => p.Title.Name == "Malformed Link");
            Assert.IsNotNull(malformedLinkPassage);
            Assert.AreEqual(0, malformedLinkPassage.Links.Count); // Malformed link should be ignored

            // Passage With No Content
            var noContentPassage = story.Passages.FirstOrDefault(p => p.Title.Name == "Passage With No Content");
            Assert.IsNotNull(noContentPassage);
            Assert.IsTrue(string.IsNullOrWhiteSpace(noContentPassage.RawContent));

        }
    }
} 