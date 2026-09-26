import { useId } from "react";
import type { UseFormRegisterReturn } from "react-hook-form";

type RadioOption = {
  value: string;
  label: string;
};

type RadioGroupProps = {
  legend: string;
  options: RadioOption[];
  registration: UseFormRegisterReturn;
  error?: string;
};

export function AppFormRadioGroup({ legend, options, registration, error }: RadioGroupProps) {
  const errorId = useId();

  return (
    <fieldset className="border-border rounded p-3">
      <legend className="px-1 text-sm font-medium">{legend}</legend>

      <div className="flex gap-6">
        {options.map((option) => (
          <label key={option.value} className="flex items-center gap-2">
            <input className="accent-clarity-blue"
              {...registration}
              type="radio"
              value={option.value}
              aria-invalid={error ? true : undefined}
              aria-describedby={error ? errorId : undefined}
            />
            {option.label}
          </label>
        ))}
      </div>

      {error && (
        <p id={errorId} className="mt-2 text-error">
          {error}
        </p>
      )}
    </fieldset>
  );
}