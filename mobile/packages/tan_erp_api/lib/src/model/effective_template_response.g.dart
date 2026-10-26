// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'effective_template_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EffectiveTemplateResponse extends EffectiveTemplateResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final int? version;
  @override
  final String? workType;
  @override
  final String? name;
  @override
  final String? status;
  @override
  final String? measurementRule;
  @override
  final String? unitCode;
  @override
  final String? taxDisplay;
  @override
  final int? validityDays;
  @override
  final BuiltList<EffectiveOptionResponse>? grades;
  @override
  final BuiltList<EffectiveOptionResponse>? complexities;
  @override
  final BuiltList<EffectiveAddOnResponse>? addOns;
  @override
  final BuiltList<String>? assumptions;
  @override
  final BuiltList<String>? exclusions;

  factory _$EffectiveTemplateResponse(
          [void Function(EffectiveTemplateResponseBuilder)? updates]) =>
      (EffectiveTemplateResponseBuilder()..update(updates))._build();

  _$EffectiveTemplateResponse._(
      {this.id,
      this.code,
      this.version,
      this.workType,
      this.name,
      this.status,
      this.measurementRule,
      this.unitCode,
      this.taxDisplay,
      this.validityDays,
      this.grades,
      this.complexities,
      this.addOns,
      this.assumptions,
      this.exclusions})
      : super._();
  @override
  EffectiveTemplateResponse rebuild(
          void Function(EffectiveTemplateResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EffectiveTemplateResponseBuilder toBuilder() =>
      EffectiveTemplateResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EffectiveTemplateResponse &&
        id == other.id &&
        code == other.code &&
        version == other.version &&
        workType == other.workType &&
        name == other.name &&
        status == other.status &&
        measurementRule == other.measurementRule &&
        unitCode == other.unitCode &&
        taxDisplay == other.taxDisplay &&
        validityDays == other.validityDays &&
        grades == other.grades &&
        complexities == other.complexities &&
        addOns == other.addOns &&
        assumptions == other.assumptions &&
        exclusions == other.exclusions;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, workType.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, measurementRule.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, taxDisplay.hashCode);
    _$hash = $jc(_$hash, validityDays.hashCode);
    _$hash = $jc(_$hash, grades.hashCode);
    _$hash = $jc(_$hash, complexities.hashCode);
    _$hash = $jc(_$hash, addOns.hashCode);
    _$hash = $jc(_$hash, assumptions.hashCode);
    _$hash = $jc(_$hash, exclusions.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EffectiveTemplateResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('version', version)
          ..add('workType', workType)
          ..add('name', name)
          ..add('status', status)
          ..add('measurementRule', measurementRule)
          ..add('unitCode', unitCode)
          ..add('taxDisplay', taxDisplay)
          ..add('validityDays', validityDays)
          ..add('grades', grades)
          ..add('complexities', complexities)
          ..add('addOns', addOns)
          ..add('assumptions', assumptions)
          ..add('exclusions', exclusions))
        .toString();
  }
}

class EffectiveTemplateResponseBuilder
    implements
        Builder<EffectiveTemplateResponse, EffectiveTemplateResponseBuilder> {
  _$EffectiveTemplateResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _workType;
  String? get workType => _$this._workType;
  set workType(String? workType) => _$this._workType = workType;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _measurementRule;
  String? get measurementRule => _$this._measurementRule;
  set measurementRule(String? measurementRule) =>
      _$this._measurementRule = measurementRule;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  String? _taxDisplay;
  String? get taxDisplay => _$this._taxDisplay;
  set taxDisplay(String? taxDisplay) => _$this._taxDisplay = taxDisplay;

  int? _validityDays;
  int? get validityDays => _$this._validityDays;
  set validityDays(int? validityDays) => _$this._validityDays = validityDays;

  ListBuilder<EffectiveOptionResponse>? _grades;
  ListBuilder<EffectiveOptionResponse> get grades =>
      _$this._grades ??= ListBuilder<EffectiveOptionResponse>();
  set grades(ListBuilder<EffectiveOptionResponse>? grades) =>
      _$this._grades = grades;

  ListBuilder<EffectiveOptionResponse>? _complexities;
  ListBuilder<EffectiveOptionResponse> get complexities =>
      _$this._complexities ??= ListBuilder<EffectiveOptionResponse>();
  set complexities(ListBuilder<EffectiveOptionResponse>? complexities) =>
      _$this._complexities = complexities;

  ListBuilder<EffectiveAddOnResponse>? _addOns;
  ListBuilder<EffectiveAddOnResponse> get addOns =>
      _$this._addOns ??= ListBuilder<EffectiveAddOnResponse>();
  set addOns(ListBuilder<EffectiveAddOnResponse>? addOns) =>
      _$this._addOns = addOns;

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

  EffectiveTemplateResponseBuilder() {
    EffectiveTemplateResponse._defaults(this);
  }

  EffectiveTemplateResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _version = $v.version;
      _workType = $v.workType;
      _name = $v.name;
      _status = $v.status;
      _measurementRule = $v.measurementRule;
      _unitCode = $v.unitCode;
      _taxDisplay = $v.taxDisplay;
      _validityDays = $v.validityDays;
      _grades = $v.grades?.toBuilder();
      _complexities = $v.complexities?.toBuilder();
      _addOns = $v.addOns?.toBuilder();
      _assumptions = $v.assumptions?.toBuilder();
      _exclusions = $v.exclusions?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EffectiveTemplateResponse other) {
    _$v = other as _$EffectiveTemplateResponse;
  }

  @override
  void update(void Function(EffectiveTemplateResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EffectiveTemplateResponse build() => _build();

  _$EffectiveTemplateResponse _build() {
    _$EffectiveTemplateResponse _$result;
    try {
      _$result = _$v ??
          _$EffectiveTemplateResponse._(
            id: id,
            code: code,
            version: version,
            workType: workType,
            name: name,
            status: status,
            measurementRule: measurementRule,
            unitCode: unitCode,
            taxDisplay: taxDisplay,
            validityDays: validityDays,
            grades: _grades?.build(),
            complexities: _complexities?.build(),
            addOns: _addOns?.build(),
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
            r'EffectiveTemplateResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
