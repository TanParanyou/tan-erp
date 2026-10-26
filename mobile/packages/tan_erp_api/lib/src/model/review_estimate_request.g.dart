// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_estimate_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReviewEstimateRequest extends ReviewEstimateRequest {
  @override
  final int? revisionNo;
  @override
  final String? decision;
  @override
  final String? reasonCode;
  @override
  final String? note;

  factory _$ReviewEstimateRequest(
          [void Function(ReviewEstimateRequestBuilder)? updates]) =>
      (ReviewEstimateRequestBuilder()..update(updates))._build();

  _$ReviewEstimateRequest._(
      {this.revisionNo, this.decision, this.reasonCode, this.note})
      : super._();
  @override
  ReviewEstimateRequest rebuild(
          void Function(ReviewEstimateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReviewEstimateRequestBuilder toBuilder() =>
      ReviewEstimateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReviewEstimateRequest &&
        revisionNo == other.revisionNo &&
        decision == other.decision &&
        reasonCode == other.reasonCode &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, revisionNo.hashCode);
    _$hash = $jc(_$hash, decision.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReviewEstimateRequest')
          ..add('revisionNo', revisionNo)
          ..add('decision', decision)
          ..add('reasonCode', reasonCode)
          ..add('note', note))
        .toString();
  }
}

class ReviewEstimateRequestBuilder
    implements Builder<ReviewEstimateRequest, ReviewEstimateRequestBuilder> {
  _$ReviewEstimateRequest? _$v;

  int? _revisionNo;
  int? get revisionNo => _$this._revisionNo;
  set revisionNo(int? revisionNo) => _$this._revisionNo = revisionNo;

  String? _decision;
  String? get decision => _$this._decision;
  set decision(String? decision) => _$this._decision = decision;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ReviewEstimateRequestBuilder() {
    ReviewEstimateRequest._defaults(this);
  }

  ReviewEstimateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _revisionNo = $v.revisionNo;
      _decision = $v.decision;
      _reasonCode = $v.reasonCode;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReviewEstimateRequest other) {
    _$v = other as _$ReviewEstimateRequest;
  }

  @override
  void update(void Function(ReviewEstimateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReviewEstimateRequest build() => _build();

  _$ReviewEstimateRequest _build() {
    final _$result = _$v ??
        _$ReviewEstimateRequest._(
          revisionNo: revisionNo,
          decision: decision,
          reasonCode: reasonCode,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
