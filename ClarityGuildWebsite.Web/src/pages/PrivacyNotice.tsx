import { cardClass, fieldTitleClass, fieldSubTitleClass, fieldHintClass, textBodyFullClass, textLinkClass } from '../components/styles/applicationClasses';

const CONTACT = "cckraken17";
const LAST_UPDATED = "29 October 2026";

const sectionClass = "flex flex-col gap-2";
const listClass = `${textBodyFullClass} flex list-disc flex-col gap-1 pl-5 marker:text-clarity-blue`;

export function PrivacyNotice() {
  return (
    <article className={`${cardClass} gap-4`}>
      <header className="flex flex-col gap-1">
        <h1 className={`${fieldTitleClass} text-2xl`}>Privacy Notice</h1>
        <p className={fieldHintClass}>Last updated {LAST_UPDATED}</p>
      </header>

      <section className={sectionClass}>
        <h2 className={fieldSubTitleClass}>Who we are</h2>
        <p className={textBodyFullClass}>
          Clarity is a World of Warcraft guild. If you have questions about your data, contact <strong className="text-titles">{CONTACT}</strong> on Discord.
        </p>
      </section>

      <section className={sectionClass}>
        <h2 className={fieldSubTitleClass}>What we collect</h2>
        <ul className={listClass}>
          <li>Your Discord ID and username, taken from Discord when you sign in</li>
          <li>Your application answers: country, characters, realms, roles, screenshot link and written answers</li>
          <li>Messages you send to our bot, CCK, about your application</li>
        </ul>
        <p className={textBodyFullClass}>
          We use this only to review your application and to talk to you about it.
        </p>
      </section>

      <section className={sectionClass}>
        <h2 className={fieldSubTitleClass}>Where it goes</h2>
        <ul className={listClass}>
          <li><strong className="text-titles">Microsoft Azure</strong> stores your application in our database.</li>
          <li><strong className="text-titles">Discord</strong> receives a copy in a private, officers-only channel. Messages between you and CCK are posted to a thread under it.</li>
        </ul>
        <p className={textBodyFullClass}>We don’t sell your data or share it with anyone else, except where the law requires it.</p>
      </section>

      <section className={sectionClass}>
        <h2 className={fieldSubTitleClass}>How long we keep it</h2>
        <ul className={listClass}>
          <li>Our database deletes your application automatically 3 months after you submit it.</li>
          <li>Officers may keep the Discord copy and message thread for reference. Contact <strong className="text-titles">{CONTACT}</strong> to have them deleted.</li>
          <li>Messages with CCK are not stored in our database.</li>
        </ul>
      </section>

      <section className={sectionClass}>
        <h2 className={fieldSubTitleClass}>Cookies</h2>
        <p className={textBodyFullClass}>
          We use cookies only to sign you in whilst you apply. It’s needed for the site to work, and we don’t use tracking or advertising cookies.
        </p>
      </section>

      <section className={sectionClass}>
        <h2 className={fieldSubTitleClass}>Your rights</h2>
        <p className={textBodyFullClass}>
          You can ask to see, correct or delete your data at any time by messaging <strong className="text-titles">{CONTACT}</strong> on Discord.
          If you’re unhappy with how we’ve handled it, you can complain to the{" "}
          <a href="https://ico.org.uk/make-a-complaint/" target="_blank" rel="noopener noreferrer" className={textLinkClass}>
            Information Commissioner’s Office<span className="sr-only"> (opens in a new tab)</span>
          </a>.
        </p>
      </section>
    </article>
  );
}
