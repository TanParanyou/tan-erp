// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_survey_measurement_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSurveyMeasurementResponse extends SiteSurveyMeasurementResponse {
  @override
  final String? id;
  @override
  final String? siteSurveyAreaId;
  @override
  final String? measurementType;
  @override
  final double? value;
  @override
  final String? unitCode;
  @override
  final String? captureMethod;
  @override
  final String? notes;
  @override
  final int? sortOrder;

  factory _$SiteSurveyMeasurementResponse(
          [void Function(SiteSurveyMeasurementResponseBuilder)? updates]) =>
      (SiteSurveyMeasurementResponseBuilder()..update(updates))._build();

  _$SiteSurveyMeasurementResponse._(
      {this.id,
      this.siteSurveyAreaId,
      this.measurementType,
      this.value,
      this.unitCode,
      this.captureMethod,
      this.notes,
      this.sortOrder})
      : super._();
  @override
  SiteSurveyMeasurementResponse rebuild(
          void Function(SiteSurveyMeasurementResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSurveyMeasurementResponseBuilder toBuilder() =>
      SiteSurveyMeasurementResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSurveyMeasurementResponse &&
        id == other.id &&
        siteSurveyAreaId == other.siteSurveyAreaId &&
        measurementType == other.measurementType &&
        value == other.value &&
        unitCode == other.unitCode &&
        captureMethod == other.captureMethod &&
        notes == other.notes &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, siteSurveyAreaId.hashCode);
    _$hash = $jc(_$hash, measurementType.hashCode);
    _$hash = $jc(_$hash, value.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, captureMethod.hashCode);
    _$hash = $jc(_$hash, notes.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSurveyMeasurementResponse')
          ..add('id', id)
          ..add('siteSurveyAreaId', siteSurveyAreaId)
          ..add('measurementType', measurementType)
          ..add('value', value)
          ..add('unitCode', unitCode)
          ..add('captureMethod', captureMethod)
          ..add('notes', notes)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class SiteSurveyMeasurementResponseBuilder
    implements
        Builder<SiteSurveyMeasurementResponse,
            SiteSurveyMeasurementResponseBuilder> {
  _$SiteSurveyMeasurementResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _siteSurveyAreaId;
  String? get siteSurveyAreaId => _$this._siteSurveyAreaId;
  set siteSurveyAreaId(String? siteSurveyAreaId) =>
      _$this._siteSurveyAreaId = siteSurveyAreaId;

  String? _measurementType;
  String? get measurementType => _$this._measurementType;
  set measurementType(String? measurementType) =>
      _$this._measurementType = measurementType;

  double? _value;
  double? get value => _$this._value;
  set value(double? value) => _$this._value = value;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  String? _captureMethod;
  String? get captureMethod => _$this._captureMethod;
  set captureMethod(String? captureMethod) =>
      _$this._captureMethod = captureMethod;

  String? _notes;
  String? get notes => _$this._notes;
  set notes(String? notes) => _$this._notes = notes;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  SiteSurveyMeasurementResponseBuilder() {
    SiteSurveyMeasurementResponse._defaults(this);
  }

  SiteSurveyMeasurementResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _siteSurveyAreaId = $v.siteSurveyAreaId;
      _measurementType = $v.measurementType;
      _value = $v.value;
      _unitCode = $v.unitCode;
      _captureMethod = $v.captureMethod;
      _notes = $v.notes;
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSurveyMeasurementResponse other) {
    _$v = other as _$SiteSurveyMeasurementResponse;
  }

  @override
  void update(void Function(SiteSurveyMeasurementResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSurveyMeasurementResponse build() => _build();

  _$SiteSurveyMeasurementResponse _build() {
    final _$result = _$v ??
        _$SiteSurveyMeasurementResponse._(
          id: id,
          siteSurveyAreaId: siteSurveyAreaId,
          measurementType: measurementType,
          value: value,
          unitCode: unitCode,
          captureMethod: captureMethod,
          notes: notes,
          sortOrder: sortOrder,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
