import { useForm, type SubmitHandler } from "react-hook-form";
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from "zod";
import { useState, useRef } from 'react'
import { AppFormSubmitDialogue } from "../components/AppForm/AppFormSubmitDialogue";
import { cardClass, fieldInputClass, fieldTitleClass, fieldSubTitleClass, fieldHintClass, appFormButtonClass, resizeTabsClass } from "../components/styles/applicationClasses";

const roles = ["Tank", "Healer", "DPS"] as const;

const schema = z.object({
  discordId: z.string().trim().min(1, { error: "Discord ID is required" }).max(50, {error: "50 characters or less"}),
  country: z.string().trim().min(1, { error: "Country is required" }).max(50, {error: "50 characters or less"}),
  mainName: z.string().trim().min(1, { error: "Main is required" }).max(50, {error: "50 characters or less"}),
  mainRealm: z.string().trim().min(1, { error: "Realm is required" }).max(50, {error: "50 characters or less"}),
  mainRole: z.enum(roles, { error: "Role is required" }),
  altName: z.string().trim().min(1, { error: "Alt is required" }).max(50, {error: "50 characters or less"}),
  altRealm: z.string().trim().min(1, { error: "Realm is required" }).max(50, {error: "50 characters or less"}),
  altRole: z.enum(roles, { error: "Role is required" }),
  screenshot: z.url({error:"Screenshot is required"}).refine((val) => !val.toLowerCase().includes("imgur"), {error: "No Imgur links, sorry!"}).max(100, {error: "Too long!"}),
  about: z.string().trim().min(1, {error: "About is required"}).max(2000, { error: "2k characters or less"}),
  tech: z.string().trim().min(1, { error: "Tech is required" }).max(500, {error: "500 characters or less"}),
  history: z.string().trim().min(1,  { error: "History is required" }).max(2000, {error: "2k characters or less"}),
  extra: z.string().trim().max(2000, {error: "2k characters or less"}).optional(),
});

export function AppForm() {
  const {
     register,
      handleSubmit,
      reset,
       formState: { errors, isSubmitting },
       } = useForm<z.input<typeof schema>,
        unknown, z.output<typeof schema>>({ resolver: zodResolver(schema),

        });

        const [isEnlarged, setIsEnlarged] = useState(false);
        const textAreaGridClasses = isEnlarged
        ? "grid grid-cols-1 gap-4 md:col-span-2"
        : "grid grid-cols-1 gap-12.5"

const [status, setStatus] = useState<"success" | "error" | "servererror" | "errorTooManyRequests" | null>(null);

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
      setStatus("success");
      reset(); 
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

  const textAreasRef = useRef<HTMLDivElement>(null);

  return (

    

<form className="grid grid-cols-1 gap-6 font-light md:grid-cols-2" onSubmit={handleSubmit(onSubmit)}>
  <AppFormSubmitDialogue
  status={status}
  onClose={() => setStatus(null)}
/>

 <div className={`self-start ${cardClass}`}>

    <div className="flex flex-col gap-1">
    <h1 className={fieldTitleClass}>Your Details</h1>
    <p className={fieldHintClass}>The basics we need to find you in-game and check you out.</p>
    </div>

      <div className="flex flex-col gap-1">
        <label className={fieldSubTitleClass} htmlFor="discordId">Discord ID</label>
        <p className={fieldHintClass}>Our primary way to contact you</p>
        <input className={fieldInputClass}
          {...register('discordId') }
          type="text"
          id="discordId"
          />
      {errors.discordId && (<div className="text-xs text-error">{errors.discordId.message}</div>)}
      </div>

      <div className="flex flex-col gap-1">
        <label className={fieldSubTitleClass} htmlFor="country">Country</label>
        <p className={fieldHintClass}>Where do you play the game from</p>
        <input className={fieldInputClass}
          {...register('country') }
          defaultValue=""
          type="text"
          id="country"
          />
      {errors.country && (<div className="text-xs text-error">{errors.country.message}</div>)}
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
              />
          {errors.mainName && (<div className="text-xs text-error" aria-invalid={errors.mainName ? true : undefined}>{errors.mainName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className={fieldSubTitleClass} htmlFor="mainRealm">Realm</label>
              <input className={fieldInputClass}
                {...register("mainRealm")}
                type="text"
                id="mainRealm"
              />
              {errors.mainRealm && (<div className="text-xs text-error">{errors.mainRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className={fieldSubTitleClass} htmlFor="mainRole">Role</label>
              <select className={fieldInputClass}
                {...register("mainRole")}
                id="mainRole"
              >
                <option value="">Select a role</option>
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
              {errors.mainRole && (<div className="text-xs text-error">{errors.mainRole.message}</div>)}
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
              />
          {errors.altName && (<div className="text-xs text-error">{errors.altName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className="sr-only" htmlFor="altRealm">Alt Realm</label>
              <input className={fieldInputClass}
                {...register("altRealm")}
                type="text"
                id="altRealm"
              />
              {errors.altRealm && (<div className="text-xs text-error">{errors.altRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className="sr-only" htmlFor="altRole">Alt Role</label>
              <select className={fieldInputClass}
                {...register("altRole")}
                id="altRole"
              >
                <option value="">Select a role</option>
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
              {errors.altRole && (<div className="text-xs text-error">{errors.altRole.message}</div>)}
          </div>

        </div>
      </div>

        <div className="flex flex-col gap-1">
        <label className={fieldTitleClass} htmlFor="screenshot">UI Screenshot</label>
          <p className={fieldHintClass}>Preferably taken in combat</p>
        <input className={fieldInputClass}
          {...register("screenshot") }
          type="url"
          id="screenshot"
        />
          {errors.screenshot && (<div className="text-xs text-error">{errors.screenshot.message}</div>)}
      </div> 

      <div className="flex flex-col gap-1">
        <label className ={fieldTitleClass} htmlFor="tech">Tech</label>
        <p className={fieldHintClass}>System specs, internet etc</p>
          <textarea className ={fieldInputClass}
            {...register("tech")}
            id="tech"
          />
          {errors.tech && (<div className="text-xs text-error">{errors.tech.message}</div>)}
         </div>

</div>


    <div className={`self-start ${cardClass}`}>

    <div className="flex flex-col gap-1">
    <h1 className={fieldTitleClass}>In your own words</h1>
      <p className={fieldHintClass}>Let us get to know you</p>

<div role="group" aria-label="Text box size" className="self-start inline-flex rounded-md border border-border p-0.5">
          <button
            type="button"
            aria-pressed={!isEnlarged}
            onClick={() => {
            textAreasRef.current?.querySelectorAll("textarea").forEach((t) => {t.style.height = ""; t.style.width = ""});
            setIsEnlarged(false);
            }}
            className={resizeTabsClass}
          >
            Normal
          </button>
          <button
          type="button"
          aria-pressed={isEnlarged}
          onClick={() => {
          textAreasRef.current?.querySelectorAll("textarea").forEach((t) => {t.style.height = ""; t.style.width = ""});
          setIsEnlarged(true);
          }}
          className={resizeTabsClass}
          >
            Large
          </button>      
      </div>

    </div>
      
        <div ref={textAreasRef} className={textAreaGridClasses}>
          
          <div className="flex flex-col gap-1">
        <label className ={fieldTitleClass} htmlFor="about">About you</label>
        <p className={fieldHintClass}>Job, hobbies etc. Whatever you like</p>
          <textarea className ={fieldInputClass}
            rows={isEnlarged ? 12 : 3}
            {...register("about")}
            id="about"
          />
          {errors.about && (<div className="text-xs text-error">{errors.about.message}</div>)}
          </div>         

         <div className="flex flex-col gap-1">
        <label className={fieldTitleClass} htmlFor="history">History</label>
        <p className={fieldHintClass}>Ex guilds, why you left, etc</p>
        <textarea className={fieldInputClass}
        rows={isEnlarged ? 12 : 3}
          {...register("history")}
          id="history"
        />
        {errors.history && (<div className="text-xs text-error">{errors.history.message}</div>)}
      </div>
   
        <div className="flex flex-col gap-1">
        <label className={fieldTitleClass} htmlFor="extra">Extra</label>
        <p className={fieldHintClass}>Anything extra? If you play other alts, here&apos;s the place</p>
          <textarea className ={fieldInputClass}
          rows={isEnlarged ? 12 : 3}
          {...register("extra")}
          id="extra"
        />
          {errors.extra && (<div className="text-xs text-error">{errors.extra.message}</div>)}
      </div>    

    </div>


  </div>

<>
<div className={"md:col-span-2 flex justify-center"}>
    <button className={appFormButtonClass} disabled={isSubmitting} type="submit">
        {isSubmitting ? "Submitting..." : "Submit"}
    </button>
    </div>

</>
  </form>
  )
}




