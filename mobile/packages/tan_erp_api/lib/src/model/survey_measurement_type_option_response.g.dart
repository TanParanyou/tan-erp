// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_measurement_type_option_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyMeasurementTypeOptionResponse
    extends SurveyMeasurementTypeOptionResponse {
  @override
  final String? code;
  @override
  final SurveyLocalizedLabel? name;
  @override
  final String? unitCode;
  @override
  final int? sortOrder;

  factory _$SurveyMeasurementTypeOptionResponse(
          [void Function(SurveyMeasurementTypeOptionResponseBuilder)?
              updates]) =>
      (SurveyMeasurementTypeOptionResponseBuilder()..update(updates))._build();

  _$SurveyMeasurementTypeOptionResponse._(
      {this.code, this.name, this.unitCode, this.sortOrder})
      : super._();
  @override
  SurveyMeasurementTypeOptionResponse rebuild(
          void Function(SurveyMeasurementTypeOptionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyMeasurementTypeOptionResponseBuilder toBuilder() =>
      SurveyMeasurementTypeOptionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyMeasurementTypeOptionResponse &&
        code == other.code &&
        name == other.name &&
        unitCode == other.unitCode &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyMeasurementTypeOptionResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('unitCode', unitCode)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class SurveyMeasurementTypeOptionResponseBuilder
    implements
        Builder<SurveyMeasurementTypeOptionResponse,
            SurveyMeasurementTypeOptionResponseBuilder> {
  _$SurveyMeasurementTypeOptionResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  SurveyLocalizedLabelBuilder? _name;
  SurveyLocalizedLabelBuilder get name =>
      _$this._name ??= SurveyLocalizedLabelBuilder();
  set name(SurveyLocalizedLabelBuilder? name) => _$this._name = name;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  SurveyMeasurementTypeOptionResponseBuilder() {
    SurveyMeasurementTypeOptionResponse._defaults(this);
  }

  SurveyMeasurementTypeOptionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _unitCode = $v.unitCode;
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyMeasurementTypeOptionResponse other) {
    _$v = other as _$SurveyMeasurementTypeOptionResponse;
  }

  @override
  void update(
      void Function(SurveyMeasurementTypeOptionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyMeasurementTypeOptionResponse build() => _build();

  _$SurveyMeasurementTypeOptionResponse _build() {
    _$SurveyMeasurementTypeOptionResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyMeasurementTypeOptionResponse._(
            code: code,
            name: _name?.build(),
            unitCode: unitCode,
            sortOrder: sortOrder,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(r'SurveyMeasurementTypeOptionResponse',
            _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
