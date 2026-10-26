// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_change_order_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectChangeOrderResponse extends ProjectChangeOrderResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? title;
  @override
  final String? reason;
  @override
  final double? budgetDelta;
  @override
  final double? contractDelta;
  @override
  final String? status;
  @override
  final ProjectPersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? submittedAtUtc;
  @override
  final ProjectPersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final String? decisionNote;
  @override
  final String? rowVersion;

  factory _$ProjectChangeOrderResponse(
          [void Function(ProjectChangeOrderResponseBuilder)? updates]) =>
      (ProjectChangeOrderResponseBuilder()..update(updates))._build();

  _$ProjectChangeOrderResponse._(
      {this.id,
      this.number,
      this.title,
      this.reason,
      this.budgetDelta,
      this.contractDelta,
      this.status,
      this.createdBy,
      this.createdAtUtc,
      this.submittedAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.decisionNote,
      this.rowVersion})
      : super._();
  @override
  ProjectChangeOrderResponse rebuild(
          void Function(ProjectChangeOrderResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectChangeOrderResponseBuilder toBuilder() =>
      ProjectChangeOrderResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectChangeOrderResponse &&
        id == other.id &&
        number == other.number &&
        title == other.title &&
        reason == other.reason &&
        budgetDelta == other.budgetDelta &&
        contractDelta == other.contractDelta &&
        status == other.status &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        submittedAtUtc == other.submittedAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        decisionNote == other.decisionNote &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, budgetDelta.hashCode);
    _$hash = $jc(_$hash, contractDelta.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, submittedAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectChangeOrderResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('title', title)
          ..add('reason', reason)
          ..add('budgetDelta', budgetDelta)
          ..add('contractDelta', contractDelta)
          ..add('status', status)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('submittedAtUtc', submittedAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('decisionNote', decisionNote)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class ProjectChangeOrderResponseBuilder
    implements
        Builder<ProjectChangeOrderResponse, ProjectChangeOrderResponseBuilder> {
  _$ProjectChangeOrderResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  double? _budgetDelta;
  double? get budgetDelta => _$this._budgetDelta;
  set budgetDelta(double? budgetDelta) => _$this._budgetDelta = budgetDelta;

  double? _contractDelta;
  double? get contractDelta => _$this._contractDelta;
  set contractDelta(double? contractDelta) =>
      _$this._contractDelta = contractDelta;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  ProjectPersonResponseBuilder? _createdBy;
  ProjectPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ProjectPersonResponseBuilder();
  set createdBy(ProjectPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _submittedAtUtc;
  DateTime? get submittedAtUtc => _$this._submittedAtUtc;
  set submittedAtUtc(DateTime? submittedAtUtc) =>
      _$this._submittedAtUtc = submittedAtUtc;

  ProjectPersonResponseBuilder? _decidedBy;
  ProjectPersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= ProjectPersonResponseBuilder();
  set decidedBy(ProjectPersonResponseBuilder? decidedBy) =>
      _$this._decidedBy = decidedBy;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  String? _decisionNote;
  String? get decisionNote => _$this._decisionNote;
  set decisionNote(String? decisionNote) => _$this._decisionNote = decisionNote;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ProjectChangeOrderResponseBuilder() {
    ProjectChangeOrderResponse._defaults(this);
  }

  ProjectChangeOrderResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _title = $v.title;
      _reason = $v.reason;
      _budgetDelta = $v.budgetDelta;
      _contractDelta = $v.contractDelta;
      _status = $v.status;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _submittedAtUtc = $v.submittedAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _decisionNote = $v.decisionNote;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectChangeOrderResponse other) {
    _$v = other as _$ProjectChangeOrderResponse;
  }

  @override
  void update(void Function(ProjectChangeOrderResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectChangeOrderResponse build() => _build();

  _$ProjectChangeOrderResponse _build() {
    _$ProjectChangeOrderResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectChangeOrderResponse._(
            id: id,
            number: number,
            title: title,
            reason: reason,
            budgetDelta: budgetDelta,
            contractDelta: contractDelta,
            status: status,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            submittedAtUtc: submittedAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            decisionNote: decisionNote,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectChangeOrderResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
