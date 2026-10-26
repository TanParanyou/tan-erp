// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_project_milestone_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateProjectMilestoneRequest extends UpdateProjectMilestoneRequest {
  @override
  final String? name;
  @override
  final Date? plannedDate;
  @override
  final int? weight;
  @override
  final String? expectedVersion;

  factory _$UpdateProjectMilestoneRequest(
          [void Function(UpdateProjectMilestoneRequestBuilder)? updates]) =>
      (UpdateProjectMilestoneRequestBuilder()..update(updates))._build();

  _$UpdateProjectMilestoneRequest._(
      {this.name, this.plannedDate, this.weight, this.expectedVersion})
      : super._();
  @override
  UpdateProjectMilestoneRequest rebuild(
          void Function(UpdateProjectMilestoneRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateProjectMilestoneRequestBuilder toBuilder() =>
      UpdateProjectMilestoneRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateProjectMilestoneRequest &&
        name == other.name &&
        plannedDate == other.plannedDate &&
        weight == other.weight &&
        expectedVersion == other.expectedVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, plannedDate.hashCode);
    _$hash = $jc(_$hash, weight.hashCode);
    _$hash = $jc(_$hash, expectedVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateProjectMilestoneRequest')
          ..add('name', name)
          ..add('plannedDate', plannedDate)
          ..add('weight', weight)
          ..add('expectedVersion', expectedVersion))
        .toString();
  }
}

class UpdateProjectMilestoneRequestBuilder
    implements
        Builder<UpdateProjectMilestoneRequest,
            UpdateProjectMilestoneRequestBuilder> {
  _$UpdateProjectMilestoneRequest? _$v;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  Date? _plannedDate;
  Date? get plannedDate => _$this._plannedDate;
  set plannedDate(Date? plannedDate) => _$this._plannedDate = plannedDate;

  int? _weight;
  int? get weight => _$this._weight;
  set weight(int? weight) => _$this._weight = weight;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  UpdateProjectMilestoneRequestBuilder() {
    UpdateProjectMilestoneRequest._defaults(this);
  }

  UpdateProjectMilestoneRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _plannedDate = $v.plannedDate;
      _weight = $v.weight;
      _expectedVersion = $v.expectedVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateProjectMilestoneRequest other) {
    _$v = other as _$UpdateProjectMilestoneRequest;
  }

  @override
  void update(void Function(UpdateProjectMilestoneRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateProjectMilestoneRequest build() => _build();

  _$UpdateProjectMilestoneRequest _build() {
    final _$result = _$v ??
        _$UpdateProjectMilestoneRequest._(
          name: name,
          plannedDate: plannedDate,
          weight: weight,
          expectedVersion: expectedVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
