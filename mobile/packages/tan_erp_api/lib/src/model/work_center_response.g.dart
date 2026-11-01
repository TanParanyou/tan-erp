// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_center_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkCenterResponse extends WorkCenterResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? dailyCapacityHours;
  @override
  final double? hourlyRate;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;

  factory _$WorkCenterResponse(
          [void Function(WorkCenterResponseBuilder)? updates]) =>
      (WorkCenterResponseBuilder()..update(updates))._build();

  _$WorkCenterResponse._(
      {this.id,
      this.branchId,
      this.code,
      this.name,
      this.dailyCapacityHours,
      this.hourlyRate,
      this.status,
      this.rowVersion,
      this.createdAtUtc,
      this.updatedAtUtc})
      : super._();
  @override
  WorkCenterResponse rebuild(
          void Function(WorkCenterResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkCenterResponseBuilder toBuilder() =>
      WorkCenterResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkCenterResponse &&
        id == other.id &&
        branchId == other.branchId &&
        code == other.code &&
        name == other.name &&
        dailyCapacityHours == other.dailyCapacityHours &&
        hourlyRate == other.hourlyRate &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, dailyCapacityHours.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkCenterResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('code', code)
          ..add('name', name)
          ..add('dailyCapacityHours', dailyCapacityHours)
          ..add('hourlyRate', hourlyRate)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class WorkCenterResponseBuilder
    implements Builder<WorkCenterResponse, WorkCenterResponseBuilder> {
  _$WorkCenterResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  double? _dailyCapacityHours;
  double? get dailyCapacityHours => _$this._dailyCapacityHours;
  set dailyCapacityHours(double? dailyCapacityHours) =>
      _$this._dailyCapacityHours = dailyCapacityHours;

  double? _hourlyRate;
  double? get hourlyRate => _$this._hourlyRate;
  set hourlyRate(double? hourlyRate) => _$this._hourlyRate = hourlyRate;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  WorkCenterResponseBuilder() {
    WorkCenterResponse._defaults(this);
  }

  WorkCenterResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _code = $v.code;
      _name = $v.name;
      _dailyCapacityHours = $v.dailyCapacityHours;
      _hourlyRate = $v.hourlyRate;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkCenterResponse other) {
    _$v = other as _$WorkCenterResponse;
  }

  @override
  void update(void Function(WorkCenterResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkCenterResponse build() => _build();

  _$WorkCenterResponse _build() {
    final _$result = _$v ??
        _$WorkCenterResponse._(
          id: id,
          branchId: branchId,
          code: code,
          name: name,
          dailyCapacityHours: dailyCapacityHours,
          hourlyRate: hourlyRate,
          status: status,
          rowVersion: rowVersion,
          createdAtUtc: createdAtUtc,
          updatedAtUtc: updatedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
