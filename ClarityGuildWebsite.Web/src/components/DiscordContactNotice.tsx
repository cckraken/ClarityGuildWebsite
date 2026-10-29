import { fieldSubTitleClass, fieldHintClass, textBodyFullClass, textLinkClass } from './styles/applicationClasses';

type DiscordContactNoticeProps = {
  inviteUrl: string;
};

export function DiscordContactNotice({ inviteUrl }: DiscordContactNoticeProps) {
  return (
    <section className="flex flex-col gap-2 rounded-xl border border-clarity-blue/40 bg-clarity-blue/5 p-4">
      <h3 className={fieldSubTitleClass}>How we’ll contact you</h3>
      <p className={textBodyFullClass}>
        Officers contact applicants through our Discord bot, <strong className="text-titles">CCK</strong>.
      </p>
      <ol className={`${textBodyFullClass} flex list-decimal flex-col gap-1 pl-5 marker:text-clarity-blue`}>
        <li>
          <a href={inviteUrl} target="_blank" rel="noopener noreferrer" className={textLinkClass}>
            Join our Discord server<span className="sr-only"> (opens in a new tab)</span>
          </a>
        </li>
        <li>Allow direct messages from server members (right-click the server icon → Privacy Settings)</li>
        <li>Reply to CCK’s messages and they’ll be passed straight to the officers</li>
      </ol>
      <p className={`${fieldHintClass} italic`}>
        This is optional, but if CCK can’t message you, we won’t be able to ask you questions or tell you our decision.
      </p>
    </section>
  );
}
