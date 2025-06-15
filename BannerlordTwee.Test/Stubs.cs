// Stub classes to make the generated code compile without real game assemblies.
namespace TaleWorlds.CampaignSystem
{
    public class CampaignGameStarter {
        public void AddDialogLine(string id, string inputToken, string outputToken, string text, System.Func<bool> condition, System.Action consequence) { }
        public void AddPlayerLine(string id, string inputToken, string outputToken, string text, System.Func<bool> condition, System.Action consequence) { }
    }
}

namespace TaleWorlds.CampaignSystem.Conversation
{
    public class ConversationManager { }
} 