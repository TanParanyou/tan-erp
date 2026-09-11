import React from "react";

export interface LogoIconProps extends React.SVGProps<SVGSVGElement> {
  size?: number | string;
  strokeWidth?: number;
  className?: string;
  color?: string;
}

/**
 * Geometric architectural grid logo for Tan ERP
 * Atelier Architectural Navy Sharp identity (sharp corners, precision lines)
 */
export function LogoIcon({
  size = 24,
  strokeWidth = 2,
  className,
  color = "currentColor",
  ...props
}: LogoIconProps) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      stroke={color}
      strokeWidth={strokeWidth}
      strokeLinecap="square"
      strokeLinejoin="miter"
      aria-hidden="true"
      className={className}
      {...props}
    >
      <rect x="3" y="3" width="18" height="18" />
      <line x1="3" y1="9" x2="21" y2="9" />
      <line x1="9" y1="21" x2="9" y2="9" />
    </svg>
  );
}

export interface LogoProps {
  /** Size of the logo icon */
  iconSize?: number;
  /** Custom class for the wrapper */
  className?: string;
  /** Whether to display the text next to icon */
  showText?: boolean;
  /** Color theme variant */
  variant?: "light" | "dark" | "navy";
}

/**
 * Reusable full Brand Logo component for Tan ERP
 */
export function Logo({
  iconSize = 26,
  className = "",
  showText = true,
  variant = "navy",
}: LogoProps) {
  const isLight = variant === "light";
  const iconColor = isLight ? "#FFFFFF" : "#0B3056";
  const textColor = isLight ? "#FFFFFF" : "var(--erp-navy)";
  const subtitleColor = isLight ? "#94A3B8" : "var(--erp-text-muted)";

  return (
    <div
      className={`inline-flex items-center gap-2 select-none ${className}`}
      style={{ display: "inline-flex", alignItems: "center", gap: "0.5rem" }}
    >
      <LogoIcon size={iconSize} color={iconColor} strokeWidth={2} />
      {showText && (
        <div style={{ display: "flex", flexDirection: "column", lineHeight: 1.1 }}>
          <span
            style={{
              fontSize: "0.625rem",
              fontWeight: 700,
              letterSpacing: "0.08em",
              textTransform: "uppercase",
              color: subtitleColor,
            }}
          >
            ATELIER ARCHITECTURAL
          </span>
          <span
            style={{
              fontSize: "1rem",
              fontWeight: 800,
              letterSpacing: "-0.01em",
              color: textColor,
            }}
          >
            TAN ERP
          </span>
        </div>
      )}
    </div>
  );
}
