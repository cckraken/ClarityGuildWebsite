using ClarityGuildWebsite.Api.Models;
using Discord;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public class DiscordModals
    {

        public DiscordModals()
        {
        }
        public Modal BuildOfficerMessageModalAsync(string discordId, string discordName)
        {
            var input = new TextInputBuilder()
                .WithCustomId("sendMessageToApplicantText")
                .WithStyle(TextInputStyle.Paragraph)
                .WithRequired(true)
                .WithMaxLength(250);

            return new ModalBuilder()
                .WithCustomId($"applicantMessageModal:{discordId}")
                .WithTitle("Send message to: " + discordName)
                .AddLabel($"Message:", input)
                .Build();
        }

        public Modal BuildApplicantMessageModalAsync()
        {
            var input = new TextInputBuilder()
                .WithCustomId("Sent message to")
                .WithStyle(TextInputStyle.Paragraph)
                .WithRequired(true)
                .WithMaxLength(250);

            return new ModalBuilder()
                .WithTitle("Message Clarity")
                .WithCustomId("btnMsgClarity")
                .AddLabel("Reply to Clarity", input)
                .Build();
        }

        public Modal BuildDeclineApplicant(string discordName, string discordId)
        {
            var options = new List<SelectMenuOptionBuilder>
            { 
        new SelectMenuOptionBuilder()
            .WithLabel("Class closed")
            .WithValue(DeclineReasons.ClassClosed)
            .WithDescription("We're not recruiting this class at the moment."),
        new SelectMenuOptionBuilder()
            .WithLabel("Not enough experience")
            .WithValue(DeclineReasons.LowExperience)
            .WithDescription("Not enough current raiding experience."),
        new SelectMenuOptionBuilder()
            .WithLabel("Other")
            .WithValue(DeclineReasons.Other)
            .WithDescription("Explain in the note below."),
            };

            var note = new TextInputBuilder()
                .WithCustomId(DeclineModal.NoteFieldId)
                .WithStyle(TextInputStyle.Paragraph)
                .WithRequired(false)
                .WithMaxLength(DeclineModal.NoteMaxLength);

            return new ModalBuilder()
                // "Decline " (8) + a username (max 32) stays under Discord's 45-character title limit
                .WithTitle($"Decline {discordName}")
                .WithCustomId($"{DeclineModal.CustomIdPrefix}{discordId}")
                .AddSelectMenu(
                    label: "Reason",
                    customId: DeclineModal.ReasonFieldId,
                    options: options,
                    placeholder: "Select a reason",
                    minValues: 1,
                    maxValues: 1,
                    required: true)
                .AddLabel("Note (required for Other)", note)
                .Build();
        }

        public static class DeclineModal
        {
            // Custom ID prefix for the modal; the router matches on this
            public const string CustomIdPrefix = "decline:";

            // Field IDs, read back in the submit handler
            public const string ReasonFieldId = "reason";
            public const string NoteFieldId = "note";

            public const int NoteMaxLength = 250;
        }

        public static class DeclineReasons
        {
            public const string ClassClosed = "class_closed";
            public const string LowExperience = "low_experience";
            public const string Other = "other";

            // For validating the value Discord sends back
            public static readonly IReadOnlySet<string> All =
                new HashSet<string> { ClassClosed, LowExperience, Other };
        }
    }
}
