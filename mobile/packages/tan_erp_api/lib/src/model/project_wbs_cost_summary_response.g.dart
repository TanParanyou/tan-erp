// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_wbs_cost_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectWbsCostSummaryResponse extends ProjectWbsCostSummaryResponse {
  @override
  final String? wbsNodeId;
  @override
  final String? wbsCode;
  @override
  final String? wbsName;
  @override
  final int? level;
  @override
  final double? baselineBudget;
  @override
  final double? committedAmount;
  @override
  final double? actualAmount;
  @override
  final double? remainingBudget;
  @override
  final bool? isOverrun;

  factory _$ProjectWbsCostSummaryResponse(
          [void Function(ProjectWbsCostSummaryResponseBuilder)? updates]) =>
      (ProjectWbsCostSummaryResponseBuilder()..update(updates))._build();

  _$ProjectWbsCostSummaryResponse._(
      {this.wbsNodeId,
      this.wbsCode,
      this.wbsName,
      this.level,
      this.baselineBudget,
      this.committedAmount,
      this.actualAmount,
      this.remainingBudget,
      this.isOverrun})
      : super._();
  @override
  ProjectWbsCostSummaryResponse rebuild(
          void Function(ProjectWbsCostSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectWbsCostSummaryResponseBuilder toBuilder() =>
      ProjectWbsCostSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectWbsCostSummaryResponse &&
        wbsNodeId == other.wbsNodeId &&
        wbsCode == other.wbsCode &&
        wbsName == other.wbsName &&
        level == other.level &&
        baselineBudget == other.baselineBudget &&
        committedAmount == other.committedAmount &&
        actualAmount == other.actualAmount &&
        remainingBudget == other.remainingBudget &&
        isOverrun == other.isOverrun;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, wbsNodeId.hashCode);
    _$hash = $jc(_$hash, wbsCode.hashCode);
    _$hash = $jc(_$hash, wbsName.hashCode);
    _$hash = $jc(_$hash, level.hashCode);
    _$hash = $jc(_$hash, baselineBudget.hashCode);
    _$hash = $jc(_$hash, committedAmount.hashCode);
    _$hash = $jc(_$hash, actualAmount.hashCode);
    _$hash = $jc(_$hash, remainingBudget.hashCode);
    _$hash = $jc(_$hash, isOverrun.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectWbsCostSummaryResponse')
          ..add('wbsNodeId', wbsNodeId)
          ..add('wbsCode', wbsCode)
          ..add('wbsName', wbsName)
          ..add('level', level)
          ..add('baselineBudget', baselineBudget)
          ..add('committedAmount', committedAmount)
          ..add('actualAmount', actualAmount)
          ..add('remainingBudget', remainingBudget)
          ..add('isOverrun', isOverrun))
        .toString();
  }
}

class ProjectWbsCostSummaryResponseBuilder
    implements
        Builder<ProjectWbsCostSummaryResponse,
            ProjectWbsCostSummaryResponseBuilder> {
  _$ProjectWbsCostSummaryResponse? _$v;

  String? _wbsNodeId;
  String? get wbsNodeId => _$this._wbsNodeId;
  set wbsNodeId(String? wbsNodeId) => _$this._wbsNodeId = wbsNodeId;

  String? _wbsCode;
  String? get wbsCode => _$this._wbsCode;
  set wbsCode(String? wbsCode) => _$this._wbsCode = wbsCode;

  String? _wbsName;
  String? get wbsName => _$this._wbsName;
  set wbsName(String? wbsName) => _$this._wbsName = wbsName;

  int? _level;
  int? get level => _$this._level;
  set level(int? level) => _$this._level = level;

  double? _baselineBudget;
  double? get baselineBudget => _$this._baselineBudget;
  set baselineBudget(double? baselineBudget) =>
      _$this._baselineBudget = baselineBudget;

  double? _committedAmount;
  double? get committedAmount => _$this._committedAmount;
  set committedAmount(double? committedAmount) =>
      _$this._committedAmount = committedAmount;

  double? _actualAmount;
  double? get actualAmount => _$this._actualAmount;
  set actualAmount(double? actualAmount) => _$this._actualAmount = actualAmount;

  double? _remainingBudget;
  double? get remainingBudget => _$this._remainingBudget;
  set remainingBudget(double? remainingBudget) =>
      _$this._remainingBudget = remainingBudget;

  bool? _isOverrun;
  bool? get isOverrun => _$this._isOverrun;
  set isOverrun(bool? isOverrun) => _$this._isOverrun = isOverrun;

  ProjectWbsCostSummaryResponseBuilder() {
    ProjectWbsCostSummaryResponse._defaults(this);
  }

  ProjectWbsCostSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _wbsNodeId = $v.wbsNodeId;
      _wbsCode = $v.wbsCode;
      _wbsName = $v.wbsName;
      _level = $v.level;
      _baselineBudget = $v.baselineBudget;
      _committedAmount = $v.committedAmount;
      _actualAmount = $v.actualAmount;
      _remainingBudget = $v.remainingBudget;
      _isOverrun = $v.isOverrun;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectWbsCostSummaryResponse other) {
    _$v = other as _$ProjectWbsCostSummaryResponse;
  }

  @override
  void update(void Function(ProjectWbsCostSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectWbsCostSummaryResponse build() => _build();

  _$ProjectWbsCostSummaryResponse _build() {
    final _$result = _$v ??
        _$ProjectWbsCostSummaryResponse._(
          wbsNodeId: wbsNodeId,
          wbsCode: wbsCode,
          wbsName: wbsName,
          level: level,
          baselineBudget: baselineBudget,
          committedAmount: committedAmount,
          actualAmount: actualAmount,
          remainingBudget: remainingBudget,
          isOverrun: isOverrun,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
