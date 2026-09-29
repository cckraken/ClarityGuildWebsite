import { Route, Routes } from "react-router";
import { SiteLayout } from "./components/layout/SiteLayout";
import GuildDesc from "./pages/GuildDesc";
import AppForm from "./pages/AppForm";

export default function App() {
  return (
    <Routes>
      <Route element={<SiteLayout />}>
        <Route index element={<GuildDesc />} />
        <Route path="apply" element={<AppForm />} />
      </Route>
    </Routes>
  );
}