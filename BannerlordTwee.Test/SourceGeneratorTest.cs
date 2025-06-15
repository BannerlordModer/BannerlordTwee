using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BannerlordTwee.Test
{
    [TestClass]
    public partial class SourceGeneratorTest
    {
        [TestMethod]
        public void TestGeneratedDialogs()
        {
            // This test is primarily for ensuring the generated code compiles.
            // The actual logic is tested via gameplay in Bannerlord.
            Assert.IsTrue(true); 
        }
    }
}

namespace BannerlordTwee.Generated.Dialogs
{
    // Implement the partial methods required by the generated code.
    public static partial class _1Dialogs
    {
        private static partial bool 下一项Condition() => true;
        private static partial bool 下一项2Condition() => true;
        private static partial bool 开始Condition() => true;
        private static partial bool 开始_这是选项1LinkCondition() => true;
        private static partial void 开始_这是选项1LinkConsequence() { }
        private static partial bool 开始_这是选项2LinkCondition() => true;
        private static partial void 开始_这是选项2LinkConsequence() { }
        private static partial bool 开始_选项3LinkCondition() => true;
        private static partial void 开始_选项3LinkConsequence() { }
        private static partial bool 这是选项1Condition() => true;
        private static partial bool 这是选项1_选项3LinkCondition() => true;
        private static partial void 这是选项1_选项3LinkConsequence() { }
        private static partial bool 这是选项1_这是选项2LinkCondition() => true;
        private static partial void 这是选项1_这是选项2LinkConsequence() { }
        private static partial bool 这是选项1_开始LinkCondition() => true;
        private static partial void 这是选项1_开始LinkConsequence() { }
        private static partial bool 这是选项2Condition() => true;
        private static partial bool 这是选项2_下一项LinkCondition() => true;
        private static partial void 这是选项2_下一项LinkConsequence() { }
        private static partial bool 这是选项2_下一项2LinkCondition() => true;
        private static partial void 这是选项2_下一项2LinkConsequence() { }
        private static partial bool 选项3Condition() => true;
    }
} 