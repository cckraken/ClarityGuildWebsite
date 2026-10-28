using Discord;
using Discord.LibDave;


namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public static class DiscordModals
    {
        public static Modal BuildOfficerMessage(Guid publicId, string discordName)
        {
            var input = new TextInputBuilder()
                .WithCustomId(OfficerMessageModal.MessageFieldId)
                .WithStyle(TextInputStyle.Paragraph)
                .WithRequired(true)
                .WithMaxLength(OfficerMessageModal.MessageMaxLength);

            return new ModalBuilder()
                .WithTitle($"Message {discordName}")
                .WithCustomId($"{OfficerMessageModal.Type}:{publicId}")
                .AddLabel("Message", input)
                .Build();
        }

        public static Modal BuildDeclineApplicant(Guid publicId, string discordName)
        {
            var options = DeclineReasons.OfficerLabels
                .Select(r => new SelectMenuOptionBuilder()
                    .WithLabel(r.Value)
                    .WithValue(r.Key))
                .ToList();

            var note = new TextInputBuilder()
                .WithCustomId(DeclineModal.NoteFieldId)
                .WithStyle(TextInputStyle.Paragraph)
                .WithRequired(false)
                .WithMaxLength(DeclineModal.NoteMaxLength);

            return new ModalBuilder()
                .WithTitle($"Decline {discordName}")
                .WithCustomId($"{DeclineModal.Type}:{publicId}")
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

        public static Modal BuildAcceptApplicant(Guid publicId, string discordName)
        {

            var note = new TextInputBuilder()
                .WithCustomId(AcceptModal.WelcomeMessageFieldId)
                .WithValue(AcceptModal.DefaultWelcomeMessage)
                .WithStyle(TextInputStyle.Paragraph)
                .WithRequired(false)
                .WithMaxLength(AcceptModal.MessageMaxLength);

            return new ModalBuilder()
                .WithTitle($"Accept {discordName}")
                .WithCustomId($"{AcceptModal.Type}:{publicId}")
                .AddLabel("Welcome message", note)
                .AddSelectMenu(
            label: "Channels to highlight",
            customId: AcceptModal.ChannelsFieldId,
            type: ComponentType.ChannelSelect,
            channelTypes: new[] { ChannelType.Text },
            minValues: 0,
            maxValues: 4,
            required: false,
            placeholder: "Select one or more channels")
                .Build();
        }

        public static class OfficerMessageModal
        {
            public const string Type = "applicantMessageModal";
            public const string MessageFieldId = "message";
            public const int MessageMaxLength = 1000;
        }

        public static class AcceptModal
        {
            public const string Type = "accept";
            public const string WelcomeMessageFieldId = "welcomeMessage";
            public const string ChannelsFieldId = "channels";
            public const string DefaultWelcomeMessage = "Welcome to Clarity, {member}! Have a look at {channels} to get started and whisper anyone for a guild invite.";
            public const int MessageMaxLength = 1000;
        }

        public static class DeclineModal
        {
            public const string Type = "decline";
            public const string ReasonFieldId = "reason";
            public const string NoteFieldId = "note";
            public const int NoteMaxLength = 250;
        }

        public static class DeclineReasons
        {
            public const string ClassClosed = "class_closed";
            public const string LowExperience = "low_experience";
            public const string Other = "other";

            public static readonly IReadOnlyDictionary<string, string> OfficerLabels = new Dictionary<string, string>
            {
                [ClassClosed] = "Class closed",
                [LowExperience] = "Not enough experience",
                [Other] = "Other (explain in the note)",
            };

            public static readonly IReadOnlyDictionary<string, string> ApplicantText = new Dictionary<string, string>
            {
                [ClassClosed] = "We're not recruiting your class at the moment.",
                [LowExperience] = "We're looking for a bit more current raiding experience at the moment.",
            };
        }
    }
}
    