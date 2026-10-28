import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_confirm_dialog.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_status_badge.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';
import 'package:tan_erp_field/features/notifications/data/notifications_repository.dart';
import 'package:tan_erp_field/features/notifications/domain/notification_models.dart';

enum NotificationFilter { all, unread }

@immutable
class NotificationsState {
  const NotificationsState({
    this.isLoading = false,
    this.filter = NotificationFilter.all,
    this.items = const <FieldNotificationItem>[],
  });

  final bool isLoading;
  final NotificationFilter filter;
  final List<FieldNotificationItem> items;

  List<FieldNotificationItem> get visibleItems {
    if (filter == NotificationFilter.unread) {
      return items.where((i) => i.isUnread).toList();
    }
    return items;
  }

  int get unreadCount => items.where((i) => i.isUnread).length;

  NotificationsState copyWith({
    bool? isLoading,
    NotificationFilter? filter,
    List<FieldNotificationItem>? items,
  }) {
    return NotificationsState(
      isLoading: isLoading ?? this.isLoading,
      filter: filter ?? this.filter,
      items: items ?? this.items,
    );
  }
}

class NotificationsController extends Notifier<NotificationsState> {
  @override
  NotificationsState build() {
    unawaited(refresh());
    return const NotificationsState(isLoading: true);
  }

  Future<void> refresh() async {
    state = state.copyWith(isLoading: true);
    final repo = ref.read(notificationsRepositoryProvider);
    final items = await repo.fetchNotifications();
    state = state.copyWith(isLoading: false, items: items);
  }

  void setFilter(NotificationFilter filter) {
    state = state.copyWith(filter: filter);
  }

  Future<void> markRead(String id) async {
    final repo = ref.read(notificationsRepositoryProvider);
    final updated = await repo.markRead(id);
    state = state.copyWith(items: updated);
  }

  Future<void> markAllRead() async {
    if (state.unreadCount == 0) return;
    final repo = ref.read(notificationsRepositoryProvider);
    final updated = await repo.markAllRead();
    state = state.copyWith(items: updated);
  }

  Future<void> submitDecision({
    required FieldNotificationItem item,
    required bool approve,
    String? note,
  }) async {
    final repo = ref.read(notificationsRepositoryProvider);
    final updated = await repo.submitQuickDecision(
      item: item,
      approve: approve,
      note: note,
    );
    state = state.copyWith(items: updated);
  }
}

final notificationsControllerProvider =
    NotifierProvider<NotificationsController, NotificationsState>(
      NotificationsController.new,
    );

class NotificationsPage extends ConsumerWidget {
  const NotificationsPage({super.key});

  String _typeLabel(AppLocalizations l10n, String rawType) {
    final lower = rawType.toLowerCase();
    if (lower.contains('estimate')) return l10n.notificationsTypeEstimate;
    if (lower.contains('cost')) return l10n.notificationsTypeCost;
    if (lower.contains('po') || lower.contains('purchase')) {
      return l10n.notificationsTypePo;
    }
    if (lower.contains('change_order') || lower.contains('changeorder')) {
      return l10n.notificationsTypeChangeOrder;
    }
    if (lower.contains('mrp')) return l10n.notificationsTypeMrp;
    if (lower.contains('role')) return l10n.notificationsTypeRole;
    if (lower.contains('member')) return l10n.notificationsTypeMembership;
    return l10n.notificationsTypeGeneral;
  }

  bool _isApprovable(String rawType) {
    final lower = rawType.toLowerCase();
    return lower.contains('estimate') ||
        lower.contains('po') ||
        lower.contains('purchase') ||
        lower.contains('change_order') ||
        lower.contains('changeorder') ||
        lower.contains('pending');
  }

  String _formatTimestamp(DateTime utc) {
    final local = utc.toLocal();
    final d = local.day.toString().padLeft(2, '0');
    final m = local.month.toString().padLeft(2, '0');
    final y = local.year;
    final hh = local.hour.toString().padLeft(2, '0');
    final mm = local.minute.toString().padLeft(2, '0');
    return '$d/$m/$y $hh:$mm';
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final t = context.tokens;
    final l10n = context.l10n;
    final state = ref.watch(notificationsControllerProvider);
    final notifier = ref.read(notificationsControllerProvider.notifier);
    final visible = state.visibleItems;

    return Scaffold(
      appBar: AppBar(
        title: Text(l10n.notificationsTitle),
        actions: [
          if (state.unreadCount > 0)
            TextButton(
              onPressed: notifier.markAllRead,
              child: Text(
                l10n.notificationsMarkAllRead,
                style: Theme.of(context).textTheme.labelMedium?.copyWith(
                  color: t.action,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          IconButton(
            tooltip: l10n.notificationsTitle,
            onPressed: notifier.refresh,
            icon: ErpIcon(ErpIcons.refresh, color: t.textMain),
          ),
        ],
      ),
      body: Column(
        children: [
          Container(
            width: double.infinity,
            padding: EdgeInsets.symmetric(
              horizontal: t.space4,
              vertical: t.space3,
            ),
            decoration: BoxDecoration(
              color: t.surface,
              border: Border(
                bottom: BorderSide(color: t.border, width: t.borderWidth),
              ),
            ),
            child: Row(
              children: [
                _FilterChipButton(
                  label: l10n.notificationsFilterAll,
                  selected: state.filter == NotificationFilter.all,
                  onTap: () => notifier.setFilter(NotificationFilter.all),
                ),
                SizedBox(width: t.space2),
                _FilterChipButton(
                  label:
                      '${l10n.notificationsFilterUnread}'
                      ' (${state.unreadCount})',
                  selected: state.filter == NotificationFilter.unread,
                  onTap: () => notifier.setFilter(NotificationFilter.unread),
                ),
              ],
            ),
          ),
          Expanded(
            child: state.isLoading
                ? Center(
                    child: SizedBox(
                      width: 24,
                      height: 24,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: t.action,
                      ),
                    ),
                  )
                : RefreshIndicator(
                    color: t.action,
                    onRefresh: notifier.refresh,
                    child: visible.isEmpty
                        ? ListView(
                            padding: EdgeInsets.all(t.space6),
                            children: [
                              ErpCard(
                                children: [
                                  Text(
                                    l10n.notificationsEmptyTitle,
                                    style: Theme.of(
                                      context,
                                    ).textTheme.titleMedium,
                                  ),
                                  SizedBox(height: t.space1),
                                  Text(
                                    l10n.notificationsEmptyBody,
                                    style: Theme.of(context).textTheme.bodySmall
                                        ?.copyWith(color: t.textBody),
                                  ),
                                ],
                              ),
                            ],
                          )
                        : ListView.separated(
                            padding: EdgeInsets.all(t.space4),
                            itemCount: visible.length,
                            separatorBuilder: (_, _) =>
                                SizedBox(height: t.space3),
                            itemBuilder: (context, index) {
                              final item = visible[index];
                              final title = _typeLabel(l10n, item.type);
                              final body = item.displayBody.isNotEmpty
                                  ? item.displayBody
                                  : item.type;
                              final isApprovable = _isApprovable(item.type);

                              return InkWell(
                                onTap: () {
                                  if (isApprovable) {
                                    unawaited(
                                      showModalBottomSheet<void>(
                                        context: context,
                                        isScrollControlled: true,
                                        backgroundColor: t.surface,
                                        shape: RoundedRectangleBorder(
                                          borderRadius: t.corner,
                                        ),
                                        builder: (_) =>
                                            _QuickApprovalSheet(item: item),
                                      ),
                                    );
                                  } else if (item.isUnread) {
                                    unawaited(notifier.markRead(item.id));
                                  }
                                },
                                child: ErpCard(
                                  children: [
                                    Row(
                                      children: [
                                        ErpStatusBadge(
                                          label: item.isUnread
                                              ? l10n.notificationsUnreadBadge
                                              : l10n.notificationsReadBadge,
                                          tone: item.isUnread
                                              ? ErpBadgeTone.info
                                              : ErpBadgeTone.neutral,
                                        ),
                                        const Spacer(),
                                        Text(
                                          _formatTimestamp(item.createdAtUtc),
                                          style: Theme.of(context)
                                              .textTheme
                                              .labelSmall
                                              ?.copyWith(
                                                color: t.textMuted,
                                                fontFeatures: const [
                                                  FontFeature.tabularFigures(),
                                                ],
                                              ),
                                        ),
                                      ],
                                    ),
                                    SizedBox(height: t.space2),
                                    Text(
                                      title,
                                      style: Theme.of(
                                        context,
                                      ).textTheme.titleMedium,
                                      maxLines: 1,
                                      overflow: TextOverflow.ellipsis,
                                    ),
                                    SizedBox(height: t.space1),
                                    Text(
                                      body,
                                      style: Theme.of(context)
                                          .textTheme
                                          .bodySmall
                                          ?.copyWith(color: t.textBody),
                                      maxLines: 2,
                                      overflow: TextOverflow.ellipsis,
                                    ),
                                    if (isApprovable) ...[
                                      SizedBox(height: t.space2),
                                      Text(
                                        l10n.approvalQuickActionHint,
                                        style: Theme.of(context)
                                            .textTheme
                                            .bodySmall
                                            ?.copyWith(
                                              color: t.action,
                                              fontWeight: FontWeight.w600,
                                            ),
                                      ),
                                    ],
                                  ],
                                ),
                              );
                            },
                          ),
                  ),
          ),
        ],
      ),
    );
  }
}

class _FilterChipButton extends StatelessWidget {
  const _FilterChipButton({
    required this.label,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    return Material(
      color: selected ? t.action : t.surfaceSubtle,
      child: InkWell(
        onTap: onTap,
        child: Container(
          constraints: const BoxConstraints(minHeight: 36),
          padding: EdgeInsets.symmetric(
            horizontal: t.space3,
            vertical: t.space2,
          ),
          decoration: BoxDecoration(
            border: Border.all(
              color: selected ? t.action : t.border,
              width: t.borderWidth,
            ),
          ),
          alignment: Alignment.center,
          child: Text(
            label,
            style: Theme.of(context).textTheme.labelMedium?.copyWith(
              color: selected ? t.onAction : t.textMain,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
      ),
    );
  }
}

class _QuickApprovalSheet extends ConsumerStatefulWidget {
  const _QuickApprovalSheet({required this.item});

  final FieldNotificationItem item;

  @override
  ConsumerState<_QuickApprovalSheet> createState() =>
      _QuickApprovalSheetState();
}

class _QuickApprovalSheetState extends ConsumerState<_QuickApprovalSheet> {
  final _noteControl = FormControl<String>();
  bool _hasNoteError = false;
  bool _isSubmitting = false;

  @override
  void dispose() {
    _noteControl.dispose();
    super.dispose();
  }

  Future<void> _handleDecision({required bool approve}) async {
    final note = _noteControl.value?.trim();
    if (!approve && (note == null || note.isEmpty)) {
      setState(() => _hasNoteError = true);
      return;
    }
    setState(() => _hasNoteError = false);

    final l10n = context.l10n;
    final confirmed = await showErpConfirmDialog(
      context,
      title: approve
          ? l10n.approvalConfirmApproveTitle
          : l10n.approvalConfirmReturnTitle,
      message: widget.item.displayBody,
      confirmLabel: approve
          ? l10n.approvalApproveAction
          : l10n.approvalReturnAction,
      cancelLabel: l10n.commonCancel,
      onConfirm: () async {
        setState(() => _isSubmitting = true);
        try {
          await ref
              .read(notificationsControllerProvider.notifier)
              .submitDecision(
                item: widget.item,
                approve: approve,
                note: note,
              );
        } finally {
          if (mounted) {
            setState(() => _isSubmitting = false);
          }
        }
      },
    );

    if (confirmed && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            approve
                ? l10n.approvalSuccessApproved
                : l10n.approvalSuccessReturned,
          ),
        ),
      );
      Navigator.of(context).pop();
    }
  }

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final l10n = context.l10n;
    final text = Theme.of(context).textTheme;

    final docNo = widget.item.payload['documentNumber'] ??
        widget.item.payload['code'] ??
        widget.item.id;
    final summary = widget.item.displayBody;

    return Padding(
      padding: EdgeInsets.only(
        bottom: MediaQuery.of(context).viewInsets.bottom,
      ),
      child: SafeArea(
        child: Padding(
          padding: EdgeInsets.all(t.space4),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(l10n.approvalSheetTitle, style: text.titleMedium),
              SizedBox(height: t.space3),
              ErpCard(
                children: [
                  Text(
                    l10n.approvalDocumentLabel,
                    style: text.bodySmall?.copyWith(color: t.textMuted),
                  ),
                  Text(docNo, style: text.titleSmall),
                  SizedBox(height: t.space2),
                  Text(
                    l10n.approvalSummaryLabel,
                    style: text.bodySmall?.copyWith(color: t.textMuted),
                  ),
                  Text(summary, style: text.bodyMedium),
                ],
              ),
              SizedBox(height: t.space3),
              if (_hasNoteError) ...[
                ErpAlert(
                  message: l10n.approvalNoteRequiredError,
                  severity: ErpAlertSeverity.danger,
                ),
                SizedBox(height: t.space2),
              ],
              ErpTextField(
                control: _noteControl,
                label: l10n.approvalNoteLabel,
                placeholder: l10n.approvalNotePlaceholder,
              ),
              SizedBox(height: t.space4),
              Row(
                children: [
                  Expanded(
                    child: ErpButton(
                      label: l10n.approvalReturnAction,
                      variant: ErpButtonVariant.outline,
                      isLoading: _isSubmitting,
                      onPressed: () => _handleDecision(approve: false),
                    ),
                  ),
                  SizedBox(width: t.space3),
                  Expanded(
                    child: ErpButton(
                      label: l10n.approvalApproveAction,
                      isLoading: _isSubmitting,
                      onPressed: () => _handleDecision(approve: true),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
