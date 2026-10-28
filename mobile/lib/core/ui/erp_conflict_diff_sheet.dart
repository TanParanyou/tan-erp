import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/offline/offline_store.dart';
import 'package:tan_erp_field/core/offline/sync_controller.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';

Future<void> showErpConflictDiffSheet(
  BuildContext context,
  OutboxMutation mutation,
) {
  return showModalBottomSheet<void>(
    context: context,
    isScrollControlled: true,
    backgroundColor: context.tokens.surface,
    shape: RoundedRectangleBorder(borderRadius: context.tokens.corner),
    builder: (context) => _ConflictDiffSheet(mutation: mutation),
  );
}

class _ConflictDiffSheet extends ConsumerWidget {
  const _ConflictDiffSheet({required this.mutation});

  final OutboxMutation mutation;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final t = context.tokens;
    final l10n = context.l10n;
    final text = Theme.of(context).textTheme;

    var payload = const <String, Object?>{};
    try {
      final decoded = jsonDecode(mutation.payloadJson);
      if (decoded is Map<String, Object?>) {
        payload = decoded;
      }
    } on Object {
      // payload decoding fallback
    }

    final serverState = mutation.errorCode ?? 'HTTP 409/412';

    return SafeArea(
      child: Padding(
        padding: EdgeInsets.all(t.space4),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(l10n.conflictSheetTitle, style: text.titleMedium),
            SizedBox(height: t.space2),
            Text(
              l10n.conflictDescription,
              style: text.bodySmall?.copyWith(color: t.textMuted),
            ),
            SizedBox(height: t.space3),
            ErpCard(
              children: [
                Row(
                  children: [
                    Expanded(
                      flex: 2,
                      child: Text(
                        l10n.conflictFieldColumn,
                        style: text.labelSmall?.copyWith(
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                    Expanded(
                      flex: 3,
                      child: Text(
                        l10n.conflictLocalColumn,
                        style: text.labelSmall?.copyWith(
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                    Expanded(
                      flex: 3,
                      child: Text(
                        l10n.conflictServerColumn,
                        style: text.labelSmall?.copyWith(
                          fontWeight: FontWeight.w700,
                          color: t.danger,
                        ),
                      ),
                    ),
                  ],
                ),
                Divider(height: t.space3, color: t.border),
                ConstrainedBox(
                  constraints: const BoxConstraints(maxHeight: 220),
                  child: ListView.separated(
                    shrinkWrap: true,
                    itemCount: payload.length,
                    separatorBuilder: (_, _) => Divider(
                      height: t.space2,
                      color: t.border,
                    ),
                    itemBuilder: (context, index) {
                      final key = payload.keys.elementAt(index);
                      final val = payload[key]?.toString() ?? '-';
                      return Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Expanded(
                            flex: 2,
                            child: Text(
                              key,
                              style: text.bodySmall?.copyWith(
                                fontFamily: 'monospace',
                                color: t.textBody,
                              ),
                            ),
                          ),
                          Expanded(
                            flex: 3,
                            child: Text(
                              val,
                              style: text.bodySmall?.copyWith(
                                color: t.textMain,
                              ),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                          Expanded(
                            flex: 3,
                            child: Text(
                              serverState,
                              style: text.bodySmall?.copyWith(
                                color: t.textMuted,
                              ),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                        ],
                      );
                    },
                  ),
                ),
              ],
            ),
            SizedBox(height: t.space4),
            ErpButton(
              label: l10n.conflictKeepLocalAction,
              onPressed: () async {
                await ref
                    .read(syncControllerProvider.notifier)
                    .retryMutation(mutation.id);
                if (context.mounted) {
                  Navigator.of(context).pop();
                }
              },
            ),
            SizedBox(height: t.space2),
            ErpButton(
              label: l10n.conflictAcceptServerAction,
              variant: ErpButtonVariant.outline,
              onPressed: () async {
                await ref
                    .read(syncControllerProvider.notifier)
                    .removeMutation(mutation.id);
                if (context.mounted) {
                  Navigator.of(context).pop();
                }
              },
            ),
          ],
        ),
      ),
    );
  }
}
