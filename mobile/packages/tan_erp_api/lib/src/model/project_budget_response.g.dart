// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetResponse extends ProjectBudgetResponse {
  @override
  final double? baselineTotal;
  @override
  final String? baselineHash;
  @override
  final bool? isFrozen;
  @override
  final DateTime? frozenAtUtc;
  @override
  final double? approvedBudgetDelta;
  @override
  final double? currentTotal;
  @override
  final double? committedAmount;
  @override
  final double? availableBudget;
  @override
  final BuiltList<ProjectBudgetLineResponse>? lines;

  factory _$ProjectBudgetResponse(
          [void Function(ProjectBudgetResponseBuilder)? updates]) =>
      (ProjectBudgetResponseBuilder()..update(updates))._build();

  _$ProjectBudgetResponse._(
      {this.baselineTotal,
      this.baselineHash,
      this.isFrozen,
      this.frozenAtUtc,
      this.approvedBudgetDelta,
      this.currentTotal,
      this.committedAmount,
      this.availableBudget,
      this.lines})
      : super._();
  @override
  ProjectBudgetResponse rebuild(
          void Function(ProjectBudgetResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetResponseBuilder toBuilder() =>
      ProjectBudgetResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetResponse &&
        baselineTotal == other.baselineTotal &&
        baselineHash == other.baselineHash &&
        isFrozen == other.isFrozen &&
        frozenAtUtc == other.frozenAtUtc &&
        approvedBudgetDelta == other.approvedBudgetDelta &&
        currentTotal == other.currentTotal &&
        committedAmount == other.committedAmount &&
        availableBudget == other.availableBudget &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, baselineTotal.hashCode);
    _$hash = $jc(_$hash, baselineHash.hashCode);
    _$hash = $jc(_$hash, isFrozen.hashCode);
    _$hash = $jc(_$hash, frozenAtUtc.hashCode);
    _$hash = $jc(_$hash, approvedBudgetDelta.hashCode);
    _$hash = $jc(_$hash, currentTotal.hashCode);
    _$hash = $jc(_$hash, committedAmount.hashCode);
    _$hash = $jc(_$hash, availableBudget.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBudgetResponse')
          ..add('baselineTotal', baselineTotal)
          ..add('baselineHash', baselineHash)
          ..add('isFrozen', isFrozen)
          ..add('frozenAtUtc', frozenAtUtc)
          ..add('approvedBudgetDelta', approvedBudgetDelta)
          ..add('currentTotal', currentTotal)
          ..add('committedAmount', committedAmount)
          ..add('availableBudget', availableBudget)
          ..add('lines', lines))
        .toString();
  }
}

class ProjectBudgetResponseBuilder
    implements Builder<ProjectBudgetResponse, ProjectBudgetResponseBuilder> {
  _$ProjectBudgetResponse? _$v;

  double? _baselineTotal;
  double? get baselineTotal => _$this._baselineTotal;
  set baselineTotal(double? baselineTotal) =>
      _$this._baselineTotal = baselineTotal;

  String? _baselineHash;
  String? get baselineHash => _$this._baselineHash;
  set baselineHash(String? baselineHash) => _$this._baselineHash = baselineHash;

  bool? _isFrozen;
  bool? get isFrozen => _$this._isFrozen;
  set isFrozen(bool? isFrozen) => _$this._isFrozen = isFrozen;

  DateTime? _frozenAtUtc;
  DateTime? get frozenAtUtc => _$this._frozenAtUtc;
  set frozenAtUtc(DateTime? frozenAtUtc) => _$this._frozenAtUtc = frozenAtUtc;

  double? _approvedBudgetDelta;
  double? get approvedBudgetDelta => _$this._approvedBudgetDelta;
  set approvedBudgetDelta(double? approvedBudgetDelta) =>
      _$this._approvedBudgetDelta = approvedBudgetDelta;

  double? _currentTotal;
  double? get currentTotal => _$this._currentTotal;
  set currentTotal(double? currentTotal) => _$this._currentTotal = currentTotal;

  double? _committedAmount;
  double? get committedAmount => _$this._committedAmount;
  set committedAmount(double? committedAmount) =>
      _$this._committedAmount = committedAmount;

  double? _availableBudget;
  double? get availableBudget => _$this._availableBudget;
  set availableBudget(double? availableBudget) =>
      _$this._availableBudget = availableBudget;

  ListBuilder<ProjectBudgetLineResponse>? _lines;
  ListBuilder<ProjectBudgetLineResponse> get lines =>
      _$this._lines ??= ListBuilder<ProjectBudgetLineResponse>();
  set lines(ListBuilder<ProjectBudgetLineResponse>? lines) =>
      _$this._lines = lines;

  ProjectBudgetResponseBuilder() {
    ProjectBudgetResponse._defaults(this);
  }

  ProjectBudgetResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _baselineTotal = $v.baselineTotal;
      _baselineHash = $v.baselineHash;
      _isFrozen = $v.isFrozen;
      _frozenAtUtc = $v.frozenAtUtc;
      _approvedBudgetDelta = $v.approvedBudgetDelta;
      _currentTotal = $v.currentTotal;
      _committedAmount = $v.committedAmount;
      _availableBudget = $v.availableBudget;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetResponse other) {
    _$v = other as _$ProjectBudgetResponse;
  }

  @override
  void update(void Function(ProjectBudgetResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetResponse build() => _build();

  _$ProjectBudgetResponse _build() {
    _$ProjectBudgetResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectBudgetResponse._(
            baselineTotal: baselineTotal,
            baselineHash: baselineHash,
            isFrozen: isFrozen,
            frozenAtUtc: frozenAtUtc,
            approvedBudgetDelta: approvedBudgetDelta,
            currentTotal: currentTotal,
            committedAmount: committedAmount,
            availableBudget: availableBudget,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectBudgetResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
