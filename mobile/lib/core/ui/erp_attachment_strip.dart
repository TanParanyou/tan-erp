import 'dart:io';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_confirm_dialog.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_status_badge.dart';
import 'package:uuid/uuid.dart';

/// Metadata for a field photo or attachment bound to an entity/area.
@immutable
class FieldAttachmentItem {
  const FieldAttachmentItem({
    required this.id,
    required this.filename,
    required this.localPath,
    this.remoteFileId,
    this.isSynced = false,
  });

  factory FieldAttachmentItem.fromJson(Map<String, Object?> json) {
    return FieldAttachmentItem(
      id: json['id'] as String? ?? '',
      filename: json['filename'] as String? ?? '',
      localPath: json['localPath'] as String? ?? '',
      remoteFileId: json['remoteFileId'] as String?,
      isSynced: json['isSynced'] as bool? ?? false,
    );
  }

  final String id;
  final String filename;
  final String localPath;
  final String? remoteFileId;
  final bool isSynced;

  FieldAttachmentItem copyWith({
    String? id,
    String? filename,
    String? localPath,
    String? remoteFileId,
    bool? isSynced,
  }) => FieldAttachmentItem(
    id: id ?? this.id,
    filename: filename ?? this.filename,
    localPath: localPath ?? this.localPath,
    remoteFileId: remoteFileId ?? this.remoteFileId,
    isSynced: isSynced ?? this.isSynced,
  );

  Map<String, Object?> toJson() => <String, Object?>{
    'id': id,
    'filename': filename,
    'localPath': localPath,
    'remoteFileId': remoteFileId,
    'isSynced': isSynced,
  };
}

/// Reusable horizontal strip for capturing/picking photos with deferred sync.
class ErpAttachmentStrip extends StatelessWidget {
  const ErpAttachmentStrip({
    required this.items,
    required this.onAdded,
    required this.onRemoved,
    this.isReadOnly = false,
    super.key,
  });

  static const _uuid = Uuid();

  final List<FieldAttachmentItem> items;
  final ValueChanged<FieldAttachmentItem> onAdded;
  final ValueChanged<FieldAttachmentItem> onRemoved;
  final bool isReadOnly;

  Future<void> _pick(ImageSource source) async {
    try {
      final picked = await ImagePicker().pickImage(
        source: source,
        maxWidth: 1920,
        maxHeight: 1920,
        imageQuality: 85,
      );
      if (picked == null) return;
      onAdded(
        FieldAttachmentItem(
          id: _uuid.v4(),
          filename: picked.name.isNotEmpty ? picked.name : 'photo.jpg',
          localPath: picked.path,
        ),
      );
    } on Exception {
      // Ignore picker cancellation or unavailable camera on emulator.
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                l10n.attachmentSectionTitle,
                style: text.labelLarge?.copyWith(color: t.textBody),
              ),
            ),
            if (!isReadOnly) ...[
              ErpButton(
                label: l10n.attachmentTakePhoto,
                variant: ErpButtonVariant.outline,
                expand: false,
                onPressed: () => _pick(ImageSource.camera),
              ),
              SizedBox(width: t.space2),
              ErpButton(
                label: l10n.attachmentPickGallery,
                variant: ErpButtonVariant.ghost,
                expand: false,
                onPressed: () => _pick(ImageSource.gallery),
              ),
            ],
          ],
        ),
        SizedBox(height: t.space2),
        if (items.isEmpty)
          Text(
            l10n.attachmentEmptyHint,
            style: text.bodySmall?.copyWith(color: t.textMuted),
          )
        else
          SizedBox(
            height: 96,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              itemCount: items.length,
              separatorBuilder: (_, _) => SizedBox(width: t.space2),
              itemBuilder: (ctx, i) {
                final item = items[i];
                final fileExists =
                    item.localPath.isNotEmpty &&
                    File(item.localPath).existsSync();
                return Container(
                  width: 140,
                  decoration: BoxDecoration(
                    color: t.surfaceSubtle,
                    borderRadius: t.corner,
                    border: Border.all(color: t.border, width: t.borderWidth),
                  ),
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      if (fileExists)
                        Image.file(
                          File(item.localPath),
                          fit: BoxFit.cover,
                        )
                      else
                        Center(
                          child: ErpIcon(
                            ErpIcons.camera,
                            color: t.textMuted,
                            size: 28,
                          ),
                        ),
                      Positioned(
                        left: t.space1,
                        bottom: t.space1,
                        child: ErpStatusBadge(
                          label: item.isSynced
                              ? l10n.attachmentStatusSynced
                              : l10n.attachmentStatusPending,
                          tone: item.isSynced
                              ? ErpBadgeTone.success
                              : ErpBadgeTone.warning,
                        ),
                      ),
                      if (!isReadOnly)
                        Positioned(
                          right: 0,
                          top: 0,
                          child: Material(
                            color: t.surface,
                            child: InkWell(
                              onTap: () => showErpConfirmDialog(
                                ctx,
                                title: l10n.attachmentDeleteTitle,
                                message: l10n.attachmentDeleteConfirm,
                                confirmLabel: l10n.commonConfirm,
                                cancelLabel: l10n.commonCancel,
                                onConfirm: () async => onRemoved(item),
                              ),
                              child: Padding(
                                padding: EdgeInsets.all(t.space1),
                                child: ErpIcon(
                                  ErpIcons.trash,
                                  color: t.danger,
                                  size: 16,
                                ),
                              ),
                            ),
                          ),
                        ),
                    ],
                  ),
                );
              },
            ),
          ),
      ],
    );
  }
}
