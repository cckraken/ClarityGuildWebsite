import {
  cardClass,
  cardTopClass,
  textBodyFullClass,
  fieldTitleClass,
  appFormButtonClass,
  dataTableClass,
} from "../components/styles/applicationClasses";
import { DiscordContactNotice } from "../components/DiscordContactNotice";
import { Fragment } from "react";
import { Link } from "react-router";

const progression = [
  { tier: "Current tier", result: "8/8M VA WR 328", current: true },
  { tier: "MDN S1", result: "9/9M WR 381", current: false },
  { tier: "TWW S3", result: "8/8M WR 320", current: false },
  { tier: "TWW S2", result: "8/8M WR 216 (HoF)", current: false },
  { tier: "TWW S1", result: "8/8M WR 327", current: false },
];

export function GuildDesc() {
  return (
    <div className="mx-auto grid max-w-4xl gap-6">
      <title>Clarity | Mythic raiding guild on Tarren Mill</title>

      <div className="text-center">
        <p className="mx-auto w-fit rounded-2xl border border-clarity-blue/60 bg-clarity-blue/10 px-4 py-1 text-clarity-blue md:rounded-full">
          Currently looking for any exceptional players! Particularly interested
          in a new <strong>main tank.</strong>
        </p>
      </div>

      <div className={cardTopClass}>
        <h1 className={fieldTitleClass}>Who are we?</h1>
        <p className={textBodyFullClass}>
          Clarity (Tarren Mill) is a long-standing mythic raiding guild. We are
          English speaking, CE experienced and progression focused, without
          losing the banter.
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <div className={cardClass}>
          <div className="flex flex-col gap-6">
            <div className="flex flex-col gap-2">
              <h2 className={fieldTitleClass}>Before you apply</h2>
              <p className={textBodyFullClass}>
                Make sure you meet our requirements below:
              </p>
              <ul
                className={`${textBodyFullClass} list-disc pl-5 marker:text-clarity-blue`}
              >
                <li>
                  Have a main and an alt character ready for raiding. Alts are
                  mostly used for our mandatory splits at the start of the
                  season (first 2-3 IDs), then for any roster needs during
                  mythic prog
                </li>
                <li>Have a solid understanding of your class and role</li>
                <li>Be able to commit to our raid schedule</li>
                <li>Previous Cutting Edge experience</li>
                <li>90%+ attendance</li>
                <li>Full fight knowledge from prep</li>
              </ul>
            </div>

            <div className="flex flex-col gap-2">
              <h2 className={fieldTitleClass}>What you get from us</h2>
              <ul
                className={`${textBodyFullClass} list-disc pl-5 marker:text-clarity-blue`}
              >
                <li>
                  A highly experienced bench raid leader. Real leadership depth
                  and organisation a lot of guilds don&apos;t have
                </li>
                <li>
                  Clear prep material and prog analysis, provided ahead of every
                  raid
                </li>
                <li>
                  A seasoned core, efficient raid environment, and a lively
                  guild banter culture
                </li>
              </ul>
            </div>
          </div>
        </div>

        <div className="grid gap-6">
          <div className={cardClass}>
            <h2 className={fieldTitleClass}>Raid Schedule</h2>
            <dl className={dataTableClass}>
              <dt className="font-medium">Wed</dt>
              <dd>19:45-23:10 ST</dd>
              <dt className="font-medium">Sun</dt>
              <dd>18:45-23:10 ST</dd>
              <dt className="font-medium">Mon</dt>
              <dd>19:45-23:10 ST</dd>
              <dt className="font-medium">Thu (Extra)</dt>
              <dd>19:45-23:10 ST</dd>
            </dl>
            <p className={`${textBodyFullClass} text-sm`}>
              Thursday is only for the first 1-2 IDs of a new season, or
              end-boss prog if we&apos;re close to HoF.
            </p>
          </div>

          <div className={cardClass}>
            <h2 className={fieldTitleClass}>Progression</h2>
            <dl className={dataTableClass}>
              {progression.map(({ tier, result, current }) => (
                <Fragment key={tier}>
                  <dt
                    className={`font-medium ${current ? "text-clarity-blue" : ""}`}
                  >
                    {tier}
                  </dt>
                  <dd className={current ? "text-clarity-blue" : undefined}>
                    {result}
                  </dd>
                </Fragment>
              ))}
            </dl>
          </div>
        </div>

        <div className="md:col-span-2">
          <DiscordContactNotice
            inviteUrl={import.meta.env.VITE_DISCORD_INVITE_URL}
          />
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
