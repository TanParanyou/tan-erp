import { fileClient } from "@/lib/api/file-client";

interface UploadItemImageInput {
  file: File;
  itemId: string;
  parentType?: "item" | "item-category" | "item-brand";
  token: string;
  membershipId: string;
  locale: "th" | "en";
  idempotencyKey: string;
}

export async function uploadVerifiedItemImage(input: UploadItemImageInput): Promise<string> {
  const { file, itemId, token, membershipId, locale, idempotencyKey } = input;
  const session = await fileClient.createSession({
    parentType: input.parentType ?? "item",
    parentId: itemId,
    creationIntentId: null,
    files: [{ filename: file.name, mediaType: file.type, fileSizeBytes: file.size }],
  }, { token, membershipId, locale, idempotencyKey });

  const slotId = session.slots?.[0]?.slotId;
  if (!session.sessionId || !slotId) throw new Error("Upload session is incomplete.");

  const completed = await fileClient.completeSession(session.sessionId, [{ slotId, file }], { token, membershipId, locale });
  const fileId = completed.files?.[0]?.fileId;
  if (!fileId) throw new Error("Verified upload did not return a file identifier.");
  return fileId;
}
