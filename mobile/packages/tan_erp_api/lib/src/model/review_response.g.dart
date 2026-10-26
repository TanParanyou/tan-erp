// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReviewResponse extends ReviewResponse {
  @override
  final String? id;
  @override
  final int? sourceVersion;
  @override
  final String? status;
  @override
  final String? requestNote;
  @override
  final QePersonResponse? requestedBy;
  @override
  final DateTime? requestedAtUtc;
  @override
  final QePersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final String? reasonCode;
  @override
  final String? decisionNote;

  factory _$ReviewResponse([void Function(ReviewResponseBuilder)? updates]) =>
      (ReviewResponseBuilder()..update(updates))._build();

  _$ReviewResponse._(
      {this.id,
      this.sourceVersion,
      this.status,
      this.requestNote,
      this.requestedBy,
      this.requestedAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.reasonCode,
      this.decisionNote})
      : super._();
  @override
  ReviewResponse rebuild(void Function(ReviewResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReviewResponseBuilder toBuilder() => ReviewResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReviewResponse &&
        id == other.id &&
        sourceVersion == other.sourceVersion &&
        status == other.status &&
        requestNote == other.requestNote &&
        requestedBy == other.requestedBy &&
        requestedAtUtc == other.requestedAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        reasonCode == other.reasonCode &&
        decisionNote == other.decisionNote;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, sourceVersion.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, requestNote.hashCode);
    _$hash = $jc(_$hash, requestedBy.hashCode);
    _$hash = $jc(_$hash, requestedAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReviewResponse')
          ..add('id', id)
          ..add('sourceVersion', sourceVersion)
          ..add('status', status)
          ..add('requestNote', requestNote)
          ..add('requestedBy', requestedBy)
          ..add('requestedAtUtc', requestedAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('reasonCode', reasonCode)
          ..add('decisionNote', decisionNote))
        .toString();
  }
}

class ReviewResponseBuilder
    implements Builder<ReviewResponse, ReviewResponseBuilder> {
  _$ReviewResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _sourceVersion;
  int? get sourceVersion => _$this._sourceVersion;
  set sourceVersion(int? sourceVersion) =>
      _$this._sourceVersion = sourceVersion;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _requestNote;
  String? get requestNote => _$this._requestNote;
  set requestNote(String? requestNote) => _$this._requestNote = requestNote;

  QePersonResponseBuilder? _requestedBy;
  QePersonResponseBuilder get requestedBy =>
      _$this._requestedBy ??= QePersonResponseBuilder();
  set requestedBy(QePersonResponseBuilder? requestedBy) =>
      _$this._requestedBy = requestedBy;

  DateTime? _requestedAtUtc;
  DateTime? get requestedAtUtc => _$this._requestedAtUtc;
  set requestedAtUtc(DateTime? requestedAtUtc) =>
      _$this._requestedAtUtc = requestedAtUtc;

  QePersonResponseBuilder? _decidedBy;
  QePersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= QePersonResponseBuilder();
  set decidedBy(QePersonResponseBuilder? decidedBy) =>
      _$this._decidedBy = decidedBy;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _decisionNote;
  String? get decisionNote => _$this._decisionNote;
  set decisionNote(String? decisionNote) => _$this._decisionNote = decisionNote;

  ReviewResponseBuilder() {
    ReviewResponse._defaults(this);
  }

  ReviewResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _sourceVersion = $v.sourceVersion;
      _status = $v.status;
      _requestNote = $v.requestNote;
      _requestedBy = $v.requestedBy?.toBuilder();
      _requestedAtUtc = $v.requestedAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _reasonCode = $v.reasonCode;
      _decisionNote = $v.decisionNote;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReviewResponse other) {
    _$v = other as _$ReviewResponse;
  }

  @override
  void update(void Function(ReviewResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReviewResponse build() => _build();

  _$ReviewResponse _build() {
    _$ReviewResponse _$result;
    try {
      _$result = _$v ??
          _$ReviewResponse._(
            id: id,
            sourceVersion: sourceVersion,
            status: status,
            requestNote: requestNote,
            requestedBy: _requestedBy?.build(),
            requestedAtUtc: requestedAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            reasonCode: reasonCode,
            decisionNote: decisionNote,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'requestedBy';
        _requestedBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ReviewResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
