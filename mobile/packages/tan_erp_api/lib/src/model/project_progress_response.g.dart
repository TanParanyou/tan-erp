// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_progress_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectProgressResponse extends ProjectProgressResponse {
  @override
  final int? totalMilestones;
  @override
  final int? completedMilestones;
  @override
  final int? totalWeight;
  @override
  final int? completedWeight;
  @override
  final double? percent;

  factory _$ProjectProgressResponse(
          [void Function(ProjectProgressResponseBuilder)? updates]) =>
      (ProjectProgressResponseBuilder()..update(updates))._build();

  _$ProjectProgressResponse._(
      {this.totalMilestones,
      this.completedMilestones,
      this.totalWeight,
      this.completedWeight,
      this.percent})
      : super._();
  @override
  ProjectProgressResponse rebuild(
          void Function(ProjectProgressResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectProgressResponseBuilder toBuilder() =>
      ProjectProgressResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectProgressResponse &&
        totalMilestones == other.totalMilestones &&
        completedMilestones == other.completedMilestones &&
        totalWeight == other.totalWeight &&
        completedWeight == other.completedWeight &&
        percent == other.percent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, totalMilestones.hashCode);
    _$hash = $jc(_$hash, completedMilestones.hashCode);
    _$hash = $jc(_$hash, totalWeight.hashCode);
    _$hash = $jc(_$hash, completedWeight.hashCode);
    _$hash = $jc(_$hash, percent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectProgressResponse')
          ..add('totalMilestones', totalMilestones)
          ..add('completedMilestones', completedMilestones)
          ..add('totalWeight', totalWeight)
          ..add('completedWeight', completedWeight)
          ..add('percent', percent))
        .toString();
  }
}

class ProjectProgressResponseBuilder
    implements
        Builder<ProjectProgressResponse, ProjectProgressResponseBuilder> {
  _$ProjectProgressResponse? _$v;

  int? _totalMilestones;
  int? get totalMilestones => _$this._totalMilestones;
  set totalMilestones(int? totalMilestones) =>
      _$this._totalMilestones = totalMilestones;

  int? _completedMilestones;
  int? get completedMilestones => _$this._completedMilestones;
  set completedMilestones(int? completedMilestones) =>
      _$this._completedMilestones = completedMilestones;

  int? _totalWeight;
  int? get totalWeight => _$this._totalWeight;
  set totalWeight(int? totalWeight) => _$this._totalWeight = totalWeight;

  int? _completedWeight;
  int? get completedWeight => _$this._completedWeight;
  set completedWeight(int? completedWeight) =>
      _$this._completedWeight = completedWeight;

  double? _percent;
  double? get percent => _$this._percent;
  set percent(double? percent) => _$this._percent = percent;

  ProjectProgressResponseBuilder() {
    ProjectProgressResponse._defaults(this);
  }

  ProjectProgressResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _totalMilestones = $v.totalMilestones;
      _completedMilestones = $v.completedMilestones;
      _totalWeight = $v.totalWeight;
      _completedWeight = $v.completedWeight;
      _percent = $v.percent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectProgressResponse other) {
    _$v = other as _$ProjectProgressResponse;
  }

  @override
  void update(void Function(ProjectProgressResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectProgressResponse build() => _build();

  _$ProjectProgressResponse _build() {
    final _$result = _$v ??
        _$ProjectProgressResponse._(
          totalMilestones: totalMilestones,
          completedMilestones: completedMilestones,
          totalWeight: totalWeight,
          completedWeight: completedWeight,
          percent: percent,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
