using BannerlordTwee.Parser;

namespace BannerlordTwee.Test {
    [TestClass]
    public class ParseTitleTest {
        [TestMethod]
        public void TestStoryTitle() {
            var parser = new TweeParser("");
            var title = parser.ParseTitle(":: StoryTitle");
            Assert.AreEqual(title.Name, "StoryTitle");
            Assert.AreEqual(title.OtherIsEmpty, true);
            Assert.AreEqual(title.IsStoryTitle, true);
            Assert.AreEqual(title.IsStoryData, false);
        }
        [TestMethod]
        public void TestStoryData() {
            var parser = new TweeParser("");
            var title = parser.ParseTitle(":: StoryData");
            Assert.AreEqual(title.Name, "StoryData");
            Assert.AreEqual(title.OtherIsEmpty, true);
            Assert.AreEqual(title.IsStoryTitle, false);
            Assert.AreEqual(title.IsStoryData, true);
        }
        [TestMethod]
        public void TestNormal_Without_Tag_1() {
            var parser = new TweeParser("");
            var title = parser.ParseTitle(":: 下一项 {\"position\":\"900,625\",\"size\":\"100,100\"}");
            Assert.AreEqual(title.Name, "下一项");
            Assert.AreEqual(title.OtherIsEmpty, false);
            Assert.AreEqual(title.IsStoryTitle, false);
            Assert.AreEqual(title.IsStoryData, false);
            Assert.AreEqual(title.Size, "100,100");
            Assert.AreEqual(title.Position, "900,625");
            Assert.AreEqual(title.Tags, null);
        }
        [TestMethod]
        public void TestNormal_With_StoryData_Title() {
            var parser = new TweeParser("");
            var title = parser.ParseTitle(":: StoryData {\"position\":\"1050,575\",\"size\":\"100,100\"}");
            Assert.AreEqual(title.Name, "StoryData");
            Assert.AreEqual(title.OtherIsEmpty, false);
            Assert.AreEqual(title.IsStoryTitle, false);
            Assert.AreEqual(title.IsStoryData, false);
            Assert.AreEqual(title.Size, "100,100");
            Assert.AreEqual(title.Position, "1050,575");
            Assert.AreEqual(title.Tags, null);
        }
        [TestMethod]
        public void TestNormal_With_Tag_1() {
            var parser = new TweeParser("");
            var title = parser.ParseTitle(":: 这是选项1 [tag1 tag2 taga] {\"position\":\"950,125\",\"size\":\"100,100\"}");
            Assert.AreEqual(title.Name, "这是选项1");
            Assert.AreEqual(title.OtherIsEmpty, false);
            Assert.AreEqual(title.IsStoryTitle, false);
            Assert.AreEqual(title.IsStoryData, false);
            Assert.AreEqual(title.Size, "100,100");
            Assert.AreEqual(title.Position, "950,125");
            CollectionAssert.AreEqual(title.Tags, new List<string>() { "tag1", "tag2", "taga" });
        }
        [TestMethod]
        public void TestNormal_With_Tag_2() {
            var parser = new TweeParser("");
            var title = parser.ParseTitle(":: 选项 3 [tagme \\[a\\] \\{b\\} aa---a] {\"position\":\"1050,325\",\"size\":\"100,100\"}");
            Assert.AreEqual(title.Name, "选项 3");
            Assert.AreEqual(title.OtherIsEmpty, false);
            Assert.AreEqual(title.IsStoryTitle, false);
            Assert.AreEqual(title.IsStoryData, false);
            Assert.AreEqual(title.Size, "100,100");
            Assert.AreEqual(title.Position, "1050,325");
            CollectionAssert.AreEqual(title.Tags, new List<string>() { "tagme", "\\[a\\]", "\\{b\\}", "aa---a" });
        }
    }
}