import { useForm, type SubmitHandler } from "react-hook-form";
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from "zod";
import Dropdown, { type Option } from 'react-dropdown'
import './App.css'

const schema = z.object({
  discordId: z.string().min(1, { error: "Discord ID is required" }),
  wowclass: z.string().min(1, { error: "Class is required" }),
  wowspecs: z.string().min(1, { error: "Specs is required" }),
  age: z.preprocess((val) => (val === "" ? undefined : val), z.coerce.number().int().positive().optional()),
  country: z.string().min(1, { error: "Country is required" }),
  warcraftlogslink: z.string().min(1, { error: "Valid Warcraft Logs link is required" }).refine((value) => value.includes("www.warcraftlogs.com/character/"), {
    message: "Valid Warcraft Logs link is required",
  }),
  tech: z.string().min(1, { error: "Tech is required" }),
  schedule: z.string().min(1, { error: "Schedule is required" }),
  splits: z.enum(["true", "false"], { error: "Please select an option" }).transform((val) => val === "true"),
  communication: z.string().min(1, { error: "Communication is required" }),
  history: z.string().min(1, { error: "History is required" }),
  screenshot: z.url({ error: "Valid screenshot link is required" }),
  vouch: z.string().optional(),
  goals: z.enum(["true", "false"], { error: "Please select an option" }).transform((val) => val === "true"),
});


function App() {
  const {
     register,
      handleSubmit,
       formState: { errors, isSubmitting },
       } = useForm<z.input<typeof schema>,
        unknown, z.output<typeof schema>>({ resolver: zodResolver(schema),
          defaultValues: {
            wowclass: '',
          },
        });

        // #region wowclassOptions
        const wowclassOptions: Option[] = [
          { value: '', label: ''},
          { value: 'Death Knight', label: 'Death Knight' },
          { value: 'Demon Hunter', label: 'Demon Hunter' },
          { value: 'Druid', label: 'Druid' },
          { value: 'Hunter', label: 'Hunter' },
          { value: 'Mage', label: 'Mage' },
          { value: 'Monk', label: 'Monk' },
          { value: 'Paladin', label: 'Paladin' },
          { value: 'Priest', label: 'Priest' },
          { value: 'Rogue', label: 'Rogue' },
          { value: 'Shaman', label: 'Shaman' },
          { value: 'Warlock', label: 'Warlock' },
          { value: 'Warrior', label: 'Warrior' },
        ];
        //#endregion

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

  return (
    <>

<div className="bg-clarity-blue font-display font-light ">
    <h1>Clarity Guild Application</h1>
      </div>

    <form className="flex flex-col gap-6 p-1.5" onSubmit={handleSubmit(onSubmit)}>

      <div className="font-display font-light">
        <label htmlFor="discordId">Discord ID:</label>
      <input
        {...register('discordId') }
        type="text"
        id="discordId"
      />
      {errors.discordId && (<div className="formRedError">{errors.discordId.message}</div>)}
      </div>

<div className="flex flex-col sm:flex-row gap-4 font-display font-light my-1.5">
      <div className="font-display font-light flex-col">
        <label htmlFor="wowclass">Class:</label>
        <select
          {...register('wowclass')}
          id="wowclass"
        >
          {wowclassOptions.map((option) => (
            <option key={String(option.value)} value={String(option.value)}>
              {option.label}
            </option>
          ))}
        </select>
        {errors.wowclass && (<div className="formRedError">{errors.wowclass.message}</div>)}
         </div>

      <div className="font-display font-light">
        <label htmlFor="wowspecs">Specs:</label>
        <input
          {...register("wowspecs")}
          type="text"
          id="wowspecs"
        />
        {errors.wowspecs && (<div className="formRedError">{errors.wowspecs.message}</div>)}
      </div>
      </div>

      <div className="font-display font-light">
        <label htmlFor="age">Age:</label>
        <input
          {...register("age") }
          type="number"
          id="age"
        />    
      {errors.age && (<div className="formRedError">{errors.age.message}</div>)}
      </div>

<div className="font-display font-light">
      <label htmlFor="country">Country:</label>
      <input
        {...register("country")}
        type="text"
        id="country"
      />
      {errors.country && (<div className="formRedError">{errors.country.message}</div>)}
      </div>

      <div className="font-display font-light">
        <label htmlFor="warcraftlogslink">Warcraft Logs Link:</label>
        <textarea
          {...register("warcraftlogslink")}
          id="warcraftlogslink"
        />
        {errors.warcraftlogslink && (<div className="formRedError">{errors.warcraftlogslink.message}</div>)}
      </div>

      <div className="font-display font-light">
        <label htmlFor="tech">Tech:</label>
        <textarea
          {...register("tech")}
          id="tech"
        />
        {errors.tech && (<div className="formRedError">{errors.tech.message}</div>)}
      </div>

      <div className="font-display font-light">
        <label htmlFor="schedule">Schedule:</label>
        <input
          {...register("schedule") }
          type="text"
          id="schedule"
        />
        {errors.schedule && (<div className="formRedError">{errors.schedule.message}</div>)}
      </div>

      <div className="font-display font-light">
        <p> Can you attend our splits?</p>
        <label htmlFor="splitsYes">Yes:</label>
        <input
          {...register("splits")}
          type="radio"
          id="splitsYes"
          value="true"
        />

        <label htmlFor="splitsNo">No:</label>
        <input
          {...register("splits")}
          type="radio"
          id="splitsNo"
          value="false"
        />
        {errors.splits && (<div className="formRedError">{errors.splits.message}</div>)}
      </div>

<div className="font-display font-light">
      <label htmlFor="communication">Communication:</label>
      <textarea
        {...register("communication")}
        id="communication"
      />
      {errors.communication && (<div className="formRedError">{errors.communication.message}</div>)}
      </div>

      <div className="font-display font-light">
        <label htmlFor="history">History:</label>
        <textarea
          {...register("history")}
          id="history"
        />
        {errors.history && (<div className="formRedError">{errors.history.message}</div>)}
      </div>

      <div className="font-display font-light">
        <label htmlFor="screenshot">Screenshot:</label>
        <input
          {...register("screenshot")}
          type="text"
          id="screenshot"
        />
        {errors.screenshot && (<div className="formRedError">{errors.screenshot.message}</div>)}
      </div>

<div className="font-display font-light">
      <label htmlFor="vouch">Vouch:</label>
      <input
        {...register("vouch")}
        type="text"
        id="vouch"
      />
      {errors.vouch && (<div className="formRedError">{errors.vouch.message}</div>)}
      </div>

<div className="font-display font-light">
      <p> Do you agree with our guild goals?</p>
      <label htmlFor="goalsYes">Yes:</label>
      <input
        {...register("goals")}
        type="radio"
        id="goalsYes"
        value="true"
      />

      <label htmlFor="goalsNo">No:</label>
      <input
        {...register("goals")}
        type="radio"
        id="goalsNo"
        value="false"
      />
      {errors.goals && (<div className="formRedError">{errors.goals.message}</div>)}
      </div>

      <button disabled={isSubmitting} type="submit">
        {isSubmitting ? "Submitting..." : "Submit"}
      </button>
    </form>
    </>
  )
}

export default App


