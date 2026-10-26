// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_decision_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReviewDecisionRequest extends ReviewDecisionRequest {
  @override
  final int? sourceVersion;
  @override
  final String? decision;
  @override
  final String? reasonCode;
  @override
  final String? note;

  factory _$ReviewDecisionRequest(
          [void Function(ReviewDecisionRequestBuilder)? updates]) =>
      (ReviewDecisionRequestBuilder()..update(updates))._build();

  _$ReviewDecisionRequest._(
      {this.sourceVersion, this.decision, this.reasonCode, this.note})
      : super._();
  @override
  ReviewDecisionRequest rebuild(
          void Function(ReviewDecisionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReviewDecisionRequestBuilder toBuilder() =>
      ReviewDecisionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReviewDecisionRequest &&
        sourceVersion == other.sourceVersion &&
        decision == other.decision &&
        reasonCode == other.reasonCode &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sourceVersion.hashCode);
    _$hash = $jc(_$hash, decision.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReviewDecisionRequest')
          ..add('sourceVersion', sourceVersion)
          ..add('decision', decision)
          ..add('reasonCode', reasonCode)
          ..add('note', note))
        .toString();
  }
}

class ReviewDecisionRequestBuilder
    implements Builder<ReviewDecisionRequest, ReviewDecisionRequestBuilder> {
  _$ReviewDecisionRequest? _$v;

  int? _sourceVersion;
  int? get sourceVersion => _$this._sourceVersion;
  set sourceVersion(int? sourceVersion) =>
      _$this._sourceVersion = sourceVersion;

  String? _decision;
  String? get decision => _$this._decision;
  set decision(String? decision) => _$this._decision = decision;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ReviewDecisionRequestBuilder() {
    ReviewDecisionRequest._defaults(this);
  }

  ReviewDecisionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sourceVersion = $v.sourceVersion;
      _decision = $v.decision;
      _reasonCode = $v.reasonCode;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReviewDecisionRequest other) {
    _$v = other as _$ReviewDecisionRequest;
  }

  @override
  void update(void Function(ReviewDecisionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReviewDecisionRequest build() => _build();

  _$ReviewDecisionRequest _build() {
    final _$result = _$v ??
        _$ReviewDecisionRequest._(
          sourceVersion: sourceVersion,
          decision: decision,
          reasonCode: reasonCode,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
