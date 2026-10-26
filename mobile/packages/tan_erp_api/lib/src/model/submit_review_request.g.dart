// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'submit_review_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SubmitReviewRequest extends SubmitReviewRequest {
  @override
  final int? sourceVersion;
  @override
  final String? note;

  factory _$SubmitReviewRequest(
          [void Function(SubmitReviewRequestBuilder)? updates]) =>
      (SubmitReviewRequestBuilder()..update(updates))._build();

  _$SubmitReviewRequest._({this.sourceVersion, this.note}) : super._();
  @override
  SubmitReviewRequest rebuild(
          void Function(SubmitReviewRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SubmitReviewRequestBuilder toBuilder() =>
      SubmitReviewRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SubmitReviewRequest &&
        sourceVersion == other.sourceVersion &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sourceVersion.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SubmitReviewRequest')
          ..add('sourceVersion', sourceVersion)
          ..add('note', note))
        .toString();
  }
}

class SubmitReviewRequestBuilder
    implements Builder<SubmitReviewRequest, SubmitReviewRequestBuilder> {
  _$SubmitReviewRequest? _$v;

  int? _sourceVersion;
  int? get sourceVersion => _$this._sourceVersion;
  set sourceVersion(int? sourceVersion) =>
      _$this._sourceVersion = sourceVersion;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  SubmitReviewRequestBuilder() {
    SubmitReviewRequest._defaults(this);
  }

  SubmitReviewRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sourceVersion = $v.sourceVersion;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SubmitReviewRequest other) {
    _$v = other as _$SubmitReviewRequest;
  }

  @override
  void update(void Function(SubmitReviewRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SubmitReviewRequest build() => _build();

  _$SubmitReviewRequest _build() {
    final _$result = _$v ??
        _$SubmitReviewRequest._(
          sourceVersion: sourceVersion,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
