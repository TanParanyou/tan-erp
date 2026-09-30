import React from "react";
import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
  TableFooter,
  TableCaption,
} from "./Table";

describe("Table primitive components", () => {
  it("renders a semantic table structure with custom classes", () => {
    render(
      <Table aria-label="test-table">
        <TableCaption>Unit Conversions List</TableCaption>
        <TableHeader>
          <TableRow>
            <TableHead>From Unit</TableHead>
            <TableHead>To Unit</TableHead>
            <TableHead>Factor</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          <TableRow selected>
            <TableCell>KG</TableCell>
            <TableCell>G</TableCell>
            <TableCell>1000</TableCell>
          </TableRow>
        </TableBody>
        <TableFooter>
          <TableRow>
            <TableCell colSpan={2}>Total</TableCell>
            <TableCell>1 Item</TableCell>
          </TableRow>
        </TableFooter>
      </Table>
    );

    expect(screen.getByRole("table", { name: "test-table" })).toBeInTheDocument();
    expect(screen.getByText("Unit Conversions List")).toBeInTheDocument();
    expect(screen.getByText("From Unit")).toBeInTheDocument();
    expect(screen.getByText("KG")).toBeInTheDocument();
    expect(screen.getByText("1000")).toBeInTheDocument();
    expect(screen.getByText("Total")).toBeInTheDocument();
  });
});
