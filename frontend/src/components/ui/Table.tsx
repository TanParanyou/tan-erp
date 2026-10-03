import * as React from "react";
import { cn } from "@/lib/utils/cn";

export interface TableProps extends React.HTMLAttributes<HTMLTableElement> {
  wrapperClassName?: string;
}

export const Table = React.forwardRef<HTMLTableElement, TableProps>(
  ({ className, wrapperClassName, ...props }, ref) => (
    <div className={cn("erp-table-wrapper w-full overflow-x-auto", wrapperClassName)}>
      <table
        ref={ref}
        className={cn("erp-table w-full border-collapse text-left text-sm", className)}
        {...props}
      />
    </div>
  )
);
Table.displayName = "Table";

export interface TableHeaderProps extends React.HTMLAttributes<HTMLTableSectionElement> {}

export const TableHeader = React.forwardRef<HTMLTableSectionElement, TableHeaderProps>(
  ({ className, ...props }, ref) => (
    <thead
      ref={ref}
      className={cn("bg-erp-surface-subtle border-b border-erp-border text-xs font-semibold text-erp-text-main", className)}
      {...props}
    />
  )
);
TableHeader.displayName = "TableHeader";

export interface TableBodyProps extends React.HTMLAttributes<HTMLTableSectionElement> {}

export const TableBody = React.forwardRef<HTMLTableSectionElement, TableBodyProps>(
  ({ className, ...props }, ref) => (
    <tbody
      ref={ref}
      className={cn("divide-y divide-erp-border bg-erp-surface", className)}
      {...props}
    />
  )
);
TableBody.displayName = "TableBody";

export interface TableFooterProps extends React.HTMLAttributes<HTMLTableSectionElement> {}

export const TableFooter = React.forwardRef<HTMLTableSectionElement, TableFooterProps>(
  ({ className, ...props }, ref) => (
    <tfoot
      ref={ref}
      className={cn("border-t border-erp-border bg-erp-surface-subtle font-medium text-erp-text-main", className)}
      {...props}
    />
  )
);
TableFooter.displayName = "TableFooter";

export interface TableRowProps extends React.HTMLAttributes<HTMLTableRowElement> {
  selected?: boolean;
}

export const TableRow = React.forwardRef<HTMLTableRowElement, TableRowProps>(
  ({ className, selected, ...props }, ref) => (
    <tr
      ref={ref}
      className={cn(
        "border-b border-erp-border transition-colors hover:bg-erp-surface-subtle",
        selected && "bg-erp-navy-light",
        className
      )}
      {...props}
    />
  )
);
TableRow.displayName = "TableRow";

export interface TableHeadProps extends React.ThHTMLAttributes<HTMLTableCellElement> {}

export const TableHead = React.forwardRef<HTMLTableCellElement, TableHeadProps>(
  ({ className, ...props }, ref) => (
    <th
      ref={ref}
      className={cn(
        "p-3 text-left font-semibold text-erp-text-main align-middle text-xs whitespace-nowrap",
        className
      )}
      {...props}
    />
  )
);
TableHead.displayName = "TableHead";

export interface TableCellProps extends React.TdHTMLAttributes<HTMLTableCellElement> {}

export const TableCell = React.forwardRef<HTMLTableCellElement, TableCellProps>(
  ({ className, ...props }, ref) => (
    <td
      ref={ref}
      className={cn("p-3 align-middle text-erp-text-body text-sm", className)}
      {...props}
    />
  )
);
TableCell.displayName = "TableCell";

export interface TableCaptionProps extends React.HTMLAttributes<HTMLTableCaptionElement> {}

export const TableCaption = React.forwardRef<HTMLTableCaptionElement, TableCaptionProps>(
  ({ className, ...props }, ref) => (
    <caption
      ref={ref}
      className={cn("mt-3 text-xs text-erp-text-muted text-center", className)}
      {...props}
    />
  )
);
TableCaption.displayName = "TableCaption";
