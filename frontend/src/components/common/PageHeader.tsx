import React from "react";
import Link from "next/link";
import { IconChevronRight } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface BreadcrumbItem {
  label: string;
  href?: string;
}

export interface PageHeaderProps {
  title: string;
  subtitle?: string;
  breadcrumbs?: BreadcrumbItem[];
  actions?: React.ReactNode;
  className?: string;
}

export function PageHeader({
  title,
  subtitle,
  breadcrumbs,
  actions,
  className,
}: PageHeaderProps) {
  return (
    <div
      className={cn("page-header", className)}
      style={{
        marginBottom: "1.5rem",
        display: "flex",
        flexDirection: "column",
        gap: "0.5rem",
      }}
    >
      {/* Breadcrumbs */}
      {breadcrumbs && breadcrumbs.length > 0 && (
        <nav aria-label="Breadcrumb">
          <ol
            style={{
              display: "flex",
              alignItems: "center",
              gap: "0.375rem",
              listStyle: "none",
              padding: 0,
              margin: 0,
              fontSize: "0.8125rem",
              color: "var(--erp-text-muted)",
            }}
          >
            {breadcrumbs.map((item, index) => {
              const isLast = index === breadcrumbs.length - 1;
              return (
                <li
                  key={item.label}
                  style={{ display: "inline-flex", alignItems: "center", gap: "0.375rem" }}
                >
                  {index > 0 && <IconChevronRight size={12} />}
                  {item.href && !isLast ? (
                    <Link
                      href={item.href}
                      style={{
                        color: "inherit",
                        textDecoration: "none",
                      }}
                      onMouseEnter={(e) => (e.currentTarget.style.color = "var(--erp-navy)")}
                      onMouseLeave={(e) => (e.currentTarget.style.color = "inherit")}
                    >
                      {item.label}
                    </Link>
                  ) : (
                    <span
                      style={{
                        fontWeight: isLast ? 600 : 400,
                        color: isLast ? "var(--erp-text-main)" : "inherit",
                      }}
                    >
                      {item.label}
                    </span>
                  )}
                </li>
              );
            })}
          </ol>
        </nav>
      )}

      {/* Title & Actions Row */}
      <div
        style={{
          display: "flex",
          alignItems: "flex-start",
          justifyContent: "space-between",
          flexWrap: "wrap",
          gap: "1rem",
        }}
      >
        <div>
          <h1
            style={{
              fontSize: "1.5rem",
              fontWeight: 700,
              color: "var(--erp-text-main)",
              margin: 0,
              letterSpacing: "-0.01em",
            }}
          >
            {title}
          </h1>
          {subtitle && (
            <p
              style={{
                fontSize: "0.875rem",
                color: "var(--erp-text-muted)",
                margin: "0.25rem 0 0 0",
              }}
            >
              {subtitle}
            </p>
          )}
        </div>

        {actions && (
          <div
            style={{
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              flexWrap: "wrap",
            }}
          >
            {actions}
          </div>
        )}
      </div>
    </div>
  );
}
