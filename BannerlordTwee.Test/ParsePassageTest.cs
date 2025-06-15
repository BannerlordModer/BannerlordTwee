using BannerlordTwee.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BannerlordTwee.Test {
    [TestClass]
    public class ParsePassageTest {
        [TestMethod]
        public void Test_Parse_Passage_With_Links_And_Comments() {
            var content = "啊啊啊\n\n<!--Comment Text-->\n\n[[选项1->这是选项1]]\n[[选项2->这是选项2]]\n[[选项3]]\n";
            var parser = new TweeParser(string.Empty);
            var passage = parser.ParsePassage(content);

            // 验证内容去除了注释
            Assert.IsFalse(passage.Content.Contains("<!--"));
            Assert.AreEqual(3, passage.Links.Count);

            Assert.AreEqual("选项1", passage.Links[0].ShowString);
            Assert.AreEqual("这是选项1", passage.Links[0].LinkToPassageName);

            Assert.AreEqual("选项2", passage.Links[1].ShowString);
            Assert.AreEqual("这是选项2", passage.Links[1].LinkToPassageName);

            Assert.AreEqual("选项3", passage.Links[2].ShowString);
            Assert.AreEqual("选项3", passage.Links[2].LinkToPassageName);
        }
    }
} 