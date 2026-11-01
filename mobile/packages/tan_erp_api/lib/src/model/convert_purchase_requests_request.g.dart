// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'convert_purchase_requests_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ConvertPurchaseRequestsRequest extends ConvertPurchaseRequestsRequest {
  @override
  final BuiltList<String>? purchaseRequestIds;
  @override
  final String? supplierId;
  @override
  final Date? expectedDeliveryDate;
  @override
  final String? note;
  @override
  final BuiltList<PurchaseRequestUnitPriceOverrideRequest>? unitPriceOverrides;

  factory _$ConvertPurchaseRequestsRequest(
          [void Function(ConvertPurchaseRequestsRequestBuilder)? updates]) =>
      (ConvertPurchaseRequestsRequestBuilder()..update(updates))._build();

  _$ConvertPurchaseRequestsRequest._(
      {this.purchaseRequestIds,
      this.supplierId,
      this.expectedDeliveryDate,
      this.note,
      this.unitPriceOverrides})
      : super._();
  @override
  ConvertPurchaseRequestsRequest rebuild(
          void Function(ConvertPurchaseRequestsRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ConvertPurchaseRequestsRequestBuilder toBuilder() =>
      ConvertPurchaseRequestsRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ConvertPurchaseRequestsRequest &&
        purchaseRequestIds == other.purchaseRequestIds &&
        supplierId == other.supplierId &&
        expectedDeliveryDate == other.expectedDeliveryDate &&
        note == other.note &&
        unitPriceOverrides == other.unitPriceOverrides;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, purchaseRequestIds.hashCode);
    _$hash = $jc(_$hash, supplierId.hashCode);
    _$hash = $jc(_$hash, expectedDeliveryDate.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, unitPriceOverrides.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ConvertPurchaseRequestsRequest')
          ..add('purchaseRequestIds', purchaseRequestIds)
          ..add('supplierId', supplierId)
          ..add('expectedDeliveryDate', expectedDeliveryDate)
          ..add('note', note)
          ..add('unitPriceOverrides', unitPriceOverrides))
        .toString();
  }
}

class ConvertPurchaseRequestsRequestBuilder
    implements
        Builder<ConvertPurchaseRequestsRequest,
            ConvertPurchaseRequestsRequestBuilder> {
  _$ConvertPurchaseRequestsRequest? _$v;

  ListBuilder<String>? _purchaseRequestIds;
  ListBuilder<String> get purchaseRequestIds =>
      _$this._purchaseRequestIds ??= ListBuilder<String>();
  set purchaseRequestIds(ListBuilder<String>? purchaseRequestIds) =>
      _$this._purchaseRequestIds = purchaseRequestIds;

  String? _supplierId;
  String? get supplierId => _$this._supplierId;
  set supplierId(String? supplierId) => _$this._supplierId = supplierId;

  Date? _expectedDeliveryDate;
  Date? get expectedDeliveryDate => _$this._expectedDeliveryDate;
  set expectedDeliveryDate(Date? expectedDeliveryDate) =>
      _$this._expectedDeliveryDate = expectedDeliveryDate;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ListBuilder<PurchaseRequestUnitPriceOverrideRequest>? _unitPriceOverrides;
  ListBuilder<PurchaseRequestUnitPriceOverrideRequest> get unitPriceOverrides =>
      _$this._unitPriceOverrides ??=
          ListBuilder<PurchaseRequestUnitPriceOverrideRequest>();
  set unitPriceOverrides(
          ListBuilder<PurchaseRequestUnitPriceOverrideRequest>?
              unitPriceOverrides) =>
      _$this._unitPriceOverrides = unitPriceOverrides;

  ConvertPurchaseRequestsRequestBuilder() {
    ConvertPurchaseRequestsRequest._defaults(this);
  }

  ConvertPurchaseRequestsRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _purchaseRequestIds = $v.purchaseRequestIds?.toBuilder();
      _supplierId = $v.supplierId;
      _expectedDeliveryDate = $v.expectedDeliveryDate;
      _note = $v.note;
      _unitPriceOverrides = $v.unitPriceOverrides?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ConvertPurchaseRequestsRequest other) {
    _$v = other as _$ConvertPurchaseRequestsRequest;
  }

  @override
  void update(void Function(ConvertPurchaseRequestsRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ConvertPurchaseRequestsRequest build() => _build();

  _$ConvertPurchaseRequestsRequest _build() {
    _$ConvertPurchaseRequestsRequest _$result;
    try {
      _$result = _$v ??
          _$ConvertPurchaseRequestsRequest._(
            purchaseRequestIds: _purchaseRequestIds?.build(),
            supplierId: supplierId,
            expectedDeliveryDate: expectedDeliveryDate,
            note: note,
            unitPriceOverrides: _unitPriceOverrides?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'purchaseRequestIds';
        _purchaseRequestIds?.build();

        _$failedField = 'unitPriceOverrides';
        _unitPriceOverrides?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ConvertPurchaseRequestsRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
