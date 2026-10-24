"use client";

import React, { createContext, useContext, useId } from "react";
import { cn } from "@/lib/utils/cn";

interface RadioGroupContextValue {
  name?: string;
  value?: string;
  onChange?: (value: string) => void;
  disabled?: boolean;
}

const RadioGroupContext = createContext<RadioGroupContextValue | null>(null);

export interface RadioProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "type"> {
  label?: React.ReactNode;
  error?: string;
  description?: string;
  labelClassName?: string;
  wrapperClassName?: string;
}

export const Radio = React.forwardRef<HTMLInputElement, RadioProps>(
  (
    {
      className,
      label,
      error,
      description,
      disabled,
      labelClassName,
      wrapperClassName,
      id,
      name,
      value,
      checked,
      onChange,
      ...props
    },
    ref
  ) => {
    const group = useContext(RadioGroupContext);
    const autoId = useId();

    const isControlledByGroup = group !== null && value !== undefined;
    const finalName = name ?? group?.name;
    const finalChecked = isControlledByGroup
      ? String(group?.value) === String(value)
      : checked;
    const finalDisabled = disabled || group?.disabled;

    const generatedId =
      id ||
      (finalName && value !== undefined
        ? `${finalName}-${String(value).replace(/\s+/g, "-").toLowerCase()}`
        : typeof label === "string"
          ? `radio-${label.replace(/\s+/g, "-").toLowerCase()}`
          : `radio-${autoId}`);

    const errorId = generatedId ? `${generatedId}-error` : undefined;

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
      if (finalDisabled) {
        return;
      }
      onChange?.(event);
      if (group?.onChange && value !== undefined) {
        group.onChange(String(value));
      }
    };

    return (
      <div className={cn("erp-form-group mb-0", wrapperClassName)}>
        <label
          htmlFor={generatedId}
          className={cn(
            "erp-radio-label min-h-11",
            finalDisabled && "erp-radio-label-disabled",
            labelClassName
          )}
        >
          <input
            id={generatedId}
            type="radio"
            name={finalName}
            value={value}
            checked={finalChecked}
            disabled={finalDisabled}
            aria-invalid={Boolean(error)}
            aria-describedby={error ? errorId : undefined}
            className={cn("erp-radio", className)}
            ref={ref}
            onChange={handleChange}
            {...props}
          />
          {(label || description) && (
            <div className="flex flex-col gap-0.5">
              {label && <span className="font-medium">{label}</span>}
              {description && (
                <span className="text-xs text-erp-text-muted">{description}</span>
              )}
            </div>
          )}
        </label>
        {error && (
          <p id={errorId} className="erp-error-text mt-1" role="alert">
            {error}
          </p>
        )}
      </div>
    );
  }
);

Radio.displayName = "Radio";

export interface RadioGroupOption<T extends string = string> {
  value: T;
  label: React.ReactNode;
  description?: string;
  disabled?: boolean;
  labelClassName?: string;
  className?: string;
}

export interface RadioGroupProps<T extends string = string> {
  name: string;
  value?: T;
  defaultValue?: T;
  onChange?: (value: T) => void;
  options?: readonly RadioGroupOption<T>[];
  children?: React.ReactNode;
  orientation?: "horizontal" | "vertical";
  label?: React.ReactNode;
  error?: string;
  disabled?: boolean;
  className?: string;
  wrapperClassName?: string;
  "aria-label"?: string;
}

export function RadioGroup<T extends string = string>({
  name,
  value,
  defaultValue,
  onChange,
  options,
  children,
  orientation = "vertical",
  label,
  error,
  disabled = false,
  className,
  wrapperClassName,
  "aria-label": ariaLabel,
}: RadioGroupProps<T>) {
  const [internalValue, setInternalValue] = React.useState<T | undefined>(
    defaultValue
  );
  const currentValue = value !== undefined ? value : internalValue;

  const handleChange = (newValue: string) => {
    if (value === undefined) {
      setInternalValue(newValue as T);
    }
    onChange?.(newValue as T);
  };

  const groupErrorId = error ? `${name}-error` : undefined;

  return (
    <RadioGroupContext.Provider
      value={{
        name,
        value: currentValue,
        onChange: handleChange,
        disabled,
      }}
    >
      <div
        role="radiogroup"
        aria-label={ariaLabel}
        aria-invalid={Boolean(error)}
        aria-describedby={groupErrorId}
        className={cn("erp-form-group mb-0", wrapperClassName)}
      >
        {label && <span className="erp-label mb-2 block">{label}</span>}
        <div
          className={cn(
            orientation === "horizontal"
              ? "flex flex-wrap gap-x-5 gap-y-2 items-center"
              : "flex flex-col gap-2",
            className
          )}
        >
          {options
            ? options.map((option) => (
                <Radio
                  key={option.value}
                  id={`${name}-${option.value}`}
                  value={option.value}
                  label={option.label}
                  description={option.description}
                  disabled={disabled || option.disabled}
                  labelClassName={option.labelClassName}
                  className={option.className}
                />
              ))
            : children}
        </div>
        {error && (
          <p id={groupErrorId} className="erp-error-text mt-1" role="alert">
            {error}
          </p>
        )}
      </div>
    </RadioGroupContext.Provider>
  );
}
