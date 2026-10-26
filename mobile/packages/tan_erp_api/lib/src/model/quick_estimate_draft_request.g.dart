// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quick_estimate_draft_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuickEstimateDraftRequest extends QuickEstimateDraftRequest {
  @override
  final String? templateId;
  @override
  final String? propertyType;
  @override
  final String? roomOrArea;
  @override
  final String? gradeCode;
  @override
  final BuiltList<String>? complexityCodes;
  @override
  final BuiltList<String>? addOnCodes;
  @override
  final String? measurementConfidence;
  @override
  final bool? customMaterial;
  @override
  final BuiltList<MeasurementRequest>? measurements;

  factory _$QuickEstimateDraftRequest(
          [void Function(QuickEstimateDraftRequestBuilder)? updates]) =>
      (QuickEstimateDraftRequestBuilder()..update(updates))._build();

  _$QuickEstimateDraftRequest._(
      {this.templateId,
      this.propertyType,
      this.roomOrArea,
      this.gradeCode,
      this.complexityCodes,
      this.addOnCodes,
      this.measurementConfidence,
      this.customMaterial,
      this.measurements})
      : super._();
  @override
  QuickEstimateDraftRequest rebuild(
          void Function(QuickEstimateDraftRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuickEstimateDraftRequestBuilder toBuilder() =>
      QuickEstimateDraftRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuickEstimateDraftRequest &&
        templateId == other.templateId &&
        propertyType == other.propertyType &&
        roomOrArea == other.roomOrArea &&
        gradeCode == other.gradeCode &&
        complexityCodes == other.complexityCodes &&
        addOnCodes == other.addOnCodes &&
        measurementConfidence == other.measurementConfidence &&
        customMaterial == other.customMaterial &&
        measurements == other.measurements;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, templateId.hashCode);
    _$hash = $jc(_$hash, propertyType.hashCode);
    _$hash = $jc(_$hash, roomOrArea.hashCode);
    _$hash = $jc(_$hash, gradeCode.hashCode);
    _$hash = $jc(_$hash, complexityCodes.hashCode);
    _$hash = $jc(_$hash, addOnCodes.hashCode);
    _$hash = $jc(_$hash, measurementConfidence.hashCode);
    _$hash = $jc(_$hash, customMaterial.hashCode);
    _$hash = $jc(_$hash, measurements.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuickEstimateDraftRequest')
          ..add('templateId', templateId)
          ..add('propertyType', propertyType)
          ..add('roomOrArea', roomOrArea)
          ..add('gradeCode', gradeCode)
          ..add('complexityCodes', complexityCodes)
          ..add('addOnCodes', addOnCodes)
          ..add('measurementConfidence', measurementConfidence)
          ..add('customMaterial', customMaterial)
          ..add('measurements', measurements))
        .toString();
  }
}

class QuickEstimateDraftRequestBuilder
    implements
        Builder<QuickEstimateDraftRequest, QuickEstimateDraftRequestBuilder> {
  _$QuickEstimateDraftRequest? _$v;

  String? _templateId;
  String? get templateId => _$this._templateId;
  set templateId(String? templateId) => _$this._templateId = templateId;

  String? _propertyType;
  String? get propertyType => _$this._propertyType;
  set propertyType(String? propertyType) => _$this._propertyType = propertyType;

  String? _roomOrArea;
  String? get roomOrArea => _$this._roomOrArea;
  set roomOrArea(String? roomOrArea) => _$this._roomOrArea = roomOrArea;

  String? _gradeCode;
  String? get gradeCode => _$this._gradeCode;
  set gradeCode(String? gradeCode) => _$this._gradeCode = gradeCode;

  ListBuilder<String>? _complexityCodes;
  ListBuilder<String> get complexityCodes =>
      _$this._complexityCodes ??= ListBuilder<String>();
  set complexityCodes(ListBuilder<String>? complexityCodes) =>
      _$this._complexityCodes = complexityCodes;

  ListBuilder<String>? _addOnCodes;
  ListBuilder<String> get addOnCodes =>
      _$this._addOnCodes ??= ListBuilder<String>();
  set addOnCodes(ListBuilder<String>? addOnCodes) =>
      _$this._addOnCodes = addOnCodes;

  String? _measurementConfidence;
  String? get measurementConfidence => _$this._measurementConfidence;
  set measurementConfidence(String? measurementConfidence) =>
      _$this._measurementConfidence = measurementConfidence;

  bool? _customMaterial;
  bool? get customMaterial => _$this._customMaterial;
  set customMaterial(bool? customMaterial) =>
      _$this._customMaterial = customMaterial;

  ListBuilder<MeasurementRequest>? _measurements;
  ListBuilder<MeasurementRequest> get measurements =>
      _$this._measurements ??= ListBuilder<MeasurementRequest>();
  set measurements(ListBuilder<MeasurementRequest>? measurements) =>
      _$this._measurements = measurements;

  QuickEstimateDraftRequestBuilder() {
    QuickEstimateDraftRequest._defaults(this);
  }

  QuickEstimateDraftRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _templateId = $v.templateId;
      _propertyType = $v.propertyType;
      _roomOrArea = $v.roomOrArea;
      _gradeCode = $v.gradeCode;
      _complexityCodes = $v.complexityCodes?.toBuilder();
      _addOnCodes = $v.addOnCodes?.toBuilder();
      _measurementConfidence = $v.measurementConfidence;
      _customMaterial = $v.customMaterial;
      _measurements = $v.measurements?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuickEstimateDraftRequest other) {
    _$v = other as _$QuickEstimateDraftRequest;
  }

  @override
  void update(void Function(QuickEstimateDraftRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuickEstimateDraftRequest build() => _build();

  _$QuickEstimateDraftRequest _build() {
    _$QuickEstimateDraftRequest _$result;
    try {
      _$result = _$v ??
          _$QuickEstimateDraftRequest._(
            templateId: templateId,
            propertyType: propertyType,
            roomOrArea: roomOrArea,
            gradeCode: gradeCode,
            complexityCodes: _complexityCodes?.build(),
            addOnCodes: _addOnCodes?.build(),
            measurementConfidence: measurementConfidence,
            customMaterial: customMaterial,
            measurements: _measurements?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'complexityCodes';
        _complexityCodes?.build();
        _$failedField = 'addOnCodes';
        _addOnCodes?.build();

        _$failedField = 'measurements';
        _measurements?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuickEstimateDraftRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
