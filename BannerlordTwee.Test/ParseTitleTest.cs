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
        public void TestNormal_1() {
            var parser = new TweeParser("");
            var title = parser.ParseTitle(":: 下一项 {\"position\":\"900,625\",\"size\":\"100,100\"}");
            Assert.AreEqual(title.Name, "下一项");
            Assert.AreEqual(title.OtherIsEmpty, false);
            Assert.AreEqual(title.IsStoryTitle, false);
            Assert.AreEqual(title.IsStoryData, false);
            Assert.AreEqual(title.Size, "100,100");
            Assert.AreEqual(title.Tags, null);
        }
    }
}