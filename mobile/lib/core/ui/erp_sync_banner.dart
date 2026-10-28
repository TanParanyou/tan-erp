import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/offline/offline_store.dart';
import 'package:tan_erp_field/core/offline/sync_controller.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_conflict_diff_sheet.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_status_badge.dart';

/// Non-intrusive top sync banner shown only when offline, syncing, or when
/// there are queued/conflicting outbox items (design.md 8.2).
class ErpSyncBanner extends ConsumerWidget {
  const ErpSyncBanner({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final sync = ref.watch(syncControllerProvider);
    if (!sync.shouldShowBanner) return const SizedBox.shrink();

    final t = context.tokens;
    final l10n = context.l10n;
    final hasConflict = sync.conflictCount > 0 || sync.failedCount > 0;
    final bg = hasConflict
        ? t.dangerBg
        : !sync.isOnline
        ? t.warningBg
        : t.infoBg;
    final fg = hasConflict
        ? t.danger
        : !sync.isOnline
        ? t.warning
        : t.info;
    final message = hasConflict
        ? l10n.syncBannerConflict(sync.conflictCount + sync.failedCount)
        : !sync.isOnline
        ? l10n.syncBannerOffline
        : sync.isSyncing
        ? l10n.syncBannerSyncing(sync.pendingCount)
        : l10n.syncBannerPending(sync.pendingCount);

    return Material(
      color: bg,
      child: InkWell(
        onTap: () => _openQueueSheet(context),
        child: Container(
          constraints: BoxConstraints(minHeight: t.tapTarget),
          padding: EdgeInsets.symmetric(
            horizontal: t.space4,
            vertical: t.space2,
          ),
          decoration: BoxDecoration(
            border: Border(
              bottom: BorderSide(color: t.border, width: t.borderWidth),
            ),
          ),
          child: Row(
            children: [
              ErpIcon(
                !sync.isOnline ? ErpIcons.cloudOff : ErpIcons.refresh,
                color: fg,
                size: 18,
              ),
              SizedBox(width: t.space2),
              Expanded(
                child: Text(
                  message,
                  style: Theme.of(
                    context,
                  ).textTheme.bodySmall?.copyWith(color: fg),
                ),
              ),
              ErpIcon(ErpIcons.chevronRight, color: fg, size: 18),
            ],
          ),
        ),
      ),
    );
  }

  static Future<void> _openQueueSheet(BuildContext context) =>
      showModalBottomSheet<void>(
        context: context,
        isScrollControlled: true,
        backgroundColor: context.tokens.surface,
        shape: RoundedRectangleBorder(borderRadius: context.tokens.corner),
        builder: (_) => const _SyncQueueSheet(),
      );
}

class _SyncQueueSheet extends ConsumerWidget {
  const _SyncQueueSheet();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final t = context.tokens;
    final l10n = context.l10n;
    final text = Theme.of(context).textTheme;
    final sync = ref.watch(syncControllerProvider);

    return SafeArea(
      child: Padding(
        padding: EdgeInsets.all(t.space4),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(l10n.syncQueueTitle, style: text.titleMedium),
            SizedBox(height: t.space3),
            if (sync.mutations.isEmpty)
              ErpEmptyState(
                icon: ErpIcons.checkCircle,
                title: l10n.syncQueueEmpty,
              )
            else
              ConstrainedBox(
                constraints: const BoxConstraints(maxHeight: 320),
                child: ListView.separated(
                  shrinkWrap: true,
                  itemCount: sync.mutations.length,
                  separatorBuilder: (_, _) => SizedBox(height: t.space2),
                  itemBuilder: (context, index) {
                    final m = sync.mutations[index];
                    final (badgeLabel, tone) = switch (m.status) {
                      OutboxStatus.pending => (
                        l10n.syncStatusPending,
                        ErpBadgeTone.info,
                      ),
                      OutboxStatus.syncing => (
                        l10n.syncStatusSyncing,
                        ErpBadgeTone.info,
                      ),
                      OutboxStatus.conflict => (
                        l10n.syncStatusConflict,
                        ErpBadgeTone.danger,
                      ),
                      OutboxStatus.failed => (
                        l10n.syncStatusFailed,
                        ErpBadgeTone.warning,
                      ),
                    };
                    return ErpCard(
                      children: [
                        Row(
                          children: [
                            Expanded(
                              child: Text(m.title, style: text.titleSmall),
                            ),
                            ErpStatusBadge(label: badgeLabel, tone: tone),
                          ],
                        ),
                        if (m.errorCode != null) ...[
                          SizedBox(height: t.space1),
                          Text(m.errorCode!, style: text.bodySmall),
                        ],
                        if (m.status == OutboxStatus.conflict ||
                            m.status == OutboxStatus.failed) ...[
                          SizedBox(height: t.space2),
                          ErpButton(
                            label: l10n.conflictResolveAction,
                            variant: ErpButtonVariant.outline,
                            onPressed: () =>
                                showErpConflictDiffSheet(context, m),
                          ),
                        ],
                      ],
                    );
                  },
                ),
              ),
            SizedBox(height: t.space4),
            if (sync.conflictCount > 0 || sync.failedCount > 0) ...[
              ErpButton(
                label: l10n.syncQueueClearFailed,
                variant: ErpButtonVariant.outline,
                onPressed: () => ref
                    .read(syncControllerProvider.notifier)
                    .clearFailedAndConflicts(),
              ),
              SizedBox(height: t.space2),
            ],
            ErpButton(
              label: l10n.syncQueueSendNow,
              isLoading: sync.isSyncing,
              onPressed: sync.mutations.isEmpty
                  ? null
                  : () => ref.read(syncControllerProvider.notifier).flushNow(),
            ),
          ],
        ),
      ),
    );
  }
}
