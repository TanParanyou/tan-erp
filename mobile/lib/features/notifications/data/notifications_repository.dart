import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/offline/offline_store.dart';
import 'package:tan_erp_field/core/offline/sync_controller.dart';
import 'package:tan_erp_field/features/notifications/domain/notification_models.dart';

class NotificationsRepository {
  NotificationsRepository(this._ref);

  final Ref _ref;
  static const String _cacheKey = 'notifications_v1';

  String get _membershipId =>
      _ref.read(activeMembershipIdProvider) ?? 'default';

  Future<List<FieldNotificationItem>> fetchNotifications() async {
    final client = _ref.read(apiClientProvider);
    try {
      final res = await client.call(
        () => client.api.getNotificationsApi().apiV1NotificationsGet(
          pageSize: 50,
        ),
      );
      final items = res.items;
      if (items != null) {
        final mapped = <FieldNotificationItem>[
          for (final item in items) _fromApi(item),
        ];
        await _saveCache(mapped);
        return mapped;
      }
    } on Object {
      // Fallback to offline cache or demo seed.
    }

    final cached = await _readCache();
    if (cached.isNotEmpty) {
      return cached;
    }

    final seeded = _seedDemoNotifications();
    await _saveCache(seeded);
    return seeded;
  }

  Future<List<FieldNotificationItem>> markRead(String id) async {
    final now = DateTime.now().toUtc();
    final current = await _readCache();
    final updated = <FieldNotificationItem>[
      for (final n in current)
        if (n.id == id && n.isUnread)
          n.copyWith(readAtUtc: () => now)
        else
          n,
    ];
    await _saveCache(updated);
    await _ref.read(syncControllerProvider.notifier).enqueue(
      entityType: 'notification',
      entityId: id,
      operation: 'mark_read',
      title: id,
      payload: <String, Object?>{'id': id},
    );
    return updated;
  }

  Future<List<FieldNotificationItem>> markAllRead() async {
    final now = DateTime.now().toUtc();
    final current = await _readCache();
    final updated = <FieldNotificationItem>[
      for (final n in current)
        if (n.isUnread) n.copyWith(readAtUtc: () => now) else n,
    ];
    await _saveCache(updated);
    await _ref.read(syncControllerProvider.notifier).enqueue(
      entityType: 'notification',
      entityId: 'all',
      operation: 'mark_all_read',
      title: 'read-all',
      payload: const <String, Object?>{},
    );
    return updated;
  }

  Future<List<FieldNotificationItem>> submitQuickDecision({
    required FieldNotificationItem item,
    required bool approve,
    String? note,
  }) async {
    final lower = item.type.toLowerCase();
    final String entityType;
    final String operation;

    if (lower.contains('po') || lower.contains('purchase')) {
      entityType = 'purchase_order';
      operation = approve ? 'approve' : 'reject';
    } else if (lower.contains('change_order') ||
        lower.contains('changeorder')) {
      entityType = 'change_order';
      operation = approve ? 'approve' : 'reject';
    } else {
      entityType = 'estimate';
      operation = 'review_decision';
    }

    final docId = item.payload['entityId'] ??
        item.payload['documentNumber'] ??
        item.id;

    await _ref.read(syncControllerProvider.notifier).enqueue(
      entityType: entityType,
      entityId: docId,
      operation: operation,
      title: docId,
      payload: <String, Object?>{
        'decision': approve ? 'approved' : 'returned',
        'note': note,
        'revisionNo': 1,
        'projectId': item.payload['projectId'] ?? '',
      },
    );

    return await markRead(item.id);
  }

  FieldNotificationItem _fromApi(NotificationResponse r) {
    final map = <String, String>{};
    final p = r.payload;
    if (p != null) {
      for (final e in p.entries) {
        map[e.key] = e.value;
      }
    }
    return FieldNotificationItem(
      id: r.id ?? '',
      type: r.type ?? '',
      payload: map,
      deepLink: r.deepLink,
      createdAtUtc: r.createdAtUtc?.toUtc() ?? DateTime.now().toUtc(),
      readAtUtc: r.readAtUtc?.toUtc(),
    );
  }

  Future<List<FieldNotificationItem>> _readCache() async {
    final store = _ref.read(offlineStoreProvider);
    final entry = await store.readCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey,
    );
    if (entry == null || entry.payloadJson.isEmpty) {
      return const <FieldNotificationItem>[];
    }
    try {
      final decoded = jsonDecode(entry.payloadJson);
      if (decoded is! List<Object?>) return const <FieldNotificationItem>[];
      final result = <FieldNotificationItem>[];
      for (final item in decoded) {
        if (item is Map<String, Object?>) {
          result.add(FieldNotificationItem.fromJson(item));
        }
      }
      return result;
    } on Object {
      return const <FieldNotificationItem>[];
    }
  }

  Future<void> _saveCache(List<FieldNotificationItem> items) async {
    final store = _ref.read(offlineStoreProvider);
    final payload = jsonEncode([for (final i in items) i.toJson()]);
    await store.writeCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey,
      payloadJson: payload,
    );
  }

  List<FieldNotificationItem> _seedDemoNotifications() {
    final now = DateTime.now().toUtc();
    return <FieldNotificationItem>[
      FieldNotificationItem(
        id: 'demo-notif-1',
        type: 'estimate_pending_approval',
        payload: const <String, String>{
          'documentNumber': 'EST-202610-0014',
          'title': 'ปรับปรุงโถงต้อนรับ อาคารสาทรสแควร์',
          'status': 'รออนุมัติใบประเมินราคา',
        },
        createdAtUtc: now.subtract(const Duration(minutes: 25)),
      ),
      FieldNotificationItem(
        id: 'demo-notif-2',
        type: 'change_order_pending',
        payload: const <String, String>{
          'documentNumber': 'CO-202610-0003',
          'title': 'งานกั้นห้องกระจก ชั้น 12',
          'status': 'แจ้งแก้ไขระยะหน้างานเพิ่มเติม',
        },
        createdAtUtc: now.subtract(const Duration(hours: 3)),
      ),
      FieldNotificationItem(
        id: 'demo-notif-3',
        type: 'po_pending_approval',
        payload: const <String, String>{
          'documentNumber': 'PO-202610-0042',
          'title': 'สั่งซื้อแผ่นไม้โอ๊คสำเร็จรูป 120 ตร.ม.',
        },
        createdAtUtc: now.subtract(const Duration(days: 1)),
        readAtUtc: now.subtract(const Duration(hours: 18)),
      ),
    ];
  }
}

final notificationsRepositoryProvider = Provider<NotificationsRepository>(
  NotificationsRepository.new,
);
