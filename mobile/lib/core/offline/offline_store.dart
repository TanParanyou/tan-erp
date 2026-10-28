import 'package:drift/drift.dart';
import 'package:drift_flutter/drift_flutter.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

enum OutboxStatus {
  pending('pending'),
  syncing('syncing'),
  conflict('conflict'),
  failed('failed');

  OutboxStatus(this.wire);
  final String wire;

  static OutboxStatus fromWire(String value) => OutboxStatus.values.firstWhere(
    (s) => s.wire == value,
    orElse: () => OutboxStatus.pending,
  );
}

class CachedEntry {
  const CachedEntry({
    required this.membershipId,
    required this.cacheKey,
    required this.payloadJson,
    required this.updatedAtUtc,
  });

  final String membershipId;
  final String cacheKey;
  final String payloadJson;
  final DateTime updatedAtUtc;
}

class OutboxMutation {
  const OutboxMutation({
    required this.id,
    required this.membershipId,
    required this.entityType,
    required this.entityId,
    required this.operation,
    required this.title,
    required this.payloadJson,
    required this.idempotencyKey,
    required this.status,
    required this.createdAtUtc,
    this.expectedRowVersion,
    this.errorCode,
  });

  final String id;
  final String membershipId;
  final String entityType;
  final String entityId;
  final String operation;
  final String title;
  final String payloadJson;
  final String idempotencyKey;
  final String? expectedRowVersion;
  final OutboxStatus status;
  final String? errorCode;
  final DateTime createdAtUtc;
}

/// Drift-backed SQLite database for offline read cache and write outbox.
class OfflineStore extends GeneratedDatabase {
  OfflineStore([QueryExecutor? executor])
    : super(executor ?? driftDatabase(name: 'tan_erp_field_offline'));

  @override
  int get schemaVersion => 1;

  @override
  Iterable<TableInfo<Table, Object?>> get allTables => const [];

  @override
  MigrationStrategy get migration => MigrationStrategy(
    onCreate: (m) async {
      await customStatement('''
        CREATE TABLE IF NOT EXISTS cached_entries (
          membership_id TEXT NOT NULL,
          cache_key TEXT NOT NULL,
          payload_json TEXT NOT NULL,
          updated_at_utc TEXT NOT NULL,
          PRIMARY KEY (membership_id, cache_key)
        )
      ''');
      await customStatement('''
        CREATE TABLE IF NOT EXISTS outbox_mutations (
          id TEXT PRIMARY KEY NOT NULL,
          membership_id TEXT NOT NULL,
          entity_type TEXT NOT NULL,
          entity_id TEXT NOT NULL,
          operation TEXT NOT NULL,
          title TEXT NOT NULL,
          payload_json TEXT NOT NULL,
          idempotency_key TEXT NOT NULL,
          expected_row_version TEXT,
          status TEXT NOT NULL,
          error_code TEXT,
          created_at_utc TEXT NOT NULL
        )
      ''');
    },
  );

  Future<CachedEntry?> readCache({
    required String membershipId,
    required String cacheKey,
  }) async {
    final rows = await customSelect(
      'SELECT membership_id, cache_key, payload_json, updated_at_utc '
      'FROM cached_entries WHERE membership_id = ? AND cache_key = ? LIMIT 1',
      variables: [
        Variable.withString(membershipId),
        Variable.withString(cacheKey),
      ],
    ).get();
    if (rows.isEmpty) return null;
    final r = rows.first;
    return CachedEntry(
      membershipId: r.read<String>('membership_id'),
      cacheKey: r.read<String>('cache_key'),
      payloadJson: r.read<String>('payload_json'),
      updatedAtUtc:
          DateTime.tryParse(r.read<String>('updated_at_utc')) ??
          DateTime.now().toUtc(),
    );
  }

  Future<void> writeCache({
    required String membershipId,
    required String cacheKey,
    required String payloadJson,
  }) async {
    final now = DateTime.now().toUtc().toIso8601String();
    await customStatement(
      'INSERT INTO cached_entries '
      '(membership_id, cache_key, payload_json, updated_at_utc) '
      'VALUES (?, ?, ?, ?) '
      'ON CONFLICT(membership_id, cache_key) DO UPDATE SET '
      'payload_json = excluded.payload_json, '
      'updated_at_utc = excluded.updated_at_utc',
      [membershipId, cacheKey, payloadJson, now],
    );
  }

  Future<void> enqueueMutation(OutboxMutation item) async {
    await customStatement(
      'INSERT OR REPLACE INTO outbox_mutations '
      '(id, membership_id, entity_type, entity_id, operation, title, '
      'payload_json, idempotency_key, expected_row_version, status, '
      'error_code, created_at_utc) '
      'VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)',
      [
        item.id,
        item.membershipId,
        item.entityType,
        item.entityId,
        item.operation,
        item.title,
        item.payloadJson,
        item.idempotencyKey,
        item.expectedRowVersion,
        item.status.wire,
        item.errorCode,
        item.createdAtUtc.toUtc().toIso8601String(),
      ],
    );
  }

  Future<List<OutboxMutation>> listMutations(String membershipId) async {
    final rows = await customSelect(
      'SELECT id, membership_id, entity_type, entity_id, operation, title, '
      'payload_json, idempotency_key, expected_row_version, status, '
      'error_code, created_at_utc '
      'FROM outbox_mutations WHERE membership_id = ? '
      'ORDER BY created_at_utc ASC',
      variables: [Variable.withString(membershipId)],
    ).get();
    return [
      for (final r in rows)
        OutboxMutation(
          id: r.read<String>('id'),
          membershipId: r.read<String>('membership_id'),
          entityType: r.read<String>('entity_type'),
          entityId: r.read<String>('entity_id'),
          operation: r.read<String>('operation'),
          title: r.read<String>('title'),
          payloadJson: r.read<String>('payload_json'),
          idempotencyKey: r.read<String>('idempotency_key'),
          expectedRowVersion: r.readNullable<String>('expected_row_version'),
          status: OutboxStatus.fromWire(r.read<String>('status')),
          errorCode: r.readNullable<String>('error_code'),
          createdAtUtc:
              DateTime.tryParse(r.read<String>('created_at_utc')) ??
              DateTime.now().toUtc(),
        ),
    ];
  }

  Future<void> updateMutationStatus({
    required String id,
    required OutboxStatus status,
    String? errorCode,
  }) async {
    await customStatement(
      'UPDATE outbox_mutations SET status = ?, error_code = ? WHERE id = ?',
      [status.wire, errorCode, id],
    );
  }

  Future<void> deleteMutation(String id) async {
    await customStatement('DELETE FROM outbox_mutations WHERE id = ?', [id]);
  }

  Future<void> clearNonPendingMutations(String membershipId) async {
    await customStatement(
      'DELETE FROM outbox_mutations '
      'WHERE membership_id = ? AND status IN (?, ?)',
      [membershipId, OutboxStatus.failed.wire, OutboxStatus.conflict.wire],
    );
  }
}

final offlineStoreProvider = Provider<OfflineStore>((ref) {
  final store = OfflineStore();
  ref.onDispose(store.close);
  return store;
});
