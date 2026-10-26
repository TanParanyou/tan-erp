// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'defect_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DefectResponse extends DefectResponse {
  @override
  final String? id;
  @override
  final int? no;
  @override
  final String? description;
  @override
  final String? severity;
  @override
  final String? status;
  @override
  final ServicePersonResponse? reportedBy;
  @override
  final DateTime? reportedAtUtc;
  @override
  final ServicePersonResponse? resolvedBy;
  @override
  final DateTime? resolvedAtUtc;
  @override
  final String? resolutionNote;
  @override
  final ServicePersonResponse? verifiedBy;
  @override
  final DateTime? verifiedAtUtc;
  @override
  final int? reopenCount;
  @override
  final String? lastReopenReason;

  factory _$DefectResponse([void Function(DefectResponseBuilder)? updates]) =>
      (DefectResponseBuilder()..update(updates))._build();

  _$DefectResponse._(
      {this.id,
      this.no,
      this.description,
      this.severity,
      this.status,
      this.reportedBy,
      this.reportedAtUtc,
      this.resolvedBy,
      this.resolvedAtUtc,
      this.resolutionNote,
      this.verifiedBy,
      this.verifiedAtUtc,
      this.reopenCount,
      this.lastReopenReason})
      : super._();
  @override
  DefectResponse rebuild(void Function(DefectResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DefectResponseBuilder toBuilder() => DefectResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DefectResponse &&
        id == other.id &&
        no == other.no &&
        description == other.description &&
        severity == other.severity &&
        status == other.status &&
        reportedBy == other.reportedBy &&
        reportedAtUtc == other.reportedAtUtc &&
        resolvedBy == other.resolvedBy &&
        resolvedAtUtc == other.resolvedAtUtc &&
        resolutionNote == other.resolutionNote &&
        verifiedBy == other.verifiedBy &&
        verifiedAtUtc == other.verifiedAtUtc &&
        reopenCount == other.reopenCount &&
        lastReopenReason == other.lastReopenReason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, no.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, severity.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, reportedBy.hashCode);
    _$hash = $jc(_$hash, reportedAtUtc.hashCode);
    _$hash = $jc(_$hash, resolvedBy.hashCode);
    _$hash = $jc(_$hash, resolvedAtUtc.hashCode);
    _$hash = $jc(_$hash, resolutionNote.hashCode);
    _$hash = $jc(_$hash, verifiedBy.hashCode);
    _$hash = $jc(_$hash, verifiedAtUtc.hashCode);
    _$hash = $jc(_$hash, reopenCount.hashCode);
    _$hash = $jc(_$hash, lastReopenReason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DefectResponse')
          ..add('id', id)
          ..add('no', no)
          ..add('description', description)
          ..add('severity', severity)
          ..add('status', status)
          ..add('reportedBy', reportedBy)
          ..add('reportedAtUtc', reportedAtUtc)
          ..add('resolvedBy', resolvedBy)
          ..add('resolvedAtUtc', resolvedAtUtc)
          ..add('resolutionNote', resolutionNote)
          ..add('verifiedBy', verifiedBy)
          ..add('verifiedAtUtc', verifiedAtUtc)
          ..add('reopenCount', reopenCount)
          ..add('lastReopenReason', lastReopenReason))
        .toString();
  }
}

class DefectResponseBuilder
    implements Builder<DefectResponse, DefectResponseBuilder> {
  _$DefectResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _no;
  int? get no => _$this._no;
  set no(int? no) => _$this._no = no;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  String? _severity;
  String? get severity => _$this._severity;
  set severity(String? severity) => _$this._severity = severity;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  ServicePersonResponseBuilder? _reportedBy;
  ServicePersonResponseBuilder get reportedBy =>
      _$this._reportedBy ??= ServicePersonResponseBuilder();
  set reportedBy(ServicePersonResponseBuilder? reportedBy) =>
      _$this._reportedBy = reportedBy;

  DateTime? _reportedAtUtc;
  DateTime? get reportedAtUtc => _$this._reportedAtUtc;
  set reportedAtUtc(DateTime? reportedAtUtc) =>
      _$this._reportedAtUtc = reportedAtUtc;

  ServicePersonResponseBuilder? _resolvedBy;
  ServicePersonResponseBuilder get resolvedBy =>
      _$this._resolvedBy ??= ServicePersonResponseBuilder();
  set resolvedBy(ServicePersonResponseBuilder? resolvedBy) =>
      _$this._resolvedBy = resolvedBy;

  DateTime? _resolvedAtUtc;
  DateTime? get resolvedAtUtc => _$this._resolvedAtUtc;
  set resolvedAtUtc(DateTime? resolvedAtUtc) =>
      _$this._resolvedAtUtc = resolvedAtUtc;

  String? _resolutionNote;
  String? get resolutionNote => _$this._resolutionNote;
  set resolutionNote(String? resolutionNote) =>
      _$this._resolutionNote = resolutionNote;

  ServicePersonResponseBuilder? _verifiedBy;
  ServicePersonResponseBuilder get verifiedBy =>
      _$this._verifiedBy ??= ServicePersonResponseBuilder();
  set verifiedBy(ServicePersonResponseBuilder? verifiedBy) =>
      _$this._verifiedBy = verifiedBy;

  DateTime? _verifiedAtUtc;
  DateTime? get verifiedAtUtc => _$this._verifiedAtUtc;
  set verifiedAtUtc(DateTime? verifiedAtUtc) =>
      _$this._verifiedAtUtc = verifiedAtUtc;

  int? _reopenCount;
  int? get reopenCount => _$this._reopenCount;
  set reopenCount(int? reopenCount) => _$this._reopenCount = reopenCount;

  String? _lastReopenReason;
  String? get lastReopenReason => _$this._lastReopenReason;
  set lastReopenReason(String? lastReopenReason) =>
      _$this._lastReopenReason = lastReopenReason;

  DefectResponseBuilder() {
    DefectResponse._defaults(this);
  }

  DefectResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _no = $v.no;
      _description = $v.description;
      _severity = $v.severity;
      _status = $v.status;
      _reportedBy = $v.reportedBy?.toBuilder();
      _reportedAtUtc = $v.reportedAtUtc;
      _resolvedBy = $v.resolvedBy?.toBuilder();
      _resolvedAtUtc = $v.resolvedAtUtc;
      _resolutionNote = $v.resolutionNote;
      _verifiedBy = $v.verifiedBy?.toBuilder();
      _verifiedAtUtc = $v.verifiedAtUtc;
      _reopenCount = $v.reopenCount;
      _lastReopenReason = $v.lastReopenReason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DefectResponse other) {
    _$v = other as _$DefectResponse;
  }

  @override
  void update(void Function(DefectResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DefectResponse build() => _build();

  _$DefectResponse _build() {
    _$DefectResponse _$result;
    try {
      _$result = _$v ??
          _$DefectResponse._(
            id: id,
            no: no,
            description: description,
            severity: severity,
            status: status,
            reportedBy: _reportedBy?.build(),
            reportedAtUtc: reportedAtUtc,
            resolvedBy: _resolvedBy?.build(),
            resolvedAtUtc: resolvedAtUtc,
            resolutionNote: resolutionNote,
            verifiedBy: _verifiedBy?.build(),
            verifiedAtUtc: verifiedAtUtc,
            reopenCount: reopenCount,
            lastReopenReason: lastReopenReason,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'reportedBy';
        _reportedBy?.build();

        _$failedField = 'resolvedBy';
        _resolvedBy?.build();

        _$failedField = 'verifiedBy';
        _verifiedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'DefectResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
