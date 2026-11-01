// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_value_report_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockValueReportRowResponse extends StockValueReportRowResponse {
  @override
  final String? warehouseId;
  @override
  final String? warehouseCode;
  @override
  final String? warehouseName;
  @override
  final String? branchCode;
  @override
  final String? itemId;
  @override
  final String? itemCode;
  @override
  final String? itemNameTh;
  @override
  final String? itemNameEn;
  @override
  final double? onHand;
  @override
  final double? reserved;
  @override
  final double? available;
  @override
  final double? totalValue;

  factory _$StockValueReportRowResponse(
          [void Function(StockValueReportRowResponseBuilder)? updates]) =>
      (StockValueReportRowResponseBuilder()..update(updates))._build();

  _$StockValueReportRowResponse._(
      {this.warehouseId,
      this.warehouseCode,
      this.warehouseName,
      this.branchCode,
      this.itemId,
      this.itemCode,
      this.itemNameTh,
      this.itemNameEn,
      this.onHand,
      this.reserved,
      this.available,
      this.totalValue})
      : super._();
  @override
  StockValueReportRowResponse rebuild(
          void Function(StockValueReportRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockValueReportRowResponseBuilder toBuilder() =>
      StockValueReportRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockValueReportRowResponse &&
        warehouseId == other.warehouseId &&
        warehouseCode == other.warehouseCode &&
        warehouseName == other.warehouseName &&
        branchCode == other.branchCode &&
        itemId == other.itemId &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        itemNameEn == other.itemNameEn &&
        onHand == other.onHand &&
        reserved == other.reserved &&
        available == other.available &&
        totalValue == other.totalValue;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, warehouseCode.hashCode);
    _$hash = $jc(_$hash, warehouseName.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, itemNameTh.hashCode);
    _$hash = $jc(_$hash, itemNameEn.hashCode);
    _$hash = $jc(_$hash, onHand.hashCode);
    _$hash = $jc(_$hash, reserved.hashCode);
    _$hash = $jc(_$hash, available.hashCode);
    _$hash = $jc(_$hash, totalValue.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockValueReportRowResponse')
          ..add('warehouseId', warehouseId)
          ..add('warehouseCode', warehouseCode)
          ..add('warehouseName', warehouseName)
          ..add('branchCode', branchCode)
          ..add('itemId', itemId)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('itemNameEn', itemNameEn)
          ..add('onHand', onHand)
          ..add('reserved', reserved)
          ..add('available', available)
          ..add('totalValue', totalValue))
        .toString();
  }
}

class StockValueReportRowResponseBuilder
    implements
        Builder<StockValueReportRowResponse,
            StockValueReportRowResponseBuilder> {
  _$StockValueReportRowResponse? _$v;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  String? _warehouseCode;
  String? get warehouseCode => _$this._warehouseCode;
  set warehouseCode(String? warehouseCode) =>
      _$this._warehouseCode = warehouseCode;

  String? _warehouseName;
  String? get warehouseName => _$this._warehouseName;
  set warehouseName(String? warehouseName) =>
      _$this._warehouseName = warehouseName;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _itemCode;
  String? get itemCode => _$this._itemCode;
  set itemCode(String? itemCode) => _$this._itemCode = itemCode;

  String? _itemNameTh;
  String? get itemNameTh => _$this._itemNameTh;
  set itemNameTh(String? itemNameTh) => _$this._itemNameTh = itemNameTh;

  String? _itemNameEn;
  String? get itemNameEn => _$this._itemNameEn;
  set itemNameEn(String? itemNameEn) => _$this._itemNameEn = itemNameEn;

  double? _onHand;
  double? get onHand => _$this._onHand;
  set onHand(double? onHand) => _$this._onHand = onHand;

  double? _reserved;
  double? get reserved => _$this._reserved;
  set reserved(double? reserved) => _$this._reserved = reserved;

  double? _available;
  double? get available => _$this._available;
  set available(double? available) => _$this._available = available;

  double? _totalValue;
  double? get totalValue => _$this._totalValue;
  set totalValue(double? totalValue) => _$this._totalValue = totalValue;

  StockValueReportRowResponseBuilder() {
    StockValueReportRowResponse._defaults(this);
  }

  StockValueReportRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _warehouseId = $v.warehouseId;
      _warehouseCode = $v.warehouseCode;
      _warehouseName = $v.warehouseName;
      _branchCode = $v.branchCode;
      _itemId = $v.itemId;
      _itemCode = $v.itemCode;
      _itemNameTh = $v.itemNameTh;
      _itemNameEn = $v.itemNameEn;
      _onHand = $v.onHand;
      _reserved = $v.reserved;
      _available = $v.available;
      _totalValue = $v.totalValue;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockValueReportRowResponse other) {
    _$v = other as _$StockValueReportRowResponse;
  }

  @override
  void update(void Function(StockValueReportRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockValueReportRowResponse build() => _build();

  _$StockValueReportRowResponse _build() {
    final _$result = _$v ??
        _$StockValueReportRowResponse._(
          warehouseId: warehouseId,
          warehouseCode: warehouseCode,
          warehouseName: warehouseName,
          branchCode: branchCode,
          itemId: itemId,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          itemNameEn: itemNameEn,
          onHand: onHand,
          reserved: reserved,
          available: available,
          totalValue: totalValue,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
