// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_request_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceRequestResponse extends ServiceRequestResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final String? title;
  @override
  final String? description;
  @override
  final String? priority;
  @override
  final String? status;
  @override
  final bool? inWarranty;
  @override
  final WarrantyRefResponse? warranty;
  @override
  final ServiceProjectRefResponse? project;
  @override
  final Date? scheduledDate;
  @override
  final String? resolutionNote;
  @override
  final int? reopenCount;
  @override
  final ServicePersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? rowVersion;
  @override
  final BuiltList<ServiceRequestEventResponse>? events;

  factory _$ServiceRequestResponse(
          [void Function(ServiceRequestResponseBuilder)? updates]) =>
      (ServiceRequestResponseBuilder()..update(updates))._build();

  _$ServiceRequestResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.title,
      this.description,
      this.priority,
      this.status,
      this.inWarranty,
      this.warranty,
      this.project,
      this.scheduledDate,
      this.resolutionNote,
      this.reopenCount,
      this.createdBy,
      this.createdAtUtc,
      this.rowVersion,
      this.events})
      : super._();
  @override
  ServiceRequestResponse rebuild(
          void Function(ServiceRequestResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceRequestResponseBuilder toBuilder() =>
      ServiceRequestResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceRequestResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        title == other.title &&
        description == other.description &&
        priority == other.priority &&
        status == other.status &&
        inWarranty == other.inWarranty &&
        warranty == other.warranty &&
        project == other.project &&
        scheduledDate == other.scheduledDate &&
        resolutionNote == other.resolutionNote &&
        reopenCount == other.reopenCount &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        rowVersion == other.rowVersion &&
        events == other.events;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, priority.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, inWarranty.hashCode);
    _$hash = $jc(_$hash, warranty.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, scheduledDate.hashCode);
    _$hash = $jc(_$hash, resolutionNote.hashCode);
    _$hash = $jc(_$hash, reopenCount.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, events.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ServiceRequestResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('title', title)
          ..add('description', description)
          ..add('priority', priority)
          ..add('status', status)
          ..add('inWarranty', inWarranty)
          ..add('warranty', warranty)
          ..add('project', project)
          ..add('scheduledDate', scheduledDate)
          ..add('resolutionNote', resolutionNote)
          ..add('reopenCount', reopenCount)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('events', events))
        .toString();
  }
}

class ServiceRequestResponseBuilder
    implements Builder<ServiceRequestResponse, ServiceRequestResponseBuilder> {
  _$ServiceRequestResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  String? _priority;
  String? get priority => _$this._priority;
  set priority(String? priority) => _$this._priority = priority;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  bool? _inWarranty;
  bool? get inWarranty => _$this._inWarranty;
  set inWarranty(bool? inWarranty) => _$this._inWarranty = inWarranty;

  WarrantyRefResponseBuilder? _warranty;
  WarrantyRefResponseBuilder get warranty =>
      _$this._warranty ??= WarrantyRefResponseBuilder();
  set warranty(WarrantyRefResponseBuilder? warranty) =>
      _$this._warranty = warranty;

  ServiceProjectRefResponseBuilder? _project;
  ServiceProjectRefResponseBuilder get project =>
      _$this._project ??= ServiceProjectRefResponseBuilder();
  set project(ServiceProjectRefResponseBuilder? project) =>
      _$this._project = project;

  Date? _scheduledDate;
  Date? get scheduledDate => _$this._scheduledDate;
  set scheduledDate(Date? scheduledDate) =>
      _$this._scheduledDate = scheduledDate;

  String? _resolutionNote;
  String? get resolutionNote => _$this._resolutionNote;
  set resolutionNote(String? resolutionNote) =>
      _$this._resolutionNote = resolutionNote;

  int? _reopenCount;
  int? get reopenCount => _$this._reopenCount;
  set reopenCount(int? reopenCount) => _$this._reopenCount = reopenCount;

  ServicePersonResponseBuilder? _createdBy;
  ServicePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ServicePersonResponseBuilder();
  set createdBy(ServicePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<ServiceRequestEventResponse>? _events;
  ListBuilder<ServiceRequestEventResponse> get events =>
      _$this._events ??= ListBuilder<ServiceRequestEventResponse>();
  set events(ListBuilder<ServiceRequestEventResponse>? events) =>
      _$this._events = events;

  ServiceRequestResponseBuilder() {
    ServiceRequestResponse._defaults(this);
  }

  ServiceRequestResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _title = $v.title;
      _description = $v.description;
      _priority = $v.priority;
      _status = $v.status;
      _inWarranty = $v.inWarranty;
      _warranty = $v.warranty?.toBuilder();
      _project = $v.project?.toBuilder();
      _scheduledDate = $v.scheduledDate;
      _resolutionNote = $v.resolutionNote;
      _reopenCount = $v.reopenCount;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _rowVersion = $v.rowVersion;
      _events = $v.events?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceRequestResponse other) {
    _$v = other as _$ServiceRequestResponse;
  }

  @override
  void update(void Function(ServiceRequestResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceRequestResponse build() => _build();

  _$ServiceRequestResponse _build() {
    _$ServiceRequestResponse _$result;
    try {
      _$result = _$v ??
          _$ServiceRequestResponse._(
            id: id,
            branchId: branchId,
            number: number,
            title: title,
            description: description,
            priority: priority,
            status: status,
            inWarranty: inWarranty,
            warranty: _warranty?.build(),
            project: _project?.build(),
            scheduledDate: scheduledDate,
            resolutionNote: resolutionNote,
            reopenCount: reopenCount,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            rowVersion: rowVersion,
            events: _events?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'warranty';
        _warranty?.build();
        _$failedField = 'project';
        _project?.build();

        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'events';
        _events?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ServiceRequestResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
