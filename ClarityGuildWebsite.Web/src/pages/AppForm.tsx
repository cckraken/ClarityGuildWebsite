import { useForm, type SubmitHandler } from "react-hook-form";
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from "zod";
import "../index.css";
import { useState, useRef } from 'react'
import { cardClass, fieldTitleClasses, fieldSubTitleClasses, fieldHintClasses, appFormButtonClass } from "../components/styles/applicationClasses";

const roles = ["Tank", "Healer", "DPS"] as const;

const schema = z.object({
  discordId: z.string().min(1, { error: "Discord ID is required" }).max(50, {error: "50 characters or less"}),
  mainName: z.string().min(1, { error: "Main is required" }).max(50, {error: "50 characters or less"}),
  mainRealm: z.string().min(1, { error: "Realm is required" }).max(50, {error: "50 characters or less"}),
  mainRole: z.enum(roles, { error: "Role is required" }),
  altName: z.string().min(1, { error: "Alt is required" }).max(50, {error: "50 characters or less"}),
  altRealm: z.string().min(1, { error: "Realm is required" }).max(50, {error: "50 characters or less"}),
  altRole: z.enum(roles, { error: "Role is required" }),
  screenshot: z.url({error:"Screenshot is required"}).refine((val) => !val.toLowerCase().includes("imgur"), {error: "No Imgur links, sorry!"}).max(100, {error: "Too long!"}),
  about: z.string().min(1, {error: "About is required"}).max(2000, { error: "2k characters or less"}),
  tech: z.string().min(1, { error: "Tech is required" }).max(500, {error: "500 characters or less"}),
  history: z.string().min(1,  { error: "History is required" }).max(2000, {error: "2k characters or less"}),
  extra: z.string().max(2000, {error: "2k characters or less"}).optional(),
});


function AppForm() {
  const {
     register,
      handleSubmit,
       formState: { errors, isSubmitting },
       } = useForm<z.input<typeof schema>,
        unknown, z.output<typeof schema>>({ resolver: zodResolver(schema),

        });

        const [isEnlarged, setIsEnlarged] = useState(false);
        const textAreaGridClasses = isEnlarged
        ? "grid grid-cols-1 gap-4"
        : "grid grid-cols-1 gap-4 md:grid-cols-1"


  const onSubmit: SubmitHandler<z.infer<typeof schema>> = async (data) => {
    console.log(data);
  try {
    const response = await fetch(
      'http://localhost:5055/api/guildapplication',
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      }
    );

    if (response.ok) {
      alert('Application submitted successfully!');
    } else {
      alert(
        'Failed to submit application. Please contact cckraken17 on Discord for assistance.'
      );
    }
  } catch {
    alert(
      'An error occurred while submitting the application. Please try again later.'
    );
  }
  };

  const textAreasRef = useRef<HTMLDivElement>(null);

  const fieldInputClasses = "w-full rounded-md border-input border-2 bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error";

  return (
    <>

<form className="grid grid-cols-1 gap-6 font-light md:grid-cols-2" onSubmit={handleSubmit(onSubmit)}>

 <div className={`self-start ${cardClass}`}>

    <div className="flex flex-col gap-1">
    <h1 className={fieldTitleClasses}>Your Details</h1>
    <p className={fieldHintClasses}>The basics we need to find you in-game and check you out.</p>
    </div>

      <div className="flex flex-col gap-1">
        <label className={fieldSubTitleClasses} htmlFor="discordId">Discord ID</label>
        <p className={fieldHintClasses}>Our primary way to contact you</p>
        <input className={fieldInputClasses}
          {...register('discordId') }
          type="text"
          id="discordId"
          />
      {errors.discordId && (<div className="text-xs text-error" aria-invalid={errors.discordId ? true : undefined}>{errors.discordId.message}</div>)}
      </div>

<div className="flex flex-col gap-1">
  <p className="text-sm text-titles italic">Main character</p>
        <div className="grid grid-cols-3 gap-4">

        <div className="flex flex-col gap-1">
          <label className={fieldSubTitleClasses} htmlFor="mainName">Name</label>
            <input className={fieldInputClasses}
              {...register('mainName')}
              type="text"
              id="mainName"
              />
          {errors.mainName && (<div className="text-xs text-error" aria-invalid={errors.mainName ? true : undefined}>{errors.mainName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className={fieldSubTitleClasses} htmlFor="mainRealm">Realm</label>
              <input className={fieldInputClasses}
                {...register("mainRealm")}
                type="text"
                id="mainRealm"
              />
              {errors.mainRealm && (<div className="text-xs text-error" aria-invalid={errors.mainRealm ? true : undefined}>{errors.mainRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className={fieldSubTitleClasses} htmlFor="mainRole">Role</label>
              <select className={fieldInputClasses}
                {...register("mainRole")}
                id="mainRole"
              >
                <option value="" className={fieldInputClasses}>Select a role</option>
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
              {errors.mainRole && (<div className="text-xs text-error" aria-invalid={errors.mainRole ? true : undefined}>{errors.mainRole.message}</div>)}
          </div>

        </div>
      </div>

      <div className="flex flex-col gap-1">

        <p className="text-sm text-titles italic">Alt character</p>
        <div className="grid grid-cols-3 gap-4">

        <div className="flex flex-col gap-1">
          <label className={`sr-only ${fieldTitleClasses}`} htmlFor="altName">Alt Name</label>
            <input className={fieldInputClasses}
              {...register('altName')}
              type="text"
              id="altName"
              />
          {errors.altName && (<div className="text-xs text-error" aria-invalid={errors.altName ? true : undefined}>{errors.altName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className={`sr-only ${fieldTitleClasses}`} htmlFor="altRealm">Alt Realm</label>
              <input className={fieldInputClasses}
                {...register("altRealm")}
                type="text"
                id="altRealm"
              />
              {errors.altRealm && (<div className="text-xs text-error" aria-invalid={errors.altRealm ? true : undefined}>{errors.altRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className={`sr-only ${fieldTitleClasses}`} htmlFor="altRole">Alt Role</label>
              <select className={fieldInputClasses}
                {...register("altRole")}
                id="altRole"
              >
                <option value="" className={fieldInputClasses}>Select a role</option>
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
              {errors.altRole && (<div className="text-xs text-error" aria-invalid={errors.altRole ? true : undefined}>{errors.altRole.message}</div>)}
          </div>

        </div>
      </div>

        <div className="flex flex-col gap-1">
        <label className={fieldTitleClasses} htmlFor="screenshot">UI Screenshot</label>
          <p className={fieldHintClasses}>Preferably taken in combat</p>
        <input className={fieldInputClasses}
          {...register("screenshot") }
          type="url"
          id="screenshot"
        />
          {errors.screenshot && (<div className="text-xs text-error" aria-invalid={errors.screenshot ? true : undefined}>{errors.screenshot.message}</div>)}
      </div> 

      <div className="flex flex-col gap-1 py-1.5">
        <label className ={fieldTitleClasses} htmlFor="tech">Tech</label>
        <p className={fieldHintClasses}>System specs, internet etc</p>
          <textarea className ={fieldInputClasses}
            {...register("tech")}
            id="tech"
          />
          {errors.tech && (<div className="text-xs text-error" aria-invalid={errors.tech ? true : undefined}>{errors.tech.message}</div>)}
         </div>

</div>


    <div className={`self-start ${cardClass}`}>

    <div className="flex flex-col gap-1">
    <h1 className={fieldTitleClasses}>In your own words</h1>
      <p className={fieldHintClasses}>Let us get to know you</p>

<div role="group" aria-label="Text box size" className="self-start inline-flex rounded-md border border-border p-0.5">
          <button
            type="button"
            aria-pressed={!isEnlarged}
            onClick={() => {
            textAreasRef.current?.querySelectorAll("textarea").forEach((t) => {t.style.height = ""; t.style.width = ""});
            setIsEnlarged(false);
            }}
            className="text-sm hover:bg-clarity-blue-darkest aria-pressed:bg-clarity-blue-deep aria-pressed:text-white focus-visible:outline-2 focus-visible:outline-clarity-blue-deep xl:rounded-l-none xl:rounded-r-md xl:bg-clarity-blue/60 xl:px-3 xl:py-2 xl:shadow-lg"
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
          className="text-sm hover:bg-clarity-blue-darkest aria-pressed:bg-clarity-blue-deep aria-pressed:text-white focus-visible:outline-2 focus-visible:outline-clarity-blue-deep xl:rounded-l-none xl:rounded-r-md xl:bg-clarity-blue/60 xl:px-3 xl:py-2 xl:shadow-lg"
          >
            Large
          </button>      
      </div>

    </div>
      
        <div ref={textAreasRef} className={textAreaGridClasses}>
          
          <div className="flex flex-col gap-1">
        <label className ={fieldTitleClasses} htmlFor="about">About you</label>
        <p className={fieldHintClasses}>Job, hobbies etc. Whatever you like</p>
          <textarea className ={fieldInputClasses}
            rows={isEnlarged ? 12 : 3}
            {...register("about")}
            id="about"
          />
          {errors.about && (<div className="text-xs text-error" aria-invalid={errors.about ? true : undefined}>{errors.about.message}</div>)}
          </div>         

         <div className="flex flex-col gap-1">
        <label className={fieldTitleClasses} htmlFor="history">History</label>
        <p className={fieldHintClasses}>Ex guilds, why you left, etc</p>
        <textarea className={fieldInputClasses}
        rows={isEnlarged ? 12 : 3}
          {...register("history")}
          id="history"
        />
        {errors.history && (<div className="text-xs text-error" aria-invalid={errors.history ? true : undefined}>{errors.history.message}</div>)}
      </div>
   
        <div className="flex flex-col gap-1">
        <label className={fieldTitleClasses} htmlFor="Extra">Extra</label>
        <p className={fieldHintClasses}>Anything extra? If you play other alts, here's the place</p>
          <textarea className ={fieldInputClasses}
          rows={isEnlarged ? 12 : 3}
          {...register("extra")}
          id="extra"
        />
          {errors.extra && (<div className="text-xs text-error" aria-invalid={errors.extra ? true : undefined}>{errors.extra.message}</div>)}
      </div>    

    </div>


  </div>

    <div className ="md:col-span-2 flex justify-center">
    <button className={appFormButtonClass} disabled={isSubmitting} type="submit">
        {isSubmitting ? "Submitting..." : "Submit"}
    </button>
    </div>

  </form>
    </>
  )
}
export default AppForm;



