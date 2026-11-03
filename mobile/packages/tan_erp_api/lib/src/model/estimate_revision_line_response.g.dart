// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_revision_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateRevisionLineResponse extends EstimateRevisionLineResponse {
  @override
  final String sectionCode;
  @override
  final String workItemCode;
  @override
  final String descriptionTh;
  @override
  final double quantity;
  @override
  final String unitCode;
  @override
  final double totalCost;
  @override
  final double totalSellingPrice;

  factory _$EstimateRevisionLineResponse(
          [void Function(EstimateRevisionLineResponseBuilder)? updates]) =>
      (EstimateRevisionLineResponseBuilder()..update(updates))._build();

  _$EstimateRevisionLineResponse._(
      {required this.sectionCode,
      required this.workItemCode,
      required this.descriptionTh,
      required this.quantity,
      required this.unitCode,
      required this.totalCost,
      required this.totalSellingPrice})
      : super._();
  @override
  EstimateRevisionLineResponse rebuild(
          void Function(EstimateRevisionLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateRevisionLineResponseBuilder toBuilder() =>
      EstimateRevisionLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateRevisionLineResponse &&
        sectionCode == other.sectionCode &&
        workItemCode == other.workItemCode &&
        descriptionTh == other.descriptionTh &&
        quantity == other.quantity &&
        unitCode == other.unitCode &&
        totalCost == other.totalCost &&
        totalSellingPrice == other.totalSellingPrice;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sectionCode.hashCode);
    _$hash = $jc(_$hash, workItemCode.hashCode);
    _$hash = $jc(_$hash, descriptionTh.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, totalCost.hashCode);
    _$hash = $jc(_$hash, totalSellingPrice.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateRevisionLineResponse')
          ..add('sectionCode', sectionCode)
          ..add('workItemCode', workItemCode)
          ..add('descriptionTh', descriptionTh)
          ..add('quantity', quantity)
          ..add('unitCode', unitCode)
          ..add('totalCost', totalCost)
          ..add('totalSellingPrice', totalSellingPrice))
        .toString();
  }
}

class EstimateRevisionLineResponseBuilder
    implements
        Builder<EstimateRevisionLineResponse,
            EstimateRevisionLineResponseBuilder> {
  _$EstimateRevisionLineResponse? _$v;

  String? _sectionCode;
  String? get sectionCode => _$this._sectionCode;
  set sectionCode(String? sectionCode) => _$this._sectionCode = sectionCode;

  String? _workItemCode;
  String? get workItemCode => _$this._workItemCode;
  set workItemCode(String? workItemCode) => _$this._workItemCode = workItemCode;

  String? _descriptionTh;
  String? get descriptionTh => _$this._descriptionTh;
  set descriptionTh(String? descriptionTh) =>
      _$this._descriptionTh = descriptionTh;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  double? _totalCost;
  double? get totalCost => _$this._totalCost;
  set totalCost(double? totalCost) => _$this._totalCost = totalCost;

  double? _totalSellingPrice;
  double? get totalSellingPrice => _$this._totalSellingPrice;
  set totalSellingPrice(double? totalSellingPrice) =>
      _$this._totalSellingPrice = totalSellingPrice;

  EstimateRevisionLineResponseBuilder() {
    EstimateRevisionLineResponse._defaults(this);
  }

  EstimateRevisionLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sectionCode = $v.sectionCode;
      _workItemCode = $v.workItemCode;
      _descriptionTh = $v.descriptionTh;
      _quantity = $v.quantity;
      _unitCode = $v.unitCode;
      _totalCost = $v.totalCost;
      _totalSellingPrice = $v.totalSellingPrice;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateRevisionLineResponse other) {
    _$v = other as _$EstimateRevisionLineResponse;
  }

  @override
  void update(void Function(EstimateRevisionLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateRevisionLineResponse build() => _build();

  _$EstimateRevisionLineResponse _build() {
    final _$result = _$v ??
        _$EstimateRevisionLineResponse._(
          sectionCode: BuiltValueNullFieldError.checkNotNull(
              sectionCode, r'EstimateRevisionLineResponse', 'sectionCode'),
          workItemCode: BuiltValueNullFieldError.checkNotNull(
              workItemCode, r'EstimateRevisionLineResponse', 'workItemCode'),
          descriptionTh: BuiltValueNullFieldError.checkNotNull(
              descriptionTh, r'EstimateRevisionLineResponse', 'descriptionTh'),
          quantity: BuiltValueNullFieldError.checkNotNull(
              quantity, r'EstimateRevisionLineResponse', 'quantity'),
          unitCode: BuiltValueNullFieldError.checkNotNull(
              unitCode, r'EstimateRevisionLineResponse', 'unitCode'),
          totalCost: BuiltValueNullFieldError.checkNotNull(
              totalCost, r'EstimateRevisionLineResponse', 'totalCost'),
          totalSellingPrice: BuiltValueNullFieldError.checkNotNull(
              totalSellingPrice,
              r'EstimateRevisionLineResponse',
              'totalSellingPrice'),
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
