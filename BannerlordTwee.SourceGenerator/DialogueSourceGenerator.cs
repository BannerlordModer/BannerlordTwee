using BannerlordTwee.SourceGenerator.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BannerlordTwee.SourceGenerator
{
    [Generator]
    public class DialogueSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }

        public void Execute(GeneratorExecutionContext context)
        {
            IEnumerable<AdditionalText> tweeFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".twee"));

            foreach (AdditionalText tweeFile in tweeFiles)
            {
                ProcessTweeFile(tweeFile, context);
            }
        }

        private void ProcessTweeFile(AdditionalText tweeFile, GeneratorExecutionContext context)
        {
            var content = tweeFile.GetText(context.CancellationToken)?.ToString();
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var parser = new TweeParser(content);
            var story = parser.ParseToStory();
            if (story == null || story.Passages.Count == 0)
            {
                return;
            }

            string tweeFileName = Path.GetFileNameWithoutExtension(tweeFile.Path);
            string className = SanitizeForIdentifier(tweeFileName) + "Dialogs";

            // Get namespace from build property, or use default
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.BannerlordTweeNamespace", out var customNamespace);
            string namespaceName = string.IsNullOrWhiteSpace(customNamespace) ? "BannerlordTwee.Generated.Dialogs" : customNamespace;

            var sourceBuilder = new StringBuilder();
            BuildSourceForStory(sourceBuilder, story, className, tweeFileName, namespaceName);

            context.AddSource(
                $"{className}.g.cs",
                SourceText.From(sourceBuilder.ToString(), Encoding.UTF8)
            );
        }

        private void BuildSourceForStory(StringBuilder sb, Models.Story story, string className, string conversationId, string namespaceName)
        {
            sb.AppendLine("using TaleWorlds.CampaignSystem;");
            sb.AppendLine("using TaleWorlds.CampaignSystem.Conversation;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static partial class {className}");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static void AddDialogs(CampaignGameStarter gameStarter)");
            sb.AppendLine("        {");

            foreach (var passage in story.Passages)
            {
                string passageName = SanitizeForIdentifier(passage.Title.Name);
                string entryPoint = $"{conversationId}_{passageName}";
                string lineText = GetLineText(passage.Content);

                sb.AppendLine($"            gameStarter.AddDialogLine(\"{entryPoint}\", \"{entryPoint}_start\", \"{entryPoint}_player_options\", \"{lineText}\", () => {passageName}Condition(), null);");

                foreach (var link in passage.Links)
                {
                    string linkTargetPassageName = SanitizeForIdentifier(link.LinkToPassageName);
                    string linkTargetEntryPoint = $"{conversationId}_{linkTargetPassageName}_start";
                    string linkText = SanitizeText(link.ShowString);
                    sb.AppendLine($"            gameStarter.AddPlayerLine(\"{entryPoint}_{linkTargetPassageName}\", \"{entryPoint}_player_options\", \"{linkTargetEntryPoint}\", \"{linkText}\", () => {passageName}_{linkTargetPassageName}LinkCondition(), () => {passageName}_{linkTargetPassageName}LinkConsequence());");
                }
            }
            sb.AppendLine("        }");
            sb.AppendLine();

            foreach (var passage in story.Passages)
            {
                string passageName = SanitizeForIdentifier(passage.Title.Name);
                sb.AppendLine($"        private static partial bool {passageName}Condition();");
                foreach (var link in passage.Links)
                {
                    string linkTargetPassageName = SanitizeForIdentifier(link.LinkToPassageName);
                    sb.AppendLine($"        private static partial bool {passageName}_{linkTargetPassageName}LinkCondition();");
                    sb.AppendLine($"        private static partial void {passageName}_{linkTargetPassageName}LinkConsequence();");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
        }
        
        private static readonly Regex CommentRegex = new Regex("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex LinkRegex = new Regex("\\[\\[(.*?)\\]\\]", RegexOptions.Compiled);

        private string GetLineText(string content)
        {
            if (string.IsNullOrEmpty(content)) return "";
            var contentWithoutComments = CommentRegex.Replace(content, string.Empty);
            var contentWithoutLinks = LinkRegex.Replace(contentWithoutComments, string.Empty);
            return SanitizeText(contentWithoutLinks);
        }

        private string SanitizeText(string text)
        {
            return text?.Replace("\n", "").Replace("\r", "").Replace("\"", "\\\"").Trim() ?? string.Empty;
        }

        private string SanitizeForIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name)) return "_";
            
            string sanitized = Regex.Replace(name, @"[^\w]", "_");

            if (char.IsDigit(sanitized[0]))
            {
                sanitized = "_" + sanitized;
            }
            return sanitized;
        }
    }
} 