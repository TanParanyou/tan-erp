// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pricing_template_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PricingTemplateRequest extends PricingTemplateRequest {
  @override
  final String? code;
  @override
  final String? workType;
  @override
  final String? name;
  @override
  final String? measurementRule;
  @override
  final String? unitCode;
  @override
  final double? referenceRate;
  @override
  final double? minimumCharge;
  @override
  final double? baseRangeRate;
  @override
  final double? maxRangeRate;
  @override
  final double? roundingStep;
  @override
  final int? validityDays;
  @override
  final double? taxRate;
  @override
  final String? taxDisplay;
  @override
  final double? directShareLimit;
  @override
  final Date? effectiveFrom;
  @override
  final Date? effectiveTo;
  @override
  final BuiltList<GradeRequest>? grades;
  @override
  final BuiltList<ComplexityRequest>? complexities;
  @override
  final BuiltList<AddOnRequest>? addOns;
  @override
  final double? lowConfidenceModifier;
  @override
  final double? mediumConfidenceModifier;
  @override
  final double? customMaterialModifier;
  @override
  final BuiltList<String>? assumptions;
  @override
  final BuiltList<String>? exclusions;

  factory _$PricingTemplateRequest(
          [void Function(PricingTemplateRequestBuilder)? updates]) =>
      (PricingTemplateRequestBuilder()..update(updates))._build();

  _$PricingTemplateRequest._(
      {this.code,
      this.workType,
      this.name,
      this.measurementRule,
      this.unitCode,
      this.referenceRate,
      this.minimumCharge,
      this.baseRangeRate,
      this.maxRangeRate,
      this.roundingStep,
      this.validityDays,
      this.taxRate,
      this.taxDisplay,
      this.directShareLimit,
      this.effectiveFrom,
      this.effectiveTo,
      this.grades,
      this.complexities,
      this.addOns,
      this.lowConfidenceModifier,
      this.mediumConfidenceModifier,
      this.customMaterialModifier,
      this.assumptions,
      this.exclusions})
      : super._();
  @override
  PricingTemplateRequest rebuild(
          void Function(PricingTemplateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PricingTemplateRequestBuilder toBuilder() =>
      PricingTemplateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PricingTemplateRequest &&
        code == other.code &&
        workType == other.workType &&
        name == other.name &&
        measurementRule == other.measurementRule &&
        unitCode == other.unitCode &&
        referenceRate == other.referenceRate &&
        minimumCharge == other.minimumCharge &&
        baseRangeRate == other.baseRangeRate &&
        maxRangeRate == other.maxRangeRate &&
        roundingStep == other.roundingStep &&
        validityDays == other.validityDays &&
        taxRate == other.taxRate &&
        taxDisplay == other.taxDisplay &&
        directShareLimit == other.directShareLimit &&
        effectiveFrom == other.effectiveFrom &&
        effectiveTo == other.effectiveTo &&
        grades == other.grades &&
        complexities == other.complexities &&
        addOns == other.addOns &&
        lowConfidenceModifier == other.lowConfidenceModifier &&
        mediumConfidenceModifier == other.mediumConfidenceModifier &&
        customMaterialModifier == other.customMaterialModifier &&
        assumptions == other.assumptions &&
        exclusions == other.exclusions;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, workType.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, measurementRule.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, referenceRate.hashCode);
    _$hash = $jc(_$hash, minimumCharge.hashCode);
    _$hash = $jc(_$hash, baseRangeRate.hashCode);
    _$hash = $jc(_$hash, maxRangeRate.hashCode);
    _$hash = $jc(_$hash, roundingStep.hashCode);
    _$hash = $jc(_$hash, validityDays.hashCode);
    _$hash = $jc(_$hash, taxRate.hashCode);
    _$hash = $jc(_$hash, taxDisplay.hashCode);
    _$hash = $jc(_$hash, directShareLimit.hashCode);
    _$hash = $jc(_$hash, effectiveFrom.hashCode);
    _$hash = $jc(_$hash, effectiveTo.hashCode);
    _$hash = $jc(_$hash, grades.hashCode);
    _$hash = $jc(_$hash, complexities.hashCode);
    _$hash = $jc(_$hash, addOns.hashCode);
    _$hash = $jc(_$hash, lowConfidenceModifier.hashCode);
    _$hash = $jc(_$hash, mediumConfidenceModifier.hashCode);
    _$hash = $jc(_$hash, customMaterialModifier.hashCode);
    _$hash = $jc(_$hash, assumptions.hashCode);
    _$hash = $jc(_$hash, exclusions.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PricingTemplateRequest')
          ..add('code', code)
          ..add('workType', workType)
          ..add('name', name)
          ..add('measurementRule', measurementRule)
          ..add('unitCode', unitCode)
          ..add('referenceRate', referenceRate)
          ..add('minimumCharge', minimumCharge)
          ..add('baseRangeRate', baseRangeRate)
          ..add('maxRangeRate', maxRangeRate)
          ..add('roundingStep', roundingStep)
          ..add('validityDays', validityDays)
          ..add('taxRate', taxRate)
          ..add('taxDisplay', taxDisplay)
          ..add('directShareLimit', directShareLimit)
          ..add('effectiveFrom', effectiveFrom)
          ..add('effectiveTo', effectiveTo)
          ..add('grades', grades)
          ..add('complexities', complexities)
          ..add('addOns', addOns)
          ..add('lowConfidenceModifier', lowConfidenceModifier)
          ..add('mediumConfidenceModifier', mediumConfidenceModifier)
          ..add('customMaterialModifier', customMaterialModifier)
          ..add('assumptions', assumptions)
          ..add('exclusions', exclusions))
        .toString();
  }
}

class PricingTemplateRequestBuilder
    implements Builder<PricingTemplateRequest, PricingTemplateRequestBuilder> {
  _$PricingTemplateRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _workType;
  String? get workType => _$this._workType;
  set workType(String? workType) => _$this._workType = workType;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _measurementRule;
  String? get measurementRule => _$this._measurementRule;
  set measurementRule(String? measurementRule) =>
      _$this._measurementRule = measurementRule;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  double? _referenceRate;
  double? get referenceRate => _$this._referenceRate;
  set referenceRate(double? referenceRate) =>
      _$this._referenceRate = referenceRate;

  double? _minimumCharge;
  double? get minimumCharge => _$this._minimumCharge;
  set minimumCharge(double? minimumCharge) =>
      _$this._minimumCharge = minimumCharge;

  double? _baseRangeRate;
  double? get baseRangeRate => _$this._baseRangeRate;
  set baseRangeRate(double? baseRangeRate) =>
      _$this._baseRangeRate = baseRangeRate;

  double? _maxRangeRate;
  double? get maxRangeRate => _$this._maxRangeRate;
  set maxRangeRate(double? maxRangeRate) => _$this._maxRangeRate = maxRangeRate;

  double? _roundingStep;
  double? get roundingStep => _$this._roundingStep;
  set roundingStep(double? roundingStep) => _$this._roundingStep = roundingStep;

  int? _validityDays;
  int? get validityDays => _$this._validityDays;
  set validityDays(int? validityDays) => _$this._validityDays = validityDays;

  double? _taxRate;
  double? get taxRate => _$this._taxRate;
  set taxRate(double? taxRate) => _$this._taxRate = taxRate;

  String? _taxDisplay;
  String? get taxDisplay => _$this._taxDisplay;
  set taxDisplay(String? taxDisplay) => _$this._taxDisplay = taxDisplay;

  double? _directShareLimit;
  double? get directShareLimit => _$this._directShareLimit;
  set directShareLimit(double? directShareLimit) =>
      _$this._directShareLimit = directShareLimit;

  Date? _effectiveFrom;
  Date? get effectiveFrom => _$this._effectiveFrom;
  set effectiveFrom(Date? effectiveFrom) =>
      _$this._effectiveFrom = effectiveFrom;

  Date? _effectiveTo;
  Date? get effectiveTo => _$this._effectiveTo;
  set effectiveTo(Date? effectiveTo) => _$this._effectiveTo = effectiveTo;

  ListBuilder<GradeRequest>? _grades;
  ListBuilder<GradeRequest> get grades =>
      _$this._grades ??= ListBuilder<GradeRequest>();
  set grades(ListBuilder<GradeRequest>? grades) => _$this._grades = grades;

  ListBuilder<ComplexityRequest>? _complexities;
  ListBuilder<ComplexityRequest> get complexities =>
      _$this._complexities ??= ListBuilder<ComplexityRequest>();
  set complexities(ListBuilder<ComplexityRequest>? complexities) =>
      _$this._complexities = complexities;

  ListBuilder<AddOnRequest>? _addOns;
  ListBuilder<AddOnRequest> get addOns =>
      _$this._addOns ??= ListBuilder<AddOnRequest>();
  set addOns(ListBuilder<AddOnRequest>? addOns) => _$this._addOns = addOns;

  double? _lowConfidenceModifier;
  double? get lowConfidenceModifier => _$this._lowConfidenceModifier;
  set lowConfidenceModifier(double? lowConfidenceModifier) =>
      _$this._lowConfidenceModifier = lowConfidenceModifier;

  double? _mediumConfidenceModifier;
  double? get mediumConfidenceModifier => _$this._mediumConfidenceModifier;
  set mediumConfidenceModifier(double? mediumConfidenceModifier) =>
      _$this._mediumConfidenceModifier = mediumConfidenceModifier;

  double? _customMaterialModifier;
  double? get customMaterialModifier => _$this._customMaterialModifier;
  set customMaterialModifier(double? customMaterialModifier) =>
      _$this._customMaterialModifier = customMaterialModifier;

  ListBuilder<String>? _assumptions;
  ListBuilder<String> get assumptions =>
      _$this._assumptions ??= ListBuilder<String>();
  set assumptions(ListBuilder<String>? assumptions) =>
      _$this._assumptions = assumptions;

  ListBuilder<String>? _exclusions;
  ListBuilder<String> get exclusions =>
      _$this._exclusions ??= ListBuilder<String>();
  set exclusions(ListBuilder<String>? exclusions) =>
      _$this._exclusions = exclusions;

  PricingTemplateRequestBuilder() {
    PricingTemplateRequest._defaults(this);
  }

  PricingTemplateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _workType = $v.workType;
      _name = $v.name;
      _measurementRule = $v.measurementRule;
      _unitCode = $v.unitCode;
      _referenceRate = $v.referenceRate;
      _minimumCharge = $v.minimumCharge;
      _baseRangeRate = $v.baseRangeRate;
      _maxRangeRate = $v.maxRangeRate;
      _roundingStep = $v.roundingStep;
      _validityDays = $v.validityDays;
      _taxRate = $v.taxRate;
      _taxDisplay = $v.taxDisplay;
      _directShareLimit = $v.directShareLimit;
      _effectiveFrom = $v.effectiveFrom;
      _effectiveTo = $v.effectiveTo;
      _grades = $v.grades?.toBuilder();
      _complexities = $v.complexities?.toBuilder();
      _addOns = $v.addOns?.toBuilder();
      _lowConfidenceModifier = $v.lowConfidenceModifier;
      _mediumConfidenceModifier = $v.mediumConfidenceModifier;
      _customMaterialModifier = $v.customMaterialModifier;
      _assumptions = $v.assumptions?.toBuilder();
      _exclusions = $v.exclusions?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PricingTemplateRequest other) {
    _$v = other as _$PricingTemplateRequest;
  }

  @override
  void update(void Function(PricingTemplateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PricingTemplateRequest build() => _build();

  _$PricingTemplateRequest _build() {
    _$PricingTemplateRequest _$result;
    try {
      _$result = _$v ??
          _$PricingTemplateRequest._(
            code: code,
            workType: workType,
            name: name,
            measurementRule: measurementRule,
            unitCode: unitCode,
            referenceRate: referenceRate,
            minimumCharge: minimumCharge,
            baseRangeRate: baseRangeRate,
            maxRangeRate: maxRangeRate,
            roundingStep: roundingStep,
            validityDays: validityDays,
            taxRate: taxRate,
            taxDisplay: taxDisplay,
            directShareLimit: directShareLimit,
            effectiveFrom: effectiveFrom,
            effectiveTo: effectiveTo,
            grades: _grades?.build(),
            complexities: _complexities?.build(),
            addOns: _addOns?.build(),
            lowConfidenceModifier: lowConfidenceModifier,
            mediumConfidenceModifier: mediumConfidenceModifier,
            customMaterialModifier: customMaterialModifier,
            assumptions: _assumptions?.build(),
            exclusions: _exclusions?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'grades';
        _grades?.build();
        _$failedField = 'complexities';
        _complexities?.build();
        _$failedField = 'addOns';
        _addOns?.build();

        _$failedField = 'assumptions';
        _assumptions?.build();
        _$failedField = 'exclusions';
        _exclusions?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PricingTemplateRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
