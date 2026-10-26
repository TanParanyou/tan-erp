// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'installation_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InstallationListItemResponse extends InstallationListItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final String? projectCode;
  @override
  final String? projectName;
  @override
  final Date? scheduledStart;
  @override
  final Date? scheduledEnd;
  @override
  final int? checklistDone;
  @override
  final int? checklistTotal;
  @override
  final int? openDefects;
  @override
  final DateTime? createdAtUtc;

  factory _$InstallationListItemResponse(
          [void Function(InstallationListItemResponseBuilder)? updates]) =>
      (InstallationListItemResponseBuilder()..update(updates))._build();

  _$InstallationListItemResponse._(
      {this.id,
      this.number,
      this.status,
      this.projectCode,
      this.projectName,
      this.scheduledStart,
      this.scheduledEnd,
      this.checklistDone,
      this.checklistTotal,
      this.openDefects,
      this.createdAtUtc})
      : super._();
  @override
  InstallationListItemResponse rebuild(
          void Function(InstallationListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InstallationListItemResponseBuilder toBuilder() =>
      InstallationListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InstallationListItemResponse &&
        id == other.id &&
        number == other.number &&
        status == other.status &&
        projectCode == other.projectCode &&
        projectName == other.projectName &&
        scheduledStart == other.scheduledStart &&
        scheduledEnd == other.scheduledEnd &&
        checklistDone == other.checklistDone &&
        checklistTotal == other.checklistTotal &&
        openDefects == other.openDefects &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, projectName.hashCode);
    _$hash = $jc(_$hash, scheduledStart.hashCode);
    _$hash = $jc(_$hash, scheduledEnd.hashCode);
    _$hash = $jc(_$hash, checklistDone.hashCode);
    _$hash = $jc(_$hash, checklistTotal.hashCode);
    _$hash = $jc(_$hash, openDefects.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InstallationListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('status', status)
          ..add('projectCode', projectCode)
          ..add('projectName', projectName)
          ..add('scheduledStart', scheduledStart)
          ..add('scheduledEnd', scheduledEnd)
          ..add('checklistDone', checklistDone)
          ..add('checklistTotal', checklistTotal)
          ..add('openDefects', openDefects)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class InstallationListItemResponseBuilder
    implements
        Builder<InstallationListItemResponse,
            InstallationListItemResponseBuilder> {
  _$InstallationListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  String? _projectName;
  String? get projectName => _$this._projectName;
  set projectName(String? projectName) => _$this._projectName = projectName;

  Date? _scheduledStart;
  Date? get scheduledStart => _$this._scheduledStart;
  set scheduledStart(Date? scheduledStart) =>
      _$this._scheduledStart = scheduledStart;

  Date? _scheduledEnd;
  Date? get scheduledEnd => _$this._scheduledEnd;
  set scheduledEnd(Date? scheduledEnd) => _$this._scheduledEnd = scheduledEnd;

  int? _checklistDone;
  int? get checklistDone => _$this._checklistDone;
  set checklistDone(int? checklistDone) =>
      _$this._checklistDone = checklistDone;

  int? _checklistTotal;
  int? get checklistTotal => _$this._checklistTotal;
  set checklistTotal(int? checklistTotal) =>
      _$this._checklistTotal = checklistTotal;

  int? _openDefects;
  int? get openDefects => _$this._openDefects;
  set openDefects(int? openDefects) => _$this._openDefects = openDefects;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  InstallationListItemResponseBuilder() {
    InstallationListItemResponse._defaults(this);
  }

  InstallationListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _status = $v.status;
      _projectCode = $v.projectCode;
      _projectName = $v.projectName;
      _scheduledStart = $v.scheduledStart;
      _scheduledEnd = $v.scheduledEnd;
      _checklistDone = $v.checklistDone;
      _checklistTotal = $v.checklistTotal;
      _openDefects = $v.openDefects;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InstallationListItemResponse other) {
    _$v = other as _$InstallationListItemResponse;
  }

  @override
  void update(void Function(InstallationListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InstallationListItemResponse build() => _build();

  _$InstallationListItemResponse _build() {
    final _$result = _$v ??
        _$InstallationListItemResponse._(
          id: id,
          number: number,
          status: status,
          projectCode: projectCode,
          projectName: projectName,
          scheduledStart: scheduledStart,
          scheduledEnd: scheduledEnd,
          checklistDone: checklistDone,
          checklistTotal: checklistTotal,
          openDefects: openDefects,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
