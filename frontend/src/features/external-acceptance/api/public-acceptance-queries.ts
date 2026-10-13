import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type PublicAcceptanceViewResponse, type PublicAcceptRequest } from "@/lib/api/api-client";
import { saveBlob } from "@/lib/files/save-blob";

type DocumentLocale = "th" | "en";

export function publicAcceptanceKey(token: string, locale: DocumentLocale) {
  return ["public", "quotation-acceptance", token, locale] as const;
}

/** The customer has no ERP session, so these calls carry no auth header; the token in the URL is the credential. */
export function usePublicAcceptance(token: string, locale: DocumentLocale): UseQueryResult<PublicAcceptanceViewResponse, Error> {
  return useQuery({
    queryKey: publicAcceptanceKey(token, locale),
    enabled: token.length > 0,
    retry: false,
    queryFn: async ({ signal }) => apiClient.getPublicAcceptance(token, locale, signal),
  });
}

/** The exact stored PDF the customer is asked to accept. Public like the rest of the page: the token in the URL is the credential. */
export function usePublicPdfDownload(token: string) {
  return useMutation({
    mutationFn: async (fileName: string) => {
      const blob = await apiClient.downloadPublicQuotationDocument(token);
      saveBlob(blob, fileName);
    },
  });
}

export function usePublicAcceptMutation(token: string, locale: DocumentLocale) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (payload: PublicAcceptRequest) => apiClient.acceptPublicQuotation(token, payload, locale),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["public", "quotation-acceptance", token] });
    },
  });
}
