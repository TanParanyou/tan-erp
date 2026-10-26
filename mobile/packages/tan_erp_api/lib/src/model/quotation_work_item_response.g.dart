// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_work_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationWorkItemResponse extends QuotationWorkItemResponse {
  @override
  final String? code;
  @override
  final String? description;
  @override
  final String? descriptionTh;
  @override
  final String? descriptionEn;
  @override
  final double? quantity;
  @override
  final String? unitCode;
  @override
  final double? unitPrice;
  @override
  final double? lineTotal;

  factory _$QuotationWorkItemResponse(
          [void Function(QuotationWorkItemResponseBuilder)? updates]) =>
      (QuotationWorkItemResponseBuilder()..update(updates))._build();

  _$QuotationWorkItemResponse._(
      {this.code,
      this.description,
      this.descriptionTh,
      this.descriptionEn,
      this.quantity,
      this.unitCode,
      this.unitPrice,
      this.lineTotal})
      : super._();
  @override
  QuotationWorkItemResponse rebuild(
          void Function(QuotationWorkItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationWorkItemResponseBuilder toBuilder() =>
      QuotationWorkItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationWorkItemResponse &&
        code == other.code &&
        description == other.description &&
        descriptionTh == other.descriptionTh &&
        descriptionEn == other.descriptionEn &&
        quantity == other.quantity &&
        unitCode == other.unitCode &&
        unitPrice == other.unitPrice &&
        lineTotal == other.lineTotal;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, descriptionTh.hashCode);
    _$hash = $jc(_$hash, descriptionEn.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, lineTotal.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationWorkItemResponse')
          ..add('code', code)
          ..add('description', description)
          ..add('descriptionTh', descriptionTh)
          ..add('descriptionEn', descriptionEn)
          ..add('quantity', quantity)
          ..add('unitCode', unitCode)
          ..add('unitPrice', unitPrice)
          ..add('lineTotal', lineTotal))
        .toString();
  }
}

class QuotationWorkItemResponseBuilder
    implements
        Builder<QuotationWorkItemResponse, QuotationWorkItemResponseBuilder> {
  _$QuotationWorkItemResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  String? _descriptionTh;
  String? get descriptionTh => _$this._descriptionTh;
  set descriptionTh(String? descriptionTh) =>
      _$this._descriptionTh = descriptionTh;

  String? _descriptionEn;
  String? get descriptionEn => _$this._descriptionEn;
  set descriptionEn(String? descriptionEn) =>
      _$this._descriptionEn = descriptionEn;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  double? _lineTotal;
  double? get lineTotal => _$this._lineTotal;
  set lineTotal(double? lineTotal) => _$this._lineTotal = lineTotal;

  QuotationWorkItemResponseBuilder() {
    QuotationWorkItemResponse._defaults(this);
  }

  QuotationWorkItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _description = $v.description;
      _descriptionTh = $v.descriptionTh;
      _descriptionEn = $v.descriptionEn;
      _quantity = $v.quantity;
      _unitCode = $v.unitCode;
      _unitPrice = $v.unitPrice;
      _lineTotal = $v.lineTotal;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationWorkItemResponse other) {
    _$v = other as _$QuotationWorkItemResponse;
  }

  @override
  void update(void Function(QuotationWorkItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationWorkItemResponse build() => _build();

  _$QuotationWorkItemResponse _build() {
    final _$result = _$v ??
        _$QuotationWorkItemResponse._(
          code: code,
          description: description,
          descriptionTh: descriptionTh,
          descriptionEn: descriptionEn,
          quantity: quantity,
          unitCode: unitCode,
          unitPrice: unitPrice,
          lineTotal: lineTotal,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
