import clarityLogo from "../../assets/clarity-logo.png";

export function AppFormHeader() {
  return (
    <header className="flex items-center gap-3 rounded-xl px-4 py-2 shadow-lg bg-cards border-2 border-clarity-blue/80 text-titles">
      <img
        src={clarityLogo}
        alt=""
        className="size-8 rounded-lg ring-2 ring-white/30"
      />
      <div>
        <p className="font-titles text-md">Clarity - Tarren Mill</p>
      </div>
    </header>
  );
}