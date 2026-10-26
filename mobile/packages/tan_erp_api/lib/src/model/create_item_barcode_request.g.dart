// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_item_barcode_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateItemBarcodeRequest extends CreateItemBarcodeRequest {
  @override
  final String identifierType;
  @override
  final String value;
  @override
  final String unitId;
  @override
  final String quantityInBaseUnit;
  @override
  final String packagingLevel;
  @override
  final bool? isPrimary;

  factory _$CreateItemBarcodeRequest(
          [void Function(CreateItemBarcodeRequestBuilder)? updates]) =>
      (CreateItemBarcodeRequestBuilder()..update(updates))._build();

  _$CreateItemBarcodeRequest._(
      {required this.identifierType,
      required this.value,
      required this.unitId,
      required this.quantityInBaseUnit,
      required this.packagingLevel,
      this.isPrimary})
      : super._();
  @override
  CreateItemBarcodeRequest rebuild(
          void Function(CreateItemBarcodeRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateItemBarcodeRequestBuilder toBuilder() =>
      CreateItemBarcodeRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateItemBarcodeRequest &&
        identifierType == other.identifierType &&
        value == other.value &&
        unitId == other.unitId &&
        quantityInBaseUnit == other.quantityInBaseUnit &&
        packagingLevel == other.packagingLevel &&
        isPrimary == other.isPrimary;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, identifierType.hashCode);
    _$hash = $jc(_$hash, value.hashCode);
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, quantityInBaseUnit.hashCode);
    _$hash = $jc(_$hash, packagingLevel.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateItemBarcodeRequest')
          ..add('identifierType', identifierType)
          ..add('value', value)
          ..add('unitId', unitId)
          ..add('quantityInBaseUnit', quantityInBaseUnit)
          ..add('packagingLevel', packagingLevel)
          ..add('isPrimary', isPrimary))
        .toString();
  }
}

class CreateItemBarcodeRequestBuilder
    implements
        Builder<CreateItemBarcodeRequest, CreateItemBarcodeRequestBuilder> {
  _$CreateItemBarcodeRequest? _$v;

  String? _identifierType;
  String? get identifierType => _$this._identifierType;
  set identifierType(String? identifierType) =>
      _$this._identifierType = identifierType;

  String? _value;
  String? get value => _$this._value;
  set value(String? value) => _$this._value = value;

  String? _unitId;
  String? get unitId => _$this._unitId;
  set unitId(String? unitId) => _$this._unitId = unitId;

  String? _quantityInBaseUnit;
  String? get quantityInBaseUnit => _$this._quantityInBaseUnit;
  set quantityInBaseUnit(String? quantityInBaseUnit) =>
      _$this._quantityInBaseUnit = quantityInBaseUnit;

  String? _packagingLevel;
  String? get packagingLevel => _$this._packagingLevel;
  set packagingLevel(String? packagingLevel) =>
      _$this._packagingLevel = packagingLevel;

  bool? _isPrimary;
  bool? get isPrimary => _$this._isPrimary;
  set isPrimary(bool? isPrimary) => _$this._isPrimary = isPrimary;

  CreateItemBarcodeRequestBuilder() {
    CreateItemBarcodeRequest._defaults(this);
  }

  CreateItemBarcodeRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _identifierType = $v.identifierType;
      _value = $v.value;
      _unitId = $v.unitId;
      _quantityInBaseUnit = $v.quantityInBaseUnit;
      _packagingLevel = $v.packagingLevel;
      _isPrimary = $v.isPrimary;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateItemBarcodeRequest other) {
    _$v = other as _$CreateItemBarcodeRequest;
  }

  @override
  void update(void Function(CreateItemBarcodeRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateItemBarcodeRequest build() => _build();

  _$CreateItemBarcodeRequest _build() {
    final _$result = _$v ??
        _$CreateItemBarcodeRequest._(
          identifierType: BuiltValueNullFieldError.checkNotNull(
              identifierType, r'CreateItemBarcodeRequest', 'identifierType'),
          value: BuiltValueNullFieldError.checkNotNull(
              value, r'CreateItemBarcodeRequest', 'value'),
          unitId: BuiltValueNullFieldError.checkNotNull(
              unitId, r'CreateItemBarcodeRequest', 'unitId'),
          quantityInBaseUnit: BuiltValueNullFieldError.checkNotNull(
              quantityInBaseUnit,
              r'CreateItemBarcodeRequest',
              'quantityInBaseUnit'),
          packagingLevel: BuiltValueNullFieldError.checkNotNull(
              packagingLevel, r'CreateItemBarcodeRequest', 'packagingLevel'),
          isPrimary: isPrimary,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
