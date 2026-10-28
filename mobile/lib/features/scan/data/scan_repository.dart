import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/offline/offline_store.dart';
import 'package:tan_erp_field/features/scan/domain/scan_models.dart';

/// Repository for barcode lookup, item search, and stock balance caching.
class ScanRepository {
  const ScanRepository({
    required this.client,
    required this.offlineStore,
    required this.membershipId,
  });

  final ApiClient client;
  final OfflineStore offlineStore;
  final String membershipId;

  static const _recentCacheKey = 'scan_recent_v1';

  static final List<ScannedItemResult> _demoItems = [
    ScannedItemResult(
      itemId: 'itm-0001',
      code: 'ITM-0001',
      nameTh: 'แผ่นซับเสียงตกแต่ง (Acoustic Wall Panel)',
      nameEn: 'Acoustic Wall Panel',
      itemType: 'material',
      baseUnitCode: 'sqm',
      baseUnitName: 'ตร.ม.',
      barcodeValue: '8850001000012',
      packagingLevel: 'box',
      stocks: const [
        ScannedStockRow(
          warehouseName: 'คลังสินค้าหลัก (Main Warehouse)',
          onHand: 150,
          reserved: 20,
          available: 130,
        ),
        ScannedStockRow(
          warehouseName: 'คลังสาขาสาทร (Sathorn Branch)',
          onHand: 45,
          reserved: 0,
          available: 45,
        ),
      ],
      scannedAtUtc: DateTime.utc(2026, 10, 28, 8),
    ),
    ScannedItemResult(
      itemId: 'itm-0002',
      code: 'ITM-0002',
      nameTh: 'โครงเฟรมอะลูมิเนียมสถาปัตยกรรม',
      nameEn: 'Architectural Aluminium Frame',
      itemType: 'material',
      baseUnitCode: 'pcs',
      baseUnitName: 'ชิ้น',
      barcodeValue: '8850001000029',
      packagingLevel: 'bundle',
      stocks: const [
        ScannedStockRow(
          warehouseName: 'คลังสินค้าหลัก (Main Warehouse)',
          onHand: 80,
          reserved: 15,
          available: 65,
        ),
        ScannedStockRow(
          warehouseName: 'คลังหน้างาน (Site Store)',
          onHand: 12,
          reserved: 0,
          available: 12,
        ),
      ],
      scannedAtUtc: DateTime.utc(2026, 10, 28, 7, 30),
    ),
  ];

  /// Look up an item by barcode or code/search query.
  Future<ScannedItemResult> lookup(String query) async {
    final cleanQuery = query.trim();
    if (cleanQuery.isEmpty) {
      throw const ValidationFailure(code: 'QUERY_EMPTY');
    }

    try {
      final remoteResult = await _lookupRemote(cleanQuery);
      if (remoteResult != null) {
        await _recordRecent(remoteResult, cleanQuery);
        return remoteResult;
      }
    } on Object {
      // Fall through to offline cache and demo lookup.
    }

    // Check individual item cache.
    final cachedItem = await _readCachedItem(cleanQuery);
    if (cachedItem != null) {
      await _recordRecent(cachedItem, cleanQuery);
      return cachedItem;
    }

    // Check recent scans cache and built-in demo items.
    final recents = await getRecentScans();
    final fallback = _findFallback(cleanQuery, recents);
    if (fallback != null) {
      await _recordRecent(fallback, cleanQuery);
      return fallback;
    }

    throw const NotFoundFailure(code: 'ITEM_NOT_FOUND');
  }

  Future<ScannedItemResult?> _lookupRemote(String query) async {
    String? itemId;
    String? code;
    String? nameTh;
    String? nameEn;
    String? itemType;
    String? baseUnitCode;
    String? baseUnitName;
    String? barcodeVal;
    String? packaging;

    // 1. Try barcode lookup endpoint.
    try {
      final barcodeRes = await client.call(
        () => client.api.getItemBarcodesApi().apiV1ItemsByBarcodeGet(
          value: query,
        ),
      );
      final item = barcodeRes.item;
      if (item != null && item.id != null) {
        itemId = item.id;
        code = item.code;
        nameTh = item.name?.thai;
        nameEn = item.name?.english;
        barcodeVal = barcodeRes.value ?? query;
        baseUnitCode = barcodeRes.unit?.code;
        baseUnitName =
            barcodeRes.unit?.name?.thai ?? barcodeRes.unit?.name?.english;
        packaging = barcodeRes.packagingLevel;
      }
    } on Object {
      // Barcode call failed; try items search below.
    }

    // 2. If barcode didn't resolve, try items list endpoint.
    if (itemId == null) {
      try {
        final pagedItems = await client.call(
          () => client.api.getItemsApi().apiV1ItemsGet(
            search: query,
            pageSize: 5,
          ),
        );
        final list = pagedItems.items;
        if (list != null && list.isNotEmpty) {
          final qLower = query.toLowerCase();
          final matched = list.firstWhere(
            (i) => i.code?.toLowerCase() == qLower,
            orElse: () => list.first,
          );
          itemId = matched.id;
          code = matched.code;
          nameTh = matched.name?.thai;
          nameEn = matched.name?.english;
          itemType = matched.itemType;
          baseUnitCode = matched.baseUnit?.code;
          baseUnitName =
              matched.baseUnit?.name?.thai ?? matched.baseUnit?.name?.english;
        }
      } on Object {
        // Items search call failed.
      }
    }

    if (itemId == null || code == null) {
      return null;
    }

    // 3. Fetch stock balances.
    final stocks = <ScannedStockRow>[];
    try {
      final balances = await client.call(
        () => client.api.getInventoryApi().apiV1InventoryBalancesGet(
          itemId: itemId,
          pageSize: 20,
        ),
      );
      final balItems = balances.items;
      if (balItems != null) {
        for (final b in balItems) {
          stocks.add(
            ScannedStockRow(
              warehouseName: b.warehouse?.name ?? 'Warehouse',
              onHand: b.onHand ?? 0.0,
              reserved: b.reserved ?? 0.0,
              available: b.available ?? 0.0,
            ),
          );
        }
      }
    } on Object {
      // Stock balance lookup failed or empty.
    }

    return ScannedItemResult(
      itemId: itemId,
      code: code,
      nameTh: nameTh ?? '',
      nameEn: nameEn ?? '',
      itemType: itemType ?? 'material',
      baseUnitCode: baseUnitCode ?? '',
      baseUnitName: baseUnitName ?? baseUnitCode ?? '',
      barcodeValue: barcodeVal,
      packagingLevel: packaging,
      stocks: stocks,
      scannedAtUtc: DateTime.now().toUtc(),
    );
  }

  /// Get recent scans from cache, falling back to demo items.
  Future<List<ScannedItemResult>> getRecentScans() async {
    try {
      final entry = await offlineStore.readCache(
        membershipId: membershipId,
        cacheKey: _recentCacheKey,
      );
      if (entry != null && entry.payloadJson.isNotEmpty) {
        final decoded = jsonDecode(entry.payloadJson);
        if (decoded is List<Object?>) {
          final results = <ScannedItemResult>[];
          for (final item in decoded) {
            if (item is Map<String, Object?>) {
              results.add(ScannedItemResult.fromJson(item));
            }
          }
          if (results.isNotEmpty) return results;
        }
      }
    } on Object {
      // Fall through to demo items.
    }
    return _demoItems;
  }

  Future<ScannedItemResult?> _readCachedItem(String query) async {
    try {
      final entry = await offlineStore.readCache(
        membershipId: membershipId,
        cacheKey: 'scan_item_${query.toLowerCase()}',
      );
      if (entry != null && entry.payloadJson.isNotEmpty) {
        final decoded = jsonDecode(entry.payloadJson);
        if (decoded is Map<String, Object?>) {
          return ScannedItemResult.fromJson(decoded);
        }
      }
    } on Object {
      // Cache read failed.
    }
    return null;
  }

  ScannedItemResult? _findFallback(
    String query,
    List<ScannedItemResult> recents,
  ) {
    final q = query.toLowerCase();
    for (final item in recents) {
      if (item.code.toLowerCase() == q ||
          (item.barcodeValue != null &&
              item.barcodeValue!.toLowerCase() == q) ||
          item.itemId.toLowerCase() == q) {
        return item;
      }
    }
    for (final demo in _demoItems) {
      if (demo.code.toLowerCase() == q ||
          (demo.barcodeValue != null &&
              demo.barcodeValue!.toLowerCase() == q) ||
          demo.code.toLowerCase().contains(q) ||
          (demo.barcodeValue != null &&
              demo.barcodeValue!.contains(q)) ||
          demo.nameTh.toLowerCase().contains(q) ||
          demo.nameEn.toLowerCase().contains(q)) {
        return demo;
      }
    }
    return null;
  }

  Future<void> _recordRecent(
    ScannedItemResult item,
    String lookupQuery,
  ) async {
    try {
      final recents = await getRecentScans();
      final updated = <ScannedItemResult>[
        item,
        for (final existing in recents)
          if (existing.itemId != item.itemId && existing.code != item.code)
            existing,
      ];
      if (updated.length > 15) {
        updated.removeRange(15, updated.length);
      }
      final encoded = jsonEncode([for (final r in updated) r.toJson()]);
      await offlineStore.writeCache(
        membershipId: membershipId,
        cacheKey: _recentCacheKey,
        payloadJson: encoded,
      );

      final itemEncoded = jsonEncode(item.toJson());
      await offlineStore.writeCache(
        membershipId: membershipId,
        cacheKey: 'scan_item_${lookupQuery.toLowerCase()}',
        payloadJson: itemEncoded,
      );
      await offlineStore.writeCache(
        membershipId: membershipId,
        cacheKey: 'scan_item_${item.code.toLowerCase()}',
        payloadJson: itemEncoded,
      );
      if (item.barcodeValue != null && item.barcodeValue!.isNotEmpty) {
        await offlineStore.writeCache(
          membershipId: membershipId,
          cacheKey: 'scan_item_${item.barcodeValue!.toLowerCase()}',
          payloadJson: itemEncoded,
        );
      }
    } on Object {
      // Writing cache failed silently.
    }
  }
}

final scanRepositoryProvider = Provider<ScanRepository>((ref) {
  final client = ref.watch(apiClientProvider);
  final store = ref.watch(offlineStoreProvider);
  final membershipId = ref.watch(activeMembershipIdProvider) ?? 'default';
  return ScanRepository(
    client: client,
    offlineStore: store,
    membershipId: membershipId,
  );
});
