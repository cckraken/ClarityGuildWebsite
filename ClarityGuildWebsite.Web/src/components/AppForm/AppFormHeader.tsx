import clarityLogo from "../../assets/clarity-logo.png";

export function AppFormHeader() {
  return (
    <header className="flex items-center gap-3 rounded-xl bg-clarity-blue-deep px-6 py-4 text-white shadow-lg">
      <img
        src={clarityLogo}
        alt=""
        className="size-8 rounded-lg ring-2 ring-white/30"
      />
      <div>
        <p className="font-display text-sm text-on-brand-muted">Clarity Guild Application</p>
      </div>
    </header>
  );
}