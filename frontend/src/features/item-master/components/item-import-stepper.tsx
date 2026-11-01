import { IconCheck } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

interface ItemImportStepperProps {
  ariaLabel: string;
  labels: readonly [string, string, string];
  /** 1-based index of the step the user is on. */
  current: 1 | 2 | 3;
}

/** Shows where the user is in choose file → review → confirm. */
export function ItemImportStepper({ ariaLabel, labels, current }: ItemImportStepperProps) {
  return (
    <ol aria-label={ariaLabel} className="flex items-center gap-2">
      {labels.map((label, index) => {
        const step = index + 1;
        const state = step < current ? "done" : step === current ? "current" : "todo";
        return (
          <li key={label} aria-current={state === "current" ? "step" : undefined} className="flex flex-1 items-center gap-2 text-xs">
            <span
              className={cn(
                "flex h-6 w-6 shrink-0 items-center justify-center border text-xs font-semibold",
                state === "current" && "border-erp-navy bg-erp-navy text-white",
                state === "done" && "border-erp-success-border bg-erp-success-bg text-erp-success",
                state === "todo" && "border-erp-border text-erp-text-muted",
              )}
            >
              {state === "done" ? <IconCheck size={13} /> : step}
            </span>
            <span className={cn(state === "current" ? "font-semibold text-erp-text-main" : "text-erp-text-muted")}>{label}</span>
          </li>
        );
      })}
    </ol>
  );
}
