import {
  useWatch,
  type Control,
  type FieldPath,
  type FieldValues,
} from "react-hook-form";

type TextAreaFooterProps<TInput extends FieldValues, TOutput> = {
  control: Control<TInput, unknown, TOutput>;
  name: FieldPath<TInput>;
  max: number;
  error?: string;
};

export function TextAreaFooter<TInput extends FieldValues, TOutput>({
  control,
  name,
  max,
  error,
}: TextAreaFooterProps<TInput, TOutput>) {
  const value = useWatch({ control, name });
  const length = typeof value === "string" ? value.length : 0;

  return (
    <div className="flex items-start gap-2">
      {error && (
        <div id={`${name}-error`} className="min-w-0 text-xs text-error">
          {error}
        </div>
      )}
      <span
        className={`ml-auto shrink-0 text-xs ${length >= max ? "text-error" : "text-hints"}`}
      >
        {length} / {max}
      </span>
    </div>
  );
}
