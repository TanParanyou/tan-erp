// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderListItemResponse extends PurchaseOrderListItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final double? totalAmount;
  @override
  final String? supplierCode;
  @override
  final String? supplierNameTh;
  @override
  final String? projectCode;
  @override
  final Date? expectedDeliveryDate;
  @override
  final DateTime? createdAtUtc;

  factory _$PurchaseOrderListItemResponse(
          [void Function(PurchaseOrderListItemResponseBuilder)? updates]) =>
      (PurchaseOrderListItemResponseBuilder()..update(updates))._build();

  _$PurchaseOrderListItemResponse._(
      {this.id,
      this.number,
      this.status,
      this.totalAmount,
      this.supplierCode,
      this.supplierNameTh,
      this.projectCode,
      this.expectedDeliveryDate,
      this.createdAtUtc})
      : super._();
  @override
  PurchaseOrderListItemResponse rebuild(
          void Function(PurchaseOrderListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderListItemResponseBuilder toBuilder() =>
      PurchaseOrderListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderListItemResponse &&
        id == other.id &&
        number == other.number &&
        status == other.status &&
        totalAmount == other.totalAmount &&
        supplierCode == other.supplierCode &&
        supplierNameTh == other.supplierNameTh &&
        projectCode == other.projectCode &&
        expectedDeliveryDate == other.expectedDeliveryDate &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, totalAmount.hashCode);
    _$hash = $jc(_$hash, supplierCode.hashCode);
    _$hash = $jc(_$hash, supplierNameTh.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, expectedDeliveryDate.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('status', status)
          ..add('totalAmount', totalAmount)
          ..add('supplierCode', supplierCode)
          ..add('supplierNameTh', supplierNameTh)
          ..add('projectCode', projectCode)
          ..add('expectedDeliveryDate', expectedDeliveryDate)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class PurchaseOrderListItemResponseBuilder
    implements
        Builder<PurchaseOrderListItemResponse,
            PurchaseOrderListItemResponseBuilder> {
  _$PurchaseOrderListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _totalAmount;
  double? get totalAmount => _$this._totalAmount;
  set totalAmount(double? totalAmount) => _$this._totalAmount = totalAmount;

  String? _supplierCode;
  String? get supplierCode => _$this._supplierCode;
  set supplierCode(String? supplierCode) => _$this._supplierCode = supplierCode;

  String? _supplierNameTh;
  String? get supplierNameTh => _$this._supplierNameTh;
  set supplierNameTh(String? supplierNameTh) =>
      _$this._supplierNameTh = supplierNameTh;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  Date? _expectedDeliveryDate;
  Date? get expectedDeliveryDate => _$this._expectedDeliveryDate;
  set expectedDeliveryDate(Date? expectedDeliveryDate) =>
      _$this._expectedDeliveryDate = expectedDeliveryDate;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  PurchaseOrderListItemResponseBuilder() {
    PurchaseOrderListItemResponse._defaults(this);
  }

  PurchaseOrderListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _status = $v.status;
      _totalAmount = $v.totalAmount;
      _supplierCode = $v.supplierCode;
      _supplierNameTh = $v.supplierNameTh;
      _projectCode = $v.projectCode;
      _expectedDeliveryDate = $v.expectedDeliveryDate;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderListItemResponse other) {
    _$v = other as _$PurchaseOrderListItemResponse;
  }

  @override
  void update(void Function(PurchaseOrderListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderListItemResponse build() => _build();

  _$PurchaseOrderListItemResponse _build() {
    final _$result = _$v ??
        _$PurchaseOrderListItemResponse._(
          id: id,
          number: number,
          status: status,
          totalAmount: totalAmount,
          supplierCode: supplierCode,
          supplierNameTh: supplierNameTh,
          projectCode: projectCode,
          expectedDeliveryDate: expectedDeliveryDate,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
