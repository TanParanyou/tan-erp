// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_survey_measurement_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateSurveyMeasurementRequest extends UpdateSurveyMeasurementRequest {
  @override
  final String? id;
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

  factory _$UpdateSurveyMeasurementRequest(
          [void Function(UpdateSurveyMeasurementRequestBuilder)? updates]) =>
      (UpdateSurveyMeasurementRequestBuilder()..update(updates))._build();

  _$UpdateSurveyMeasurementRequest._(
      {this.id,
      this.measurementType,
      this.value,
      this.unitCode,
      this.captureMethod,
      this.notes,
      this.sortOrder})
      : super._();
  @override
  UpdateSurveyMeasurementRequest rebuild(
          void Function(UpdateSurveyMeasurementRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateSurveyMeasurementRequestBuilder toBuilder() =>
      UpdateSurveyMeasurementRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateSurveyMeasurementRequest &&
        id == other.id &&
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
    return (newBuiltValueToStringHelper(r'UpdateSurveyMeasurementRequest')
          ..add('id', id)
          ..add('measurementType', measurementType)
          ..add('value', value)
          ..add('unitCode', unitCode)
          ..add('captureMethod', captureMethod)
          ..add('notes', notes)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class UpdateSurveyMeasurementRequestBuilder
    implements
        Builder<UpdateSurveyMeasurementRequest,
            UpdateSurveyMeasurementRequestBuilder> {
  _$UpdateSurveyMeasurementRequest? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  UpdateSurveyMeasurementRequestBuilder() {
    UpdateSurveyMeasurementRequest._defaults(this);
  }

  UpdateSurveyMeasurementRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
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
  void replace(UpdateSurveyMeasurementRequest other) {
    _$v = other as _$UpdateSurveyMeasurementRequest;
  }

  @override
  void update(void Function(UpdateSurveyMeasurementRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateSurveyMeasurementRequest build() => _build();

  _$UpdateSurveyMeasurementRequest _build() {
    final _$result = _$v ??
        _$UpdateSurveyMeasurementRequest._(
          id: id,
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
