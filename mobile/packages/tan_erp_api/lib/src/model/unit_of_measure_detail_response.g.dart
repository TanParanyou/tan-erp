// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'unit_of_measure_detail_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UnitOfMeasureDetailResponse extends UnitOfMeasureDetailResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final String? symbol;
  @override
  final String? dimension;
  @override
  final int? decimalScale;
  @override
  final String? roundingMode;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;

  factory _$UnitOfMeasureDetailResponse(
          [void Function(UnitOfMeasureDetailResponseBuilder)? updates]) =>
      (UnitOfMeasureDetailResponseBuilder()..update(updates))._build();

  _$UnitOfMeasureDetailResponse._(
      {this.id,
      this.organizationId,
      this.code,
      this.name,
      this.symbol,
      this.dimension,
      this.decimalScale,
      this.roundingMode,
      this.status,
      this.rowVersion,
      this.createdAtUtc,
      this.updatedAtUtc})
      : super._();
  @override
  UnitOfMeasureDetailResponse rebuild(
          void Function(UnitOfMeasureDetailResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UnitOfMeasureDetailResponseBuilder toBuilder() =>
      UnitOfMeasureDetailResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UnitOfMeasureDetailResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        code == other.code &&
        name == other.name &&
        symbol == other.symbol &&
        dimension == other.dimension &&
        decimalScale == other.decimalScale &&
        roundingMode == other.roundingMode &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, symbol.hashCode);
    _$hash = $jc(_$hash, dimension.hashCode);
    _$hash = $jc(_$hash, decimalScale.hashCode);
    _$hash = $jc(_$hash, roundingMode.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UnitOfMeasureDetailResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('code', code)
          ..add('name', name)
          ..add('symbol', symbol)
          ..add('dimension', dimension)
          ..add('decimalScale', decimalScale)
          ..add('roundingMode', roundingMode)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class UnitOfMeasureDetailResponseBuilder
    implements
        Builder<UnitOfMeasureDetailResponse,
            UnitOfMeasureDetailResponseBuilder> {
  _$UnitOfMeasureDetailResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  String? _symbol;
  String? get symbol => _$this._symbol;
  set symbol(String? symbol) => _$this._symbol = symbol;

  String? _dimension;
  String? get dimension => _$this._dimension;
  set dimension(String? dimension) => _$this._dimension = dimension;

  int? _decimalScale;
  int? get decimalScale => _$this._decimalScale;
  set decimalScale(int? decimalScale) => _$this._decimalScale = decimalScale;

  String? _roundingMode;
  String? get roundingMode => _$this._roundingMode;
  set roundingMode(String? roundingMode) => _$this._roundingMode = roundingMode;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  UnitOfMeasureDetailResponseBuilder() {
    UnitOfMeasureDetailResponse._defaults(this);
  }

  UnitOfMeasureDetailResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _symbol = $v.symbol;
      _dimension = $v.dimension;
      _decimalScale = $v.decimalScale;
      _roundingMode = $v.roundingMode;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UnitOfMeasureDetailResponse other) {
    _$v = other as _$UnitOfMeasureDetailResponse;
  }

  @override
  void update(void Function(UnitOfMeasureDetailResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UnitOfMeasureDetailResponse build() => _build();

  _$UnitOfMeasureDetailResponse _build() {
    _$UnitOfMeasureDetailResponse _$result;
    try {
      _$result = _$v ??
          _$UnitOfMeasureDetailResponse._(
            id: id,
            organizationId: organizationId,
            code: code,
            name: _name?.build(),
            symbol: symbol,
            dimension: dimension,
            decimalScale: decimalScale,
            roundingMode: roundingMode,
            status: status,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UnitOfMeasureDetailResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
