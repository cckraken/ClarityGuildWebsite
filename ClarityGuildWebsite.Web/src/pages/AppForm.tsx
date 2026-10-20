import { useForm, type SubmitHandler, type DefaultValues } from "react-hook-form";
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from "zod";
import { useState, useEffect } from 'react'
import { AppFormSubmitDialogue } from "../components/AppForm/AppFormSubmitDialogue";
import { cardClass, fieldInputClass, textAreaInputClass, fieldTitleClass, fieldSubTitleClass, fieldHintClass, appFormButtonClass, appFormButtonDiscordClass } from "../components/styles/applicationClasses";
import discordLogo from "../assets/discord_PNG7.png";
import { useCurrentUser } from "../hooks/useCurrentUser";


const roles = ["Tank", "Healer", "DPS"] as const;

const schema = z.object({
  country: z.string().trim().min(1, { error: "Country is required" }).max(50, {error: "50 characters or less"}),
  mainName: z.string().trim().min(1, { error: "Main is required" }).max(20, {error: "20 characters or less"}),
  mainRealm: z.string().trim().min(1, { error: "Realm is required" }).max(25, {error: "25 characters or less"}),
  mainRole: z.enum(roles, { error: "Role is required" }),
  altName: z.string().trim().min(1, { error: "Alt is required" }).max(20, {error: "20 characters or less"}),
  altRealm: z.string().trim().min(1, { error: "Realm is required" }).max(25, {error: "25 characters or less"}),
  altRole: z.enum(roles, { error: "Role is required" }),
  screenshot: z.url({ protocol: /^https$/i, error: "Must be a valid https:// link" }).refine((val) => !val.toLowerCase().includes("imgur"), {error: "No Imgur links, sorry!"}).transform((val) => new URL(val).href).pipe(z.string().max(500, {error: "Too long!"})),
  about: z.string().trim().min(1, {error: "About is required"}).max(2000, { error: "2k characters or less"}),
  tech: z.string().trim().min(1, { error: "Tech is required" }).max(500, {error: "500 characters or less"}),
  history: z.string().trim().min(1,  { error: "History is required" }).max(2000, {error: "2k characters or less"}),
  extra: z.string().trim().max(2000, {error: "2k characters or less"}).optional(),
});

const DRAFT_KEY = "guildApplicationDraft";
const emptyForm: DefaultValues<z.input<typeof schema>> = {
  country: "",
  mainName: "",
  mainRealm: "",
  mainRole: undefined,
  altName: "",
  altRealm: "",
  altRole: undefined,
  screenshot: "",
  about: "",
  tech: "",
  history: "",
  extra: "",
};

function clearDraft() {
  try {
    sessionStorage.removeItem(DRAFT_KEY);
  } catch {
    // storage unavailable: nothing to clear
  }
}

function loadDraft() {
  try {
    const raw = sessionStorage.getItem(DRAFT_KEY);
    return raw ? JSON.parse(raw) : undefined;
  } catch {
    return undefined;
  }
}

export function AppForm() {
  const {
    register,
    handleSubmit,
    subscribe,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<z.input<typeof schema>, unknown, z.output<typeof schema>>({
    resolver: zodResolver(schema),
    defaultValues: loadDraft() ?? emptyForm,
  });

  useEffect(() => {
    const unsubscribe = subscribe({
      formState: { values: true },
      callback: ({ values }) => {
        try {
          sessionStorage.setItem(DRAFT_KEY, JSON.stringify(values));
        } catch {
          // storage unavailable: cannot save draft
        }
      },
    });
    return unsubscribe;
  }, [subscribe]);

const [status, setStatus] = useState<"success" | "error" | "servererror" | "errorTooManyRequests" | null>(null);
const [submissionDate, setSubmissionDate] = useState<string | null>(null);
const { state, signOut } = useCurrentUser();

  const onSubmit: SubmitHandler<z.infer<typeof schema>> = async (data) => { 
  try {
    const response = await fetch(
      `${import.meta.env.VITE_API_BASE_URL}/api/guildapplication`,
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      }
    );
    if (response.ok) {
      const body = await response.json();
      setStatus("success");
      setSubmissionDate(body.timestampUtc);
      reset(emptyForm);
      clearDraft();
    } else if (response.status === 429) {
      setStatus("errorTooManyRequests");
    } else if (response.status === 404) {
      setStatus("servererror");
    } else {
      setStatus("error");
    }
  } catch {
    setStatus("error");
  }
  };

 function handleDiscordSignIn() {
  const returnUrl = encodeURIComponent(window.location.href);
  const baseUrl = import.meta.env.VITE_API_BASE_URL;
  window.location.href = `${baseUrl}/api/auth/login?returnUrl=${returnUrl}`;
}


  return (

<form className="grid grid-cols-1 gap-6 font-light md:grid-cols-2" onSubmit={handleSubmit(onSubmit)}>
  <AppFormSubmitDialogue
  status={status}
  submissionDate={submissionDate}
  onClose={() => { setStatus(null); setSubmissionDate(null); }}
/>

 <div className={cardClass}>

    <div className="flex flex-col gap-2">
      <div className="flex flex-col gap-1">
    <h2 className={fieldTitleClass}>Your Details</h2>
    <p className={fieldHintClass}>The basics we need to find you in-game and check you out.</p>
    </div>
    </div>

        <div className="flex flex-wrap items-center justify-between gap-2">
          <div className="flex flex-col gap-1">
            <span className={fieldSubTitleClass}>
              Discord
              {state.status === "signedIn" && (
                <>: <strong>{state.user.discordName}</strong></>
              )}
            </span>
            <p className={fieldHintClass}>We use this to contact you</p>
          </div>

          {state.status === "signedOut" && (
            <button
              type="button"
              onClick={handleDiscordSignIn}
              className={appFormButtonDiscordClass}
            >
              <img src={discordLogo} alt="" className="h-5 w-5" />
              Sign in with Discord
            </button>
          )}

          {state.status === "signedIn" && (
            <button
              type="button"
              onClick={signOut}
              className={appFormButtonDiscordClass}
            >
              <img src={discordLogo} alt="" className="h-5 w-5" />
              Not you? Sign out
            </button>
          )}

          {state.status === "loading" && <p>Loading…</p>}
          {state.status === "error" && <p>Couldn&apos;t check sign-in. Please refresh.</p>}
        </div>

      <div className="flex flex-col gap-1">
        <label className={fieldSubTitleClass} htmlFor="country">Country</label>
        <p className={fieldHintClass}>Where do you play the game from</p>
        <input className={fieldInputClass}
          {...register('country') }
          type="text"
          id="country"
          aria-invalid={!!errors.country}
          aria-describedby={errors.country ? "country-error" : undefined}
          />
      {errors.country && (<div id="country-error" className="text-xs text-error">{errors.country.message}</div>)}
      </div>

<div className="flex flex-col gap-1">
  <p className={fieldHintClass}>Main character & an alt character</p>
        <div className="grid grid-cols-3 gap-4">

        <div className="flex flex-col gap-1">
          <label className={fieldSubTitleClass} htmlFor="mainName">Name</label>
            <input className={fieldInputClass}
              {...register('mainName')}
              type="text"
              id="mainName"
              aria-invalid={!!errors.mainName}
              aria-describedby={errors.mainName ? "mainName-error" : undefined}
              />
          {errors.mainName && (<div id="mainName-error" className="text-xs text-error">{errors.mainName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className={fieldSubTitleClass} htmlFor="mainRealm">Realm</label>
              <input className={fieldInputClass}
                {...register("mainRealm")}
                type="text"
                id="mainRealm"
                aria-invalid={!!errors.mainRealm}
                aria-describedby={errors.mainRealm ? "mainRealm-error" : undefined}
              />
              {errors.mainRealm && (<div id="mainRealm-error" className="text-xs text-error">{errors.mainRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className={fieldSubTitleClass} htmlFor="mainRole">Role</label>
              <select className={fieldInputClass}
                {...register("mainRole")}
                id="mainRole"
                aria-invalid={!!errors.mainRole}
                aria-describedby={errors.mainRole ? "mainRole-error" : undefined}
              >
                <option value="">Select a role</option>
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
              {errors.mainRole && (<div id="mainRole-error" className="text-xs text-error">{errors.mainRole.message}</div>)}
          </div>

        </div>
      </div>

      <div className="flex flex-col gap-1">

        <div className="grid grid-cols-3 gap-4">

        <div className="flex flex-col gap-1">
          <label className="sr-only" htmlFor="altName">Alt Name</label>
            <input className={fieldInputClass}
              {...register('altName')}
              type="text"
              id="altName"
              aria-invalid={!!errors.altName}
              aria-describedby={errors.altName ? "altName-error" : undefined}
              />
          {errors.altName && (<div id="altName-error" className="text-xs text-error">{errors.altName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className="sr-only" htmlFor="altRealm">Alt Realm</label>
              <input className={fieldInputClass}
                {...register("altRealm")}
                type="text"
                id="altRealm"
                aria-invalid={!!errors.altRealm}
                aria-describedby={errors.altRealm ? "altRealm-error" : undefined}
              />
              {errors.altRealm && (<div id="altRealm-error" className="text-xs text-error">{errors.altRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className="sr-only" htmlFor="altRole">Alt Role</label>
              <select className={fieldInputClass}
                {...register("altRole")}
                id="altRole"
                aria-invalid={!!errors.altRole}
                aria-describedby={errors.altRole ? "altRole-error" : undefined}
              >
                <option value="">Select a role</option>
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
              {errors.altRole && (<div id="altRole-error" className="text-xs text-error">{errors.altRole.message}</div>)}
          </div>

        </div>
      </div>

        <div className="flex flex-col gap-1">
        <label className={fieldSubTitleClass} htmlFor="screenshot">UI Screenshot</label>
          <p className={fieldHintClass}>Preferably taken in combat</p>
        <input className={fieldInputClass}
          {...register("screenshot") }
          type="url"
          id="screenshot"
          aria-invalid={!!errors.screenshot}
          aria-describedby={errors.screenshot ? "screenshot-error" : undefined}
        />
          {errors.screenshot && (<div id="screenshot-error" className="text-xs text-error">{errors.screenshot.message}</div>)}
      </div> 

      <div className="flex flex-col gap-1">
        <label className ={fieldSubTitleClass} htmlFor="tech">Tech</label>
        <p className={fieldHintClass}>System specs, internet etc</p>
          <textarea className ={fieldInputClass}
            {...register("tech")}
            id="tech"
            aria-invalid={!!errors.tech}
            aria-describedby={errors.tech ? "tech-error" : undefined}
          />
          {errors.tech && (<div id="tech-error" className="text-xs text-error">{errors.tech.message}</div>)}
         </div>

</div>


    <div className={cardClass}>

    <div className="flex flex-col gap-1">
    <h2 className={fieldTitleClass}>In your own words</h2>
      <p className={fieldHintClass}>Let us get to know you</p>

    </div>
      
        <div className="flex flex-col gap-4">
          
          <div className="flex flex-col gap-1">
        <label className ={fieldSubTitleClass} htmlFor="about">About you</label>
        <p className={fieldHintClass}>Job, hobbies etc. Whatever you like</p>
          <textarea className ={textAreaInputClass}
            {...register("about")}
            id="about"
            aria-invalid={!!errors.about}
            aria-describedby={errors.about ? "about-error" : undefined}
          />
          {errors.about && (<div id="about-error" className="text-xs text-error">{errors.about.message}</div>)}
          </div>         

         <div className="flex flex-col gap-1">
        <label className={fieldSubTitleClass} htmlFor="history">History</label>
        <p className={fieldHintClass}>Ex guilds, why you left, etc</p>
        <textarea className={textAreaInputClass}
          {...register("history")}
          id="history"
          aria-invalid={!!errors.history}
          aria-describedby={errors.history ? "history-error" : undefined}
        />
        {errors.history && (<div id="history-error" className="text-xs text-error">{errors.history.message}</div>)}
      </div>
   
        <div className="flex flex-col gap-1">
        <label className={fieldSubTitleClass} htmlFor="extra">Extra</label>
        <p className={fieldHintClass}>Anything extra? If you play other alts, here&apos;s the place</p>
          <textarea className={textAreaInputClass}
          {...register("extra")}
          id="extra"
          aria-invalid={!!errors.extra}
          aria-describedby={errors.extra ? "extra-error" : undefined}
        />
          {errors.extra && (<div id="extra-error" className="text-xs text-error">{errors.extra.message}</div>)}
      </div>    

    </div>


  </div>

<>
<div className="md:col-span-2 flex justify-center"
  
>
  <button
    className={appFormButtonClass}
    disabled={state.status !== "signedIn" || isSubmitting}
    type="submit"
  >
    {isSubmitting ? "Submitting..." : "Submit"}
  </button>
</div>
</>
  </form>
  )
}
