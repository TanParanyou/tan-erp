import { QueryClient } from "@tanstack/react-query";

export function createQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 1000 * 60 * 5,
        retry: (failureCount, error: unknown) => {
          const status = (error as { status?: number })?.status;
          if (status === 401 || status === 403) {
            return false;
          }
          return failureCount < 2;
        },
      },
    },
  });
}

export const queryClient = createQueryClient();
