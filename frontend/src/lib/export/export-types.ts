export type ExportFormat = "csv" | "xlsx";

export interface ExportColumn<T> {
  header: string;
  accessor: (item: T) => string | number | boolean | null | undefined;
}
