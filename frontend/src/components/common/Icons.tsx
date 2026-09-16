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

export function IconUsers({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
      <path d="M16 3.13a4 4 0 0 1 0 7.75" />
    </svg>
  );
}

export function IconCopy({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="9" y="9" width="13" height="13" />
      <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1" />
    </svg>
  );
}

export function IconUpload({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
      <polyline points="17 8 12 3 7 8" />
      <line x1="12" y1="3" x2="12" y2="15" />
    </svg>
  );
}

export function IconCalendar({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="3" y="4" width="18" height="18" />
      <line x1="16" y1="2" x2="16" y2="6" />
      <line x1="8" y1="2" x2="8" y2="6" />
      <line x1="3" y1="10" x2="21" y2="10" />
    </svg>
  );
}

export function IconClock({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="12" cy="12" r="10" />
      <polyline points="12 6 12 12 16 14" />
    </svg>
  );
}

export function IconLock({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="3" y="11" width="18" height="11" />
      <path d="M7 11V7a5 5 0 0 1 10 0v4" />
    </svg>
  );
}

export function IconUnlock({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="3" y="11" width="18" height="11" />
      <path d="M7 11V7a5 5 0 0 1 9.9-1" />
    </svg>
  );
}

export function IconArrowLeft({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="19" y1="12" x2="5" y2="12" />
      <polyline points="12 19 5 12 12 5" />
    </svg>
  );
}

export function IconArrowRight({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="5" y1="12" x2="19" y2="12" />
      <polyline points="12 5 19 12 12 19" />
    </svg>
  );
}

export function IconGripVertical({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="9" cy="12" r="1" />
      <circle cx="9" cy="5" r="1" />
      <circle cx="9" cy="19" r="1" />
      <circle cx="15" cy="12" r="1" />
      <circle cx="15" cy="5" r="1" />
      <circle cx="15" cy="19" r="1" />
    </svg>
  );
}

export function IconMapPin({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z" />
      <circle cx="12" cy="10" r="3" />
    </svg>
  );
}

export function IconBriefcase({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="2" y="7" width="20" height="14" rx="0" ry="0" />
      <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16" />
    </svg>
  );
}

export function IconPhone({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
    </svg>
  );
}

export function IconChevronDown({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <polyline points="6 9 12 15 18 9" />
    </svg>
  );
}

export function IconGrid({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="3" y="3" width="7" height="7" />
      <rect x="14" y="3" width="7" height="7" />
      <rect x="14" y="14" width="7" height="7" />
      <rect x="3" y="14" width="7" height="7" />
    </svg>
  );
}

export function IconList({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="8" y1="6" x2="21" y2="6" />
      <line x1="8" y1="12" x2="21" y2="12" />
      <line x1="8" y1="18" x2="21" y2="18" />
      <line x1="3" y1="6" x2="3.01" y2="6" />
      <line x1="3" y1="12" x2="3.01" y2="12" />
      <line x1="3" y1="18" x2="3.01" y2="18" />
    </svg>
  );
}

export function IconUser({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
      <circle cx="12" cy="7" r="4" />
    </svg>
  );
}

export function IconBuilding({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="4" y="2" width="16" height="20" />
      <line x1="9" y1="22" x2="9" y2="18" />
      <line x1="15" y1="22" x2="15" y2="18" />
      <line x1="9" y1="18" x2="15" y2="18" />
      <line x1="8" y1="6" x2="10" y2="6" />
      <line x1="14" y1="6" x2="16" y2="6" />
      <line x1="8" y1="10" x2="10" y2="10" />
      <line x1="14" y1="10" x2="16" y2="10" />
      <line x1="8" y1="14" x2="10" y2="14" />
      <line x1="14" y1="14" x2="16" y2="14" />
    </svg>
  );
}

export function IconGitBranch({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <line x1="6" y1="3" x2="6" y2="15" />
      <circle cx="18" cy="6" r="3" />
      <circle cx="6" cy="18" r="3" />
      <path d="M18 9a9 9 0 0 1-9 9" />
    </svg>
  );
}

export function IconBox3D({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z" />
      <polyline points="3.27 6.96 12 12.01 20.73 6.96" />
      <line x1="12" y1="22.08" x2="12" y2="12" />
    </svg>
  );
}

export function IconZoomIn({ size = 16, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="11" cy="11" r="8" />
      <line x1="21" y1="21" x2="16.65" y2="16.65" />
      <line x1="11" y1="8" x2="11" y2="14" />
      <line x1="8" y1="11" x2="14" y2="11" />
    </svg>
  );
}

export function IconZoomOut({ size = 16, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <circle cx="11" cy="11" r="8" />
      <line x1="21" y1="21" x2="16.65" y2="16.65" />
      <line x1="8" y1="11" x2="14" y2="11" />
    </svg>
  );
}

export function IconMaximize({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3" />
    </svg>
  );
}

export function IconMinimize({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M4 14h6m0 0v6m0-6L3 21m17-7h-6m0 0v6m0-6l7 7M4 10h6m0 0V4m0 6L3 3m17 7h-6m0 0V4m0 6l7-7" />
    </svg>
  );
}

export function IconRotateCw({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M21 12a9 9 0 1 1-9-9c2.52 0 4.85.99 6.57 2.6L21 8" />
      <polyline points="21 3 21 8 16 8" />
    </svg>
  );
}

export function IconRotateCcw({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M3 12a9 9 0 1 0 9-9 8.95 8.95 0 0 0-6.57 2.6L3 8" />
      <polyline points="3 3 3 8 8 8" />
    </svg>
  );
}

export function IconCamera({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z" />
      <circle cx="12" cy="13" r="4" />
    </svg>
  );
}

export function IconImage({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <rect x="3" y="3" width="18" height="18" />
      <circle cx="8.5" cy="8.5" r="1.5" />
      <polyline points="21 15 16 10 5 21" />
    </svg>
  );
}

export function IconExternalLink({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6" />
      <polyline points="15 3 21 3 21 9" />
      <line x1="10" y1="14" x2="21" y2="3" />
    </svg>
  );
}
