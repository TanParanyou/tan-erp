import 'package:flutter/foundation.dart';

/// Stock balance per warehouse for a scanned item.
@immutable
class ScannedStockRow {
  const ScannedStockRow({
    required this.warehouseName,
    required this.onHand,
    required this.reserved,
    required this.available,
  });

  factory ScannedStockRow.fromJson(Map<String, Object?> json) {
    return ScannedStockRow(
      warehouseName: json['warehouseName'] as String? ?? '',
      onHand: (json['onHand'] as num?)?.toDouble() ?? 0.0,
      reserved: (json['reserved'] as num?)?.toDouble() ?? 0.0,
      available: (json['available'] as num?)?.toDouble() ?? 0.0,
    );
  }

  final String warehouseName;
  final double onHand;
  final double reserved;
  final double available;

  Map<String, Object?> toJson() => <String, Object?>{
    'warehouseName': warehouseName,
    'onHand': onHand,
    'reserved': reserved,
    'available': available,
  };
}

/// Aggregated item details and stock rows from barcode or code lookup.
@immutable
class ScannedItemResult {
  const ScannedItemResult({
    required this.itemId,
    required this.code,
    required this.nameTh,
    required this.nameEn,
    required this.itemType,
    required this.baseUnitCode,
    required this.baseUnitName,
    required this.scannedAtUtc,
    this.barcodeValue,
    this.packagingLevel,
    this.stocks = const <ScannedStockRow>[],
  });

  factory ScannedItemResult.fromJson(Map<String, Object?> json) {
    final rawStocks = json['stocks'];
    final parsedStocks = <ScannedStockRow>[];
    if (rawStocks is List<Object?>) {
      for (final s in rawStocks) {
        if (s is Map<String, Object?>) {
          parsedStocks.add(ScannedStockRow.fromJson(s));
        }
      }
    }

    final rawScannedAt = json['scannedAtUtc'] as String?;
    final scannedAt = rawScannedAt != null
        ? DateTime.tryParse(rawScannedAt)?.toUtc() ?? DateTime.now().toUtc()
        : DateTime.now().toUtc();

    return ScannedItemResult(
      itemId: json['itemId'] as String? ?? '',
      code: json['code'] as String? ?? '',
      nameTh: json['nameTh'] as String? ?? '',
      nameEn: json['nameEn'] as String? ?? '',
      itemType: json['itemType'] as String? ?? '',
      baseUnitCode: json['baseUnitCode'] as String? ?? '',
      baseUnitName: json['baseUnitName'] as String? ?? '',
      barcodeValue: json['barcodeValue'] as String?,
      packagingLevel: json['packagingLevel'] as String?,
      stocks: parsedStocks,
      scannedAtUtc: scannedAt,
    );
  }

  final String itemId;
  final String code;
  final String nameTh;
  final String nameEn;
  final String itemType;
  final String baseUnitCode;
  final String baseUnitName;
  final String? barcodeValue;
  final String? packagingLevel;
  final List<ScannedStockRow> stocks;
  final DateTime scannedAtUtc;

  String localizedName(String languageCode) {
    if (languageCode == 'en' && nameEn.trim().isNotEmpty) {
      return nameEn;
    }
    return nameTh.isNotEmpty ? nameTh : (nameEn.isNotEmpty ? nameEn : code);
  }

  Map<String, Object?> toJson() => <String, Object?>{
    'itemId': itemId,
    'code': code,
    'nameTh': nameTh,
    'nameEn': nameEn,
    'itemType': itemType,
    'baseUnitCode': baseUnitCode,
    'baseUnitName': baseUnitName,
    'barcodeValue': barcodeValue,
    'packagingLevel': packagingLevel,
    'stocks': [for (final s in stocks) s.toJson()],
    'scannedAtUtc': scannedAtUtc.toIso8601String(),
  };
}
