using BannerlordTwee.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace BannerlordTwee.Test {
    [TestClass]
    public class ParseStoryTest {
        [TestMethod]
        public void Test_Parse_Story_From_File() {
            var data = File.ReadAllText("TestFiles/1.twee");
            var parser = new TweeParser(data);
            var story = parser.ParseToStory();

            // StoryTitle
            Assert.AreEqual("1", story.StoryTitle);

            // StoryData basic
            Assert.IsNotNull(story.StoryData);
            Assert.AreEqual("Chapbook", story.StoryData.Format);
            Assert.AreEqual("1.2.3", story.StoryData.FormatVersion);
            Assert.AreEqual("开始", story.StoryData.Start);
            Assert.AreEqual(1, story.StoryData.Zoom);

            // passages count (file中共有 6 个普通 passage)
            Assert.AreEqual(6, story.Passages.Count);

            // 验证第一个 passage 链接是否被正确解析
            var startPassage = story.Passages.First(p => p.Title.Name == "开始");
            Assert.IsTrue(startPassage.Links.Any());
            Assert.AreEqual(3, startPassage.Links.Count);
        }
    }
} 