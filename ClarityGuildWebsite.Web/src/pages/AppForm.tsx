import { useForm, type SubmitHandler } from "react-hook-form";
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from "zod";
import "../index.css";
import { AppFormRadioGroup } from '../components/AppForm/AppFormRadioGroup';
import { useState, useRef } from  'react'

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
  schedule: z.string().min(1, { error: "Schedule is required" }).max(500, {error: "500 characters or less"}),
  splits: z.enum(["true", "false"], { error: "Please select an option" }).transform((val) => val === "true"),
  goals: z.enum(["true", "false"], { error: "Please select an option" }).transform((val) => val === "true"),
  about: z.string().min(1, {error: "About is required"}).max(2000, { error: "2k characters or less"}),
  tech: z.string().min(1, { error: "Tech is required" }).max(500, {error: "500 characters or less"}),
  history: z.string().min(1,  { error: "History is required" }).max(2000, {error: "2k characters or less"}),
  extra: z.string().max(2000, {error: "2k characters or less"}).optional(),
});

const yesNoOptions = [
  { value: "true", label: "Yes" },
  { value: "false", label: "No" },
];


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

  return (
    <>

<form className="grid grid-cols-1 gap-6 font-light md:grid-cols-2" onSubmit={handleSubmit(onSubmit)}>

 <div className="flex flex-col gap-6 rounded-xl self-start bg-surface p-4 shadow-lg">

    <div className="flex flex-col gap-1">
    <p className="text-base font-medium text-foreground">Your Details</p>
    <p className="text-sm text-muted">The basics we need to find you in-game and check you out.</p>
    </div>

      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium" htmlFor="discordId">Discord ID</label>
        <p className="text-xs text-muted">Our primary way to contact you</p>
        <input className="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
          {...register('discordId') }
          type="text"
          id="discordId"
          />
      {errors.discordId && (<div className="text-xs text-error">{errors.discordId.message}</div>)}
      </div>

<div className="flex flex-col gap-1">
  <p className="text-sm text-muted">Main character</p>
        <div className="grid grid-cols-3 gap-4">

        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="mainName">Name</label>
            <input className="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
              {...register('mainName')}
              type="text"
              id="mainName"
              />
          {errors.mainName && (<div className="text-xs text-error">{errors.mainName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium" htmlFor="mainRealm">Realm</label>
              <input className= "w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
                {...register("mainRealm")}
                type="text"
                id="mainRealm"
              />
              {errors.mainRealm && (<div className="text-xs text-error">{errors.mainRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium" htmlFor="mainRole">Role</label>
              <select className="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
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

        <p className="text-sm text-muted ">Alt character</p>
        <div className="grid grid-cols-3 gap-4">

        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="altName">Name</label>
            <input className="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
              {...register('altName')}
              type="text"
              id="altName"
              />
          {errors.altName && (<div className="text-xs text-error">{errors.altName.message}</div>)}
        </div>
        
          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium" htmlFor="altRealm">Realm</label>
              <input className= "w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
                {...register("altRealm")}
                type="text"
                id="altRealm"
              />
              {errors.altRealm && (<div className="text-xs text-error">{errors.altRealm.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium" htmlFor="altRole">Role</label>
              <select className="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
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
        <label className = "text-sm font-medium" htmlFor="screenshot">UI Screenshot</label>
          <p className="text-xs text-muted ">Preferably taken in combat</p>
        <input className = "w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
          {...register("screenshot") }
          type="url"
          id="screenshot"
        />
          {errors.screenshot && (<div className="text-xs text-error">{errors.screenshot.message}</div>)}
      </div> 

        <div className="flex flex-col gap-1">
        <label className = "text-sm font-medium" htmlFor="schedule">Schedule</label>
          <p className="text-xs text-muted">Schedule found on other tab</p>
        <input className = "w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
          {...register("schedule") }
          type="text"
          id="schedule"
        />
          {errors.schedule && (<div className="text-xs text-error">{errors.schedule.message}</div>)}
      </div> 

      <p className="text-xs text-muted">Splits and goals info found on other tab</p> 
    <div className="flex flex-col gap-1 w-full rounded-md border border-line bg-field px-1 py-1.5 focus-visible:border-clarity-blue focus-within:ring-3"> 
    <div className="grid grid-cols-2">
      <AppFormRadioGroup 
        legend="Can you attend our splits?"
        options={yesNoOptions}
        registration={register("splits")}
        error={errors.splits?.message}
      />
      <AppFormRadioGroup
        legend="Do our goals align with yours?"
        options={yesNoOptions}
        registration={register("goals")}
        error={errors.goals?.message}
      />
     </div>
     </div>
</div>


    <div className="relative flex flex-col gap-6 rounded-xl bg-surface p-4 shadow-lg">

    <div className="flex flex-col gap-1">
    <p className="text-base font-medium text-foreground">In your own words</p>
      <p className="text-sm text-muted">Let us get to know you</p>
    </div>

    <div role="group" aria-label="Text box size" className="self-start inline-flex rounded-md border border-line p-0.5 xl:absolute xl:left-full xl:top-16 xl:flex-col xl:gap-1 xl:border-0 xl:p-0">
          <button
            type="button"
            aria-pressed={!isEnlarged}
            onClick={() => {
            textAreasRef.current?.querySelectorAll("textarea").forEach((t) => {t.style.height = ""; t.style.width = ""});
            setIsEnlarged(false);
            }}
            className="rounded px-3 py-1 text-sm aria-pressed:bg-clarity-blue-deep aria-pressed:text-white focus-visible:outline-2 focus-visible:outline-clarity-blue-deep xl:rounded-l-none xl:rounded-r-md xl:bg-surface xl:px-3 xl:py-2 xl:shadow-lg"
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
          className="rounded px-3 py-1 text-sm aria-pressed:bg-clarity-blue-deep aria-pressed:text-white focus-visible:outline-2 focus-visible:outline-clarity-blue-deep xl:rounded-l-none xl:rounded-r-md xl:bg-surface xl:px-3 xl:py-2 xl:shadow-lg"
          >
            Large
          </button>      
      </div>
      
        <div ref={textAreasRef} className={textAreaGridClasses}>
          
          <div className="flex flex-col gap-1">
        <label className ="text-sm font-medium" htmlFor="about">About you</label>
        <p className="text-xs text-muted">Job, hobbies etc. Whatever you like</p>
          <textarea className ="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
            rows={isEnlarged ? 12 : 3}
            {...register("about")}
            id="about"
          />
          {errors.about && (<div className="text-xs text-error">{errors.about.message}</div>)}
          </div>

          <div className="flex flex-col gap-1">
        <label className ="text-sm font-medium" htmlFor="tech">Tech</label>
        <p className="text-xs text-muted">System specs, internet etc</p>
          <textarea className ="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
            rows={isEnlarged ? 12 : 3}
            {...register("tech")}
            id="tech"
          />
          {errors.tech && (<div className="text-xs text-error">{errors.tech.message}</div>)}
         </div>

         <div className="flex flex-col gap-1">
        <label className="text-sm font-medium" htmlFor="history">History</label>
        <p className="text-xs text-muted">Ex guilds, why you left, etc</p>
        <textarea className="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
        rows={isEnlarged ? 12 : 3}
          {...register("history")}
          id="history"
        />
        {errors.history && (<div className="text-xs text-error">{errors.history.message}</div>)}
      </div>
   
        <div className="flex flex-col gap-1">
        <label className="text-sm font-medium" htmlFor="Extra">Extra</label>
        <p className="text-xs text-muted">Anything extra? If you play other alts, here's the place</p>
          <textarea className ="w-full rounded-md border border-line bg-field px-3 py-1.5 outline-none focus-visible:border-clarity-blue focus-visible:ring-3 focus-visible:ring-clarity-blue/20 aria-invalid:border-error"
          rows={isEnlarged ? 12 : 3}
          {...register("extra")}
          id="Extra"
        />
          {errors.extra && (<div className="text-xs text-error">{errors.extra.message}</div>)}
      </div>    

    </div>


  </div>

    <div className = "md:col-span-2 flex justify-center focus-visible:outline-clarity-blue-darkest">
    <button className= "rounded-md bg-clarity-blue-deep px-6 py-2 font-medium text-white hover:bg-clarity-blue-darkest focus-within:outline-2 focus-within:outline-offset-2 focus-within:outline-accent disabled:cursor-not-allowed disabled:opacity-50" disabled={isSubmitting} type="submit">
        {isSubmitting ? "Submitting..." : "Submit"}
    </button>
    </div>

  </form>
    </>
  )
}

export default AppForm


