// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestListItemResponse
    extends PurchaseRequestListItemResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final double? totalEstimatedAmount;
  @override
  final String? projectCode;
  @override
  final Date? requiredByDate;
  @override
  final String? convertedPurchaseOrderId;
  @override
  final String? convertedPurchaseOrderNumber;
  @override
  final DateTime? createdAtUtc;

  factory _$PurchaseRequestListItemResponse(
          [void Function(PurchaseRequestListItemResponseBuilder)? updates]) =>
      (PurchaseRequestListItemResponseBuilder()..update(updates))._build();

  _$PurchaseRequestListItemResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.status,
      this.totalEstimatedAmount,
      this.projectCode,
      this.requiredByDate,
      this.convertedPurchaseOrderId,
      this.convertedPurchaseOrderNumber,
      this.createdAtUtc})
      : super._();
  @override
  PurchaseRequestListItemResponse rebuild(
          void Function(PurchaseRequestListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestListItemResponseBuilder toBuilder() =>
      PurchaseRequestListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestListItemResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        status == other.status &&
        totalEstimatedAmount == other.totalEstimatedAmount &&
        projectCode == other.projectCode &&
        requiredByDate == other.requiredByDate &&
        convertedPurchaseOrderId == other.convertedPurchaseOrderId &&
        convertedPurchaseOrderNumber == other.convertedPurchaseOrderNumber &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, totalEstimatedAmount.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, requiredByDate.hashCode);
    _$hash = $jc(_$hash, convertedPurchaseOrderId.hashCode);
    _$hash = $jc(_$hash, convertedPurchaseOrderNumber.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseRequestListItemResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('status', status)
          ..add('totalEstimatedAmount', totalEstimatedAmount)
          ..add('projectCode', projectCode)
          ..add('requiredByDate', requiredByDate)
          ..add('convertedPurchaseOrderId', convertedPurchaseOrderId)
          ..add('convertedPurchaseOrderNumber', convertedPurchaseOrderNumber)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class PurchaseRequestListItemResponseBuilder
    implements
        Builder<PurchaseRequestListItemResponse,
            PurchaseRequestListItemResponseBuilder> {
  _$PurchaseRequestListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _totalEstimatedAmount;
  double? get totalEstimatedAmount => _$this._totalEstimatedAmount;
  set totalEstimatedAmount(double? totalEstimatedAmount) =>
      _$this._totalEstimatedAmount = totalEstimatedAmount;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  Date? _requiredByDate;
  Date? get requiredByDate => _$this._requiredByDate;
  set requiredByDate(Date? requiredByDate) =>
      _$this._requiredByDate = requiredByDate;

  String? _convertedPurchaseOrderId;
  String? get convertedPurchaseOrderId => _$this._convertedPurchaseOrderId;
  set convertedPurchaseOrderId(String? convertedPurchaseOrderId) =>
      _$this._convertedPurchaseOrderId = convertedPurchaseOrderId;

  String? _convertedPurchaseOrderNumber;
  String? get convertedPurchaseOrderNumber =>
      _$this._convertedPurchaseOrderNumber;
  set convertedPurchaseOrderNumber(String? convertedPurchaseOrderNumber) =>
      _$this._convertedPurchaseOrderNumber = convertedPurchaseOrderNumber;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  PurchaseRequestListItemResponseBuilder() {
    PurchaseRequestListItemResponse._defaults(this);
  }

  PurchaseRequestListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _status = $v.status;
      _totalEstimatedAmount = $v.totalEstimatedAmount;
      _projectCode = $v.projectCode;
      _requiredByDate = $v.requiredByDate;
      _convertedPurchaseOrderId = $v.convertedPurchaseOrderId;
      _convertedPurchaseOrderNumber = $v.convertedPurchaseOrderNumber;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestListItemResponse other) {
    _$v = other as _$PurchaseRequestListItemResponse;
  }

  @override
  void update(void Function(PurchaseRequestListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestListItemResponse build() => _build();

  _$PurchaseRequestListItemResponse _build() {
    final _$result = _$v ??
        _$PurchaseRequestListItemResponse._(
          id: id,
          branchId: branchId,
          number: number,
          status: status,
          totalEstimatedAmount: totalEstimatedAmount,
          projectCode: projectCode,
          requiredByDate: requiredByDate,
          convertedPurchaseOrderId: convertedPurchaseOrderId,
          convertedPurchaseOrderNumber: convertedPurchaseOrderNumber,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
