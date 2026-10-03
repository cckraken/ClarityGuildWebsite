import {
  cardClass,
  cardTop,
  textBodyFull,
  fieldTitleClasses,
  appFormButtonClass,
} from "../components/styles/applicationClasses";
import { Link } from "react-router";

function GuildDesc() {
  return (
    <div className="mx-auto grid max-w-4xl gap-6">
      <div className="text-center">
        <p className="px-4 py-1 w-fit mx-auto rounded-full border border-clarity-blue/60 bg-clarity-blue/10 text-clarity-blue">
          Currently looking for a <strong>Shadow Priest</strong> and{" "}
          <strong>Elemental Shaman</strong>
        </p>
      </div>

      <div className={cardTop}>
        <h2 className={fieldTitleClasses}>Who are we?</h2>
        <p className={textBodyFull}>
          Clarity (Tarren Mill) is a long-standing mythic raiding guild. We are
          English speaking, CE experienced and progression focused, without
          losing the banter.
        </p>
        <p className={textBodyFull}>
          Currently WR 190-240 and climbing. Most of our prog lands Sun/Mon, so
          our rank jumps later in the week.
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <div className={cardClass}>
          <h3 className={fieldTitleClasses}>Before you apply</h3>
          <p className={textBodyFull}>
            Make sure you meet our requirements below:
          </p>
          <ul
            className={`${textBodyFull} list-disc list-outside pl-5 marker:text-clarity-blue`}
          >
            <li>
              Have a main and an alt character ready for raiding. Alts are
              mostly used for our mandatory splits at the start of the season
              (first 2-3 IDs), then for any roster needs during mythic prog
            </li>
            <li>Have a solid understanding of your class and role</li>
            <li>Be able to commit to our raid schedule</li>
            <li>Previous Cutting Edge experience</li>
            <li>90%+ attendance</li>
            <li>Full fight knowledge from prep</li>
          </ul>

          <h3 className={fieldTitleClasses}>What you get from us</h3>
          <ul
            className={`${textBodyFull} list-disc list-outside pl-5 marker:text-clarity-blue`}
          >
            <li>
              A highly experienced bench raid leader. Real leadership depth and
              organisation a lot of guilds don't have
            </li>
            <li>
              Clear prep material and prog analysis, provided ahead of every
              raid
            </li>
            <li>
              A seasoned core, efficient raid environment, and a lively guild
              banter culture
            </li>
          </ul>
        </div>

        <div className="grid gap-6">
          <div className={cardClass}>
            <h3 className={fieldTitleClasses}>Raid Schedule</h3>
            <dl
              className={`${textBodyFull} grid grid-cols-[auto_1fr] gap-x-4 gap-y-1`}
            >
              <dt className="font-medium">Wed</dt>
              <dd>19:45-23:10 ST</dd>
              <dt className="font-medium">Sun</dt>
              <dd>18:45-23:10 ST</dd>
              <dt className="font-medium">Mon</dt>
              <dd>19:45-23:10 ST</dd>
              <dt className="font-medium">Thu (Extra)</dt>
              <dd>19:45-23:10 ST</dd>
            </dl>
            <p className={`${textBodyFull} text-sm`}>
              (first 1-2 IDs of a new season / end-boss prog if we&apos;re close to
              HoF)
            </p>
          </div>

          <div className={cardClass}>
            <h3 className={fieldTitleClasses}>Progression</h3>
            <dl
              className={`${textBodyFull} grid grid-cols-[auto_1fr] gap-x-4 gap-y-1`}
            >
              <dt className="font-medium text-clarity-blue">Current tier</dt>
              <dd className="text-clarity-blue">6/8M VA</dd>
              <dt className="font-medium">MDN S1</dt>
              <dd>9/9M WR 381</dd>
              <dt className="font-medium">TWW S3</dt>
              <dd>8/8M WR 320</dd>
              <dt className="font-medium">TWW S2</dt>
              <dd>8/8M WR 216 (HoF)</dd>
              <dt className="font-medium">TWW S1</dt>
              <dd>8/8M WR 327</dd>
            </dl>
          </div>
        </div>

        <div className="md:col-span-2 flex justify-center">
          <Link to="/apply" className={appFormButtonClass}>
            Apply now
          </Link>
        </div>
      </div>
    </div>
  );
}

export default GuildDesc;
