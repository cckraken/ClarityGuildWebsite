import { NavLink, Outlet } from "react-router";
import { AppFormHeader } from "../AppForm/AppFormHeader";

const tabClasses =
  "border-b-2 border-transparent px-1 py-2 text-sm text-muted " +
  "aria-[current=page]:border-clarity-blue-deep aria-[current=page]:font-medium aria-[current=page]:text-foreground";

export function SiteLayout() {
  return (
    <div className="mx-auto flex max-w-6xl  flex-col px-4 py-3">
      <AppFormHeader />

      <nav aria-label="Main" className="flex gap-6 border-b border-line/30">
        <NavLink to="/" end className={tabClasses}>Overview</NavLink>
        <NavLink to="/apply" className={tabClasses}>Apply</NavLink>
      </nav>

      <Outlet />
    </div>
  );
}