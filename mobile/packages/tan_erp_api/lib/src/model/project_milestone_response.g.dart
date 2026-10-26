// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_milestone_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectMilestoneResponse extends ProjectMilestoneResponse {
  @override
  final String? id;
  @override
  final String? name;
  @override
  final Date? plannedDate;
  @override
  final int? weight;
  @override
  final int? sortOrder;
  @override
  final DateTime? completedAtUtc;
  @override
  final ProjectPersonResponse? completedBy;
  @override
  final String? rowVersion;

  factory _$ProjectMilestoneResponse(
          [void Function(ProjectMilestoneResponseBuilder)? updates]) =>
      (ProjectMilestoneResponseBuilder()..update(updates))._build();

  _$ProjectMilestoneResponse._(
      {this.id,
      this.name,
      this.plannedDate,
      this.weight,
      this.sortOrder,
      this.completedAtUtc,
      this.completedBy,
      this.rowVersion})
      : super._();
  @override
  ProjectMilestoneResponse rebuild(
          void Function(ProjectMilestoneResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectMilestoneResponseBuilder toBuilder() =>
      ProjectMilestoneResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectMilestoneResponse &&
        id == other.id &&
        name == other.name &&
        plannedDate == other.plannedDate &&
        weight == other.weight &&
        sortOrder == other.sortOrder &&
        completedAtUtc == other.completedAtUtc &&
        completedBy == other.completedBy &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, plannedDate.hashCode);
    _$hash = $jc(_$hash, weight.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, completedAtUtc.hashCode);
    _$hash = $jc(_$hash, completedBy.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectMilestoneResponse')
          ..add('id', id)
          ..add('name', name)
          ..add('plannedDate', plannedDate)
          ..add('weight', weight)
          ..add('sortOrder', sortOrder)
          ..add('completedAtUtc', completedAtUtc)
          ..add('completedBy', completedBy)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class ProjectMilestoneResponseBuilder
    implements
        Builder<ProjectMilestoneResponse, ProjectMilestoneResponseBuilder> {
  _$ProjectMilestoneResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  Date? _plannedDate;
  Date? get plannedDate => _$this._plannedDate;
  set plannedDate(Date? plannedDate) => _$this._plannedDate = plannedDate;

  int? _weight;
  int? get weight => _$this._weight;
  set weight(int? weight) => _$this._weight = weight;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  DateTime? _completedAtUtc;
  DateTime? get completedAtUtc => _$this._completedAtUtc;
  set completedAtUtc(DateTime? completedAtUtc) =>
      _$this._completedAtUtc = completedAtUtc;

  ProjectPersonResponseBuilder? _completedBy;
  ProjectPersonResponseBuilder get completedBy =>
      _$this._completedBy ??= ProjectPersonResponseBuilder();
  set completedBy(ProjectPersonResponseBuilder? completedBy) =>
      _$this._completedBy = completedBy;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ProjectMilestoneResponseBuilder() {
    ProjectMilestoneResponse._defaults(this);
  }

  ProjectMilestoneResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _plannedDate = $v.plannedDate;
      _weight = $v.weight;
      _sortOrder = $v.sortOrder;
      _completedAtUtc = $v.completedAtUtc;
      _completedBy = $v.completedBy?.toBuilder();
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectMilestoneResponse other) {
    _$v = other as _$ProjectMilestoneResponse;
  }

  @override
  void update(void Function(ProjectMilestoneResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectMilestoneResponse build() => _build();

  _$ProjectMilestoneResponse _build() {
    _$ProjectMilestoneResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectMilestoneResponse._(
            id: id,
            name: name,
            plannedDate: plannedDate,
            weight: weight,
            sortOrder: sortOrder,
            completedAtUtc: completedAtUtc,
            completedBy: _completedBy?.build(),
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'completedBy';
        _completedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectMilestoneResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
