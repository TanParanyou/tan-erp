import React from "react";

export interface IconProps extends React.SVGProps<SVGSVGElement> {
  size?: number | string;
  className?: string;
  strokeWidth?: number;
}

const baseProps = {
  viewBox: "0 0 24 24",
  fill: "none",
  stroke: "currentColor",
  strokeLinecap: "square" as const,
  strokeLinejoin: "miter" as const,
  "aria-hidden": true,
};

export function IconPlus({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="12" y1="5" x2="12" y2="19" />
      <line x1="5" y1="12" x2="19" y2="12" />
    </svg>
  );
}

export function IconTrash({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="3 6 5 6 21 6" />
      <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
      <line x1="10" y1="11" x2="10" y2="17" />
      <line x1="14" y1="11" x2="14" y2="17" />
    </svg>
  );
}

export function IconEdit({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
      <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
    </svg>
  );
}

export function IconEye({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
      <circle cx="12" cy="12" r="3" />
    </svg>
  );
}

export function IconEyeOff({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
      <line x1="1" y1="1" x2="23" y2="23" />
    </svg>
  );
}

export function IconDownload({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
      <polyline points="7 10 12 15 17 10" />
      <line x1="12" y1="15" x2="12" y2="3" />
    </svg>
  );
}

export function IconSave({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z" />
      <polyline points="17 21 17 13 7 13 7 21" />
      <polyline points="7 3 7 8 15 8" />
    </svg>
  );
}

export function IconClose({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="18" y1="6" x2="6" y2="18" />
      <line x1="6" y1="6" x2="18" y2="18" />
    </svg>
  );
}

export function IconCheck({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="20 6 9 17 4 12" />
    </svg>
  );
}

export function IconSearch({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="11" cy="11" r="8" />
      <line x1="21" y1="21" x2="16.65" y2="16.65" />
    </svg>
  );
}

export function IconFilter({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3" />
    </svg>
  );
}

export function IconChevronLeft({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="15 18 9 12 15 6" />
    </svg>
  );
}

export function IconChevronRight({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="9 18 15 12 9 6" />
    </svg>
  );
}

export function IconChevronsLeft({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="11 17 6 12 11 7" />
      <polyline points="18 17 13 12 18 7" />
    </svg>
  );
}

export function IconChevronsRight({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="13 17 18 12 13 7" />
      <polyline points="6 17 11 12 6 7" />
    </svg>
  );
}

export function IconArrowUpDown({ size = 16, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="7 15 12 20 17 15" />
      <polyline points="7 9 12 4 17 9" />
    </svg>
  );
}

export function IconArrowUp({ size = 16, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="12" y1="19" x2="12" y2="5" />
      <polyline points="5 12 12 5 19 12" />
    </svg>
  );
}

export function IconArrowDown({ size = 16, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="12" y1="5" x2="12" y2="19" />
      <polyline points="19 12 12 19 5 12" />
    </svg>
  );
}

export function IconAlertTriangle({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
      <line x1="12" y1="9" x2="12" y2="13" />
      <line x1="12" y1="17" x2="12.01" y2="17" />
    </svg>
  );
}

export function IconAlertCircle({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="12" cy="12" r="10" />
      <line x1="12" y1="8" x2="12" y2="12" />
      <line x1="12" y1="16" x2="12.01" y2="16" />
    </svg>
  );
}

export function IconCheckCircle({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
      <polyline points="22 4 12 14.01 9 11.01" />
    </svg>
  );
}

export function IconInfo({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="12" cy="12" r="10" />
      <line x1="12" y1="16" x2="12" y2="12" />
      <line x1="12" y1="8" x2="12.01" y2="8" />
    </svg>
  );
}

export function IconFileText({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
      <polyline points="14 2 14 8 20 8" />
      <line x1="16" y1="13" x2="8" y2="13" />
      <line x1="16" y1="17" x2="8" y2="17" />
      <polyline points="10 9 9 9 8 9" />
    </svg>
  );
}

export function IconPrinter({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="6 9 6 2 18 2 18 9" />
      <path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2" />
      <rect x="6" y="14" width="12" height="8" />
    </svg>
  );
}

export function IconRefresh({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="23 4 23 10 17 10" />
      <polyline points="1 20 1 14 7 14" />
      <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15" />
    </svg>
  );
}

export function IconSpinner({ size = 18, strokeWidth = 2, className, ...props }: IconProps) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth={strokeWidth}
      className={`animate-spin ${className || ""}`}
      style={{ animation: "erp-spin 1s linear infinite" }}
      aria-hidden="true"
      {...props}
    >
      <style>{`@keyframes erp-spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }`}</style>
      <circle cx="12" cy="12" r="10" strokeOpacity="0.25" />
      <path d="M12 2a10 10 0 0 1 10 10" strokeLinecap="square" />
    </svg>
  );
}

export function IconSun({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="12" cy="12" r="5" />
      <line x1="12" y1="1" x2="12" y2="3" />
      <line x1="12" y1="21" x2="12" y2="23" />
      <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
      <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
      <line x1="1" y1="12" x2="3" y2="12" />
      <line x1="21" y1="12" x2="23" y2="12" />
      <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
      <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
    </svg>
  );
}

export function IconMoon({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
    </svg>
  );
}

export function IconMenu({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="3" y1="6" x2="21" y2="6" />
      <line x1="3" y1="12" x2="21" y2="12" />
      <line x1="3" y1="18" x2="21" y2="18" />
    </svg>
  );
}

export function IconHome({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
      <polyline points="9 22 9 12 15 12 15 22" />
    </svg>
  );
}

export function IconLogOut({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
      <polyline points="16 17 21 12 16 7" />
      <line x1="21" y1="12" x2="9" y2="12" />
    </svg>
  );
}

export function IconGlobe({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="12" cy="12" r="10" />
      <line x1="2" y1="12" x2="22" y2="12" />
      <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z" />
    </svg>
  );
}
