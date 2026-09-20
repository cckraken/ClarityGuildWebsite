import { useState } from 'react'
import './App.css'

interface ApplicationForm {
  discordId: string
  age: number | null
  country: string
  warcraftlogslink: string
  tech: string
  schedule: string
  splits: boolean | null
  communication: string
  history: string
  screenshot: string
  vouch: string | null
  goals: boolean | null
  
}


function App() {

const [form, setForm] = useState<ApplicationForm>({
  discordId: '',
  age: null,
  country: '',
  warcraftlogslink: '',
  tech: '',
  schedule: '',
  splits: null,
  communication: '',
  history: '',
  screenshot: '',
  vouch: '',
  goals: null
})




function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
  setForm({ ...form, [e.target.name]: e.target.value })
}

return (
  <>
  <h1>Clarity Guild Application</h1>

  <input
  type="text"
  name="discordId"
  value={form.discordId}
  onChange={handleChange}
/>
<input
  type="number"
  name="age"
  value={form.age ?? ''}
  onChange={handleChange}
/>
<input 
type="text"
name="country"
value={form.country}
onChange={handleChange}
/>
<input 
type="text"
name="warcraftlogslink"
value={form.warcraftlogslink}
onChange={handleChange}
/>
<input 
type="text"
name="tech"
value={form.tech}
onChange={handleChange}
/>
<input 
type="text"
name="schedule"
value={form.schedule}
onChange={handleChange}
/>
<input 
type="checkbox"
name="splits"
checked={form.splits ?? false}
onChange={(e) => setForm({ ...form, splits: e.target.checked })}
/>
<input 
type="text"
name="communication"
value={form.communication}
onChange={handleChange}
/>
<input 
type="text"
name="history"
value={form.history}
onChange={handleChange}
/>
<input 
type="text"
name="screenshot"
value={form.screenshot}
onChange={handleChange}
/>
<input 
type="text"
name="vouch"
value={form.vouch ?? ''}
onChange={handleChange}
/>
<input 
type="checkbox"
name="goals"
checked={form.goals ?? false}
onChange={(e) => setForm({ ...form, goals: e.target.checked })}
/>  
</>
)
}

export default App
