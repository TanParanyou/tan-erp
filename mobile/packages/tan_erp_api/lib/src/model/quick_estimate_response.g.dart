// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quick_estimate_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuickEstimateResponse extends QuickEstimateResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final String? customerId;
  @override
  final String? opportunityId;
  @override
  final QuickEstimateTemplateRefResponse? template;
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
  final BuiltList<MeasurementResponse>? measurements;
  @override
  final int? currentCalculationVersion;
  @override
  final CalculationResponse? currentCalculation;
  @override
  final BuiltList<ReviewResponse>? reviews;
  @override
  final BuiltList<ShareResponse>? shares;
  @override
  final BuiltList<ConversionResponse>? conversions;
  @override
  final QePersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;
  @override
  final String? rowVersion;

  factory _$QuickEstimateResponse(
          [void Function(QuickEstimateResponseBuilder)? updates]) =>
      (QuickEstimateResponseBuilder()..update(updates))._build();

  _$QuickEstimateResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.status,
      this.customerId,
      this.opportunityId,
      this.template,
      this.propertyType,
      this.roomOrArea,
      this.gradeCode,
      this.complexityCodes,
      this.addOnCodes,
      this.measurementConfidence,
      this.customMaterial,
      this.measurements,
      this.currentCalculationVersion,
      this.currentCalculation,
      this.reviews,
      this.shares,
      this.conversions,
      this.createdBy,
      this.createdAtUtc,
      this.updatedAtUtc,
      this.rowVersion})
      : super._();
  @override
  QuickEstimateResponse rebuild(
          void Function(QuickEstimateResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuickEstimateResponseBuilder toBuilder() =>
      QuickEstimateResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuickEstimateResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        status == other.status &&
        customerId == other.customerId &&
        opportunityId == other.opportunityId &&
        template == other.template &&
        propertyType == other.propertyType &&
        roomOrArea == other.roomOrArea &&
        gradeCode == other.gradeCode &&
        complexityCodes == other.complexityCodes &&
        addOnCodes == other.addOnCodes &&
        measurementConfidence == other.measurementConfidence &&
        customMaterial == other.customMaterial &&
        measurements == other.measurements &&
        currentCalculationVersion == other.currentCalculationVersion &&
        currentCalculation == other.currentCalculation &&
        reviews == other.reviews &&
        shares == other.shares &&
        conversions == other.conversions &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, template.hashCode);
    _$hash = $jc(_$hash, propertyType.hashCode);
    _$hash = $jc(_$hash, roomOrArea.hashCode);
    _$hash = $jc(_$hash, gradeCode.hashCode);
    _$hash = $jc(_$hash, complexityCodes.hashCode);
    _$hash = $jc(_$hash, addOnCodes.hashCode);
    _$hash = $jc(_$hash, measurementConfidence.hashCode);
    _$hash = $jc(_$hash, customMaterial.hashCode);
    _$hash = $jc(_$hash, measurements.hashCode);
    _$hash = $jc(_$hash, currentCalculationVersion.hashCode);
    _$hash = $jc(_$hash, currentCalculation.hashCode);
    _$hash = $jc(_$hash, reviews.hashCode);
    _$hash = $jc(_$hash, shares.hashCode);
    _$hash = $jc(_$hash, conversions.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuickEstimateResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('status', status)
          ..add('customerId', customerId)
          ..add('opportunityId', opportunityId)
          ..add('template', template)
          ..add('propertyType', propertyType)
          ..add('roomOrArea', roomOrArea)
          ..add('gradeCode', gradeCode)
          ..add('complexityCodes', complexityCodes)
          ..add('addOnCodes', addOnCodes)
          ..add('measurementConfidence', measurementConfidence)
          ..add('customMaterial', customMaterial)
          ..add('measurements', measurements)
          ..add('currentCalculationVersion', currentCalculationVersion)
          ..add('currentCalculation', currentCalculation)
          ..add('reviews', reviews)
          ..add('shares', shares)
          ..add('conversions', conversions)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class QuickEstimateResponseBuilder
    implements Builder<QuickEstimateResponse, QuickEstimateResponseBuilder> {
  _$QuickEstimateResponse? _$v;

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

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  QuickEstimateTemplateRefResponseBuilder? _template;
  QuickEstimateTemplateRefResponseBuilder get template =>
      _$this._template ??= QuickEstimateTemplateRefResponseBuilder();
  set template(QuickEstimateTemplateRefResponseBuilder? template) =>
      _$this._template = template;

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

  ListBuilder<MeasurementResponse>? _measurements;
  ListBuilder<MeasurementResponse> get measurements =>
      _$this._measurements ??= ListBuilder<MeasurementResponse>();
  set measurements(ListBuilder<MeasurementResponse>? measurements) =>
      _$this._measurements = measurements;

  int? _currentCalculationVersion;
  int? get currentCalculationVersion => _$this._currentCalculationVersion;
  set currentCalculationVersion(int? currentCalculationVersion) =>
      _$this._currentCalculationVersion = currentCalculationVersion;

  CalculationResponseBuilder? _currentCalculation;
  CalculationResponseBuilder get currentCalculation =>
      _$this._currentCalculation ??= CalculationResponseBuilder();
  set currentCalculation(CalculationResponseBuilder? currentCalculation) =>
      _$this._currentCalculation = currentCalculation;

  ListBuilder<ReviewResponse>? _reviews;
  ListBuilder<ReviewResponse> get reviews =>
      _$this._reviews ??= ListBuilder<ReviewResponse>();
  set reviews(ListBuilder<ReviewResponse>? reviews) =>
      _$this._reviews = reviews;

  ListBuilder<ShareResponse>? _shares;
  ListBuilder<ShareResponse> get shares =>
      _$this._shares ??= ListBuilder<ShareResponse>();
  set shares(ListBuilder<ShareResponse>? shares) => _$this._shares = shares;

  ListBuilder<ConversionResponse>? _conversions;
  ListBuilder<ConversionResponse> get conversions =>
      _$this._conversions ??= ListBuilder<ConversionResponse>();
  set conversions(ListBuilder<ConversionResponse>? conversions) =>
      _$this._conversions = conversions;

  QePersonResponseBuilder? _createdBy;
  QePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= QePersonResponseBuilder();
  set createdBy(QePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  QuickEstimateResponseBuilder() {
    QuickEstimateResponse._defaults(this);
  }

  QuickEstimateResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _status = $v.status;
      _customerId = $v.customerId;
      _opportunityId = $v.opportunityId;
      _template = $v.template?.toBuilder();
      _propertyType = $v.propertyType;
      _roomOrArea = $v.roomOrArea;
      _gradeCode = $v.gradeCode;
      _complexityCodes = $v.complexityCodes?.toBuilder();
      _addOnCodes = $v.addOnCodes?.toBuilder();
      _measurementConfidence = $v.measurementConfidence;
      _customMaterial = $v.customMaterial;
      _measurements = $v.measurements?.toBuilder();
      _currentCalculationVersion = $v.currentCalculationVersion;
      _currentCalculation = $v.currentCalculation?.toBuilder();
      _reviews = $v.reviews?.toBuilder();
      _shares = $v.shares?.toBuilder();
      _conversions = $v.conversions?.toBuilder();
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuickEstimateResponse other) {
    _$v = other as _$QuickEstimateResponse;
  }

  @override
  void update(void Function(QuickEstimateResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuickEstimateResponse build() => _build();

  _$QuickEstimateResponse _build() {
    _$QuickEstimateResponse _$result;
    try {
      _$result = _$v ??
          _$QuickEstimateResponse._(
            id: id,
            branchId: branchId,
            number: number,
            status: status,
            customerId: customerId,
            opportunityId: opportunityId,
            template: _template?.build(),
            propertyType: propertyType,
            roomOrArea: roomOrArea,
            gradeCode: gradeCode,
            complexityCodes: _complexityCodes?.build(),
            addOnCodes: _addOnCodes?.build(),
            measurementConfidence: measurementConfidence,
            customMaterial: customMaterial,
            measurements: _measurements?.build(),
            currentCalculationVersion: currentCalculationVersion,
            currentCalculation: _currentCalculation?.build(),
            reviews: _reviews?.build(),
            shares: _shares?.build(),
            conversions: _conversions?.build(),
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'template';
        _template?.build();

        _$failedField = 'complexityCodes';
        _complexityCodes?.build();
        _$failedField = 'addOnCodes';
        _addOnCodes?.build();

        _$failedField = 'measurements';
        _measurements?.build();

        _$failedField = 'currentCalculation';
        _currentCalculation?.build();
        _$failedField = 'reviews';
        _reviews?.build();
        _$failedField = 'shares';
        _shares?.build();
        _$failedField = 'conversions';
        _conversions?.build();
        _$failedField = 'createdBy';
        _createdBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuickEstimateResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
