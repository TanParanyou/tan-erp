// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_project_milestone_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AddProjectMilestoneRequest extends AddProjectMilestoneRequest {
  @override
  final String? name;
  @override
  final Date? plannedDate;
  @override
  final int? weight;

  factory _$AddProjectMilestoneRequest(
          [void Function(AddProjectMilestoneRequestBuilder)? updates]) =>
      (AddProjectMilestoneRequestBuilder()..update(updates))._build();

  _$AddProjectMilestoneRequest._({this.name, this.plannedDate, this.weight})
      : super._();
  @override
  AddProjectMilestoneRequest rebuild(
          void Function(AddProjectMilestoneRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AddProjectMilestoneRequestBuilder toBuilder() =>
      AddProjectMilestoneRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AddProjectMilestoneRequest &&
        name == other.name &&
        plannedDate == other.plannedDate &&
        weight == other.weight;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, plannedDate.hashCode);
    _$hash = $jc(_$hash, weight.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AddProjectMilestoneRequest')
          ..add('name', name)
          ..add('plannedDate', plannedDate)
          ..add('weight', weight))
        .toString();
  }
}

class AddProjectMilestoneRequestBuilder
    implements
        Builder<AddProjectMilestoneRequest, AddProjectMilestoneRequestBuilder> {
  _$AddProjectMilestoneRequest? _$v;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  Date? _plannedDate;
  Date? get plannedDate => _$this._plannedDate;
  set plannedDate(Date? plannedDate) => _$this._plannedDate = plannedDate;

  int? _weight;
  int? get weight => _$this._weight;
  set weight(int? weight) => _$this._weight = weight;

  AddProjectMilestoneRequestBuilder() {
    AddProjectMilestoneRequest._defaults(this);
  }

  AddProjectMilestoneRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _plannedDate = $v.plannedDate;
      _weight = $v.weight;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AddProjectMilestoneRequest other) {
    _$v = other as _$AddProjectMilestoneRequest;
  }

  @override
  void update(void Function(AddProjectMilestoneRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AddProjectMilestoneRequest build() => _build();

  _$AddProjectMilestoneRequest _build() {
    final _$result = _$v ??
        _$AddProjectMilestoneRequest._(
          name: name,
          plannedDate: plannedDate,
          weight: weight,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
