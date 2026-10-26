// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_survey_ready_revision_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSurveyReadyRevisionResponse
    extends SiteSurveyReadyRevisionResponse {
  @override
  final String? id;
  @override
  final int? revisionNumber;
  @override
  final String? snapshotHash;

  factory _$SiteSurveyReadyRevisionResponse(
          [void Function(SiteSurveyReadyRevisionResponseBuilder)? updates]) =>
      (SiteSurveyReadyRevisionResponseBuilder()..update(updates))._build();

  _$SiteSurveyReadyRevisionResponse._(
      {this.id, this.revisionNumber, this.snapshotHash})
      : super._();
  @override
  SiteSurveyReadyRevisionResponse rebuild(
          void Function(SiteSurveyReadyRevisionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSurveyReadyRevisionResponseBuilder toBuilder() =>
      SiteSurveyReadyRevisionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSurveyReadyRevisionResponse &&
        id == other.id &&
        revisionNumber == other.revisionNumber &&
        snapshotHash == other.snapshotHash;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, revisionNumber.hashCode);
    _$hash = $jc(_$hash, snapshotHash.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSurveyReadyRevisionResponse')
          ..add('id', id)
          ..add('revisionNumber', revisionNumber)
          ..add('snapshotHash', snapshotHash))
        .toString();
  }
}

class SiteSurveyReadyRevisionResponseBuilder
    implements
        Builder<SiteSurveyReadyRevisionResponse,
            SiteSurveyReadyRevisionResponseBuilder> {
  _$SiteSurveyReadyRevisionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _revisionNumber;
  int? get revisionNumber => _$this._revisionNumber;
  set revisionNumber(int? revisionNumber) =>
      _$this._revisionNumber = revisionNumber;

  String? _snapshotHash;
  String? get snapshotHash => _$this._snapshotHash;
  set snapshotHash(String? snapshotHash) => _$this._snapshotHash = snapshotHash;

  SiteSurveyReadyRevisionResponseBuilder() {
    SiteSurveyReadyRevisionResponse._defaults(this);
  }

  SiteSurveyReadyRevisionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _revisionNumber = $v.revisionNumber;
      _snapshotHash = $v.snapshotHash;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSurveyReadyRevisionResponse other) {
    _$v = other as _$SiteSurveyReadyRevisionResponse;
  }

  @override
  void update(void Function(SiteSurveyReadyRevisionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSurveyReadyRevisionResponse build() => _build();

  _$SiteSurveyReadyRevisionResponse _build() {
    final _$result = _$v ??
        _$SiteSurveyReadyRevisionResponse._(
          id: id,
          revisionNumber: revisionNumber,
          snapshotHash: snapshotHash,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
