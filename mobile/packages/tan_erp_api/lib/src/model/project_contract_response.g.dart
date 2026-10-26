// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_contract_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectContractResponse extends ProjectContractResponse {
  @override
  final double? baselineAmount;
  @override
  final double? approvedDelta;
  @override
  final double? currentAmount;

  factory _$ProjectContractResponse(
          [void Function(ProjectContractResponseBuilder)? updates]) =>
      (ProjectContractResponseBuilder()..update(updates))._build();

  _$ProjectContractResponse._(
      {this.baselineAmount, this.approvedDelta, this.currentAmount})
      : super._();
  @override
  ProjectContractResponse rebuild(
          void Function(ProjectContractResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectContractResponseBuilder toBuilder() =>
      ProjectContractResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectContractResponse &&
        baselineAmount == other.baselineAmount &&
        approvedDelta == other.approvedDelta &&
        currentAmount == other.currentAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, baselineAmount.hashCode);
    _$hash = $jc(_$hash, approvedDelta.hashCode);
    _$hash = $jc(_$hash, currentAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectContractResponse')
          ..add('baselineAmount', baselineAmount)
          ..add('approvedDelta', approvedDelta)
          ..add('currentAmount', currentAmount))
        .toString();
  }
}

class ProjectContractResponseBuilder
    implements
        Builder<ProjectContractResponse, ProjectContractResponseBuilder> {
  _$ProjectContractResponse? _$v;

  double? _baselineAmount;
  double? get baselineAmount => _$this._baselineAmount;
  set baselineAmount(double? baselineAmount) =>
      _$this._baselineAmount = baselineAmount;

  double? _approvedDelta;
  double? get approvedDelta => _$this._approvedDelta;
  set approvedDelta(double? approvedDelta) =>
      _$this._approvedDelta = approvedDelta;

  double? _currentAmount;
  double? get currentAmount => _$this._currentAmount;
  set currentAmount(double? currentAmount) =>
      _$this._currentAmount = currentAmount;

  ProjectContractResponseBuilder() {
    ProjectContractResponse._defaults(this);
  }

  ProjectContractResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _baselineAmount = $v.baselineAmount;
      _approvedDelta = $v.approvedDelta;
      _currentAmount = $v.currentAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectContractResponse other) {
    _$v = other as _$ProjectContractResponse;
  }

  @override
  void update(void Function(ProjectContractResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectContractResponse build() => _build();

  _$ProjectContractResponse _build() {
    final _$result = _$v ??
        _$ProjectContractResponse._(
          baselineAmount: baselineAmount,
          approvedDelta: approvedDelta,
          currentAmount: currentAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
