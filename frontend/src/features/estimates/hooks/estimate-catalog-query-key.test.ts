import { describe, expect, it } from "vitest";
import { QueryClient } from "@tanstack/react-query";
import { estimateCatalogQueryKey, isEstimateCatalogQueryKey } from "./estimate-catalog-query-key";

describe("estimate catalog query key", () => {
  it("invalidates every catalog variant without touching unrelated business queries", async () => {
    const client = new QueryClient();
    const thaiKey = estimateCatalogQueryKey("membership-1", "th", { branchId: "branch-1", pageSize: 50 });
    const englishKey = estimateCatalogQueryKey("membership-1", "en", { branchId: "branch-1", search: "wood", pageSize: 50 });
    const unrelatedKey = ["business", "membership-1", "th", "estimates", "detail", "estimate-1"];
    client.setQueryData(thaiKey, { items: [] });
    client.setQueryData(englishKey, { items: [] });
    client.setQueryData(unrelatedKey, { id: "estimate-1" });

    await client.invalidateQueries({ predicate: ({ queryKey }) => isEstimateCatalogQueryKey(queryKey) });

    expect(client.getQueryState(thaiKey)?.isInvalidated).toBe(true);
    expect(client.getQueryState(englishKey)?.isInvalidated).toBe(true);
    expect(client.getQueryState(unrelatedKey)?.isInvalidated).toBe(false);
  });
});
