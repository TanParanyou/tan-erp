import { ItemEditor } from "@/features/item-master/components/item-editor";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";

interface ItemEditorPageProps {
  params: Promise<{ id: string }>;
}

export default async function ItemEditorPage({ params }: ItemEditorPageProps) {
  const { id } = await params;
  const isCreate = id === "create" || id === "add";

  return (
    <PermissionGuard permission={isCreate ? PERMISSIONS.ITEMS_CREATE : PERMISSIONS.ITEMS_READ}>
      <ItemEditor id={id} />
    </PermissionGuard>
  );
}
