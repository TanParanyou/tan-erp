// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'submit_estimate_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SubmitEstimateRequest extends SubmitEstimateRequest {
  @override
  final int? revisionNo;
  @override
  final int? calculationVersion;
  @override
  final String? note;

  factory _$SubmitEstimateRequest(
          [void Function(SubmitEstimateRequestBuilder)? updates]) =>
      (SubmitEstimateRequestBuilder()..update(updates))._build();

  _$SubmitEstimateRequest._(
      {this.revisionNo, this.calculationVersion, this.note})
      : super._();
  @override
  SubmitEstimateRequest rebuild(
          void Function(SubmitEstimateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SubmitEstimateRequestBuilder toBuilder() =>
      SubmitEstimateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SubmitEstimateRequest &&
        revisionNo == other.revisionNo &&
        calculationVersion == other.calculationVersion &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, revisionNo.hashCode);
    _$hash = $jc(_$hash, calculationVersion.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SubmitEstimateRequest')
          ..add('revisionNo', revisionNo)
          ..add('calculationVersion', calculationVersion)
          ..add('note', note))
        .toString();
  }
}

class SubmitEstimateRequestBuilder
    implements Builder<SubmitEstimateRequest, SubmitEstimateRequestBuilder> {
  _$SubmitEstimateRequest? _$v;

  int? _revisionNo;
  int? get revisionNo => _$this._revisionNo;
  set revisionNo(int? revisionNo) => _$this._revisionNo = revisionNo;

  int? _calculationVersion;
  int? get calculationVersion => _$this._calculationVersion;
  set calculationVersion(int? calculationVersion) =>
      _$this._calculationVersion = calculationVersion;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  SubmitEstimateRequestBuilder() {
    SubmitEstimateRequest._defaults(this);
  }

  SubmitEstimateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _revisionNo = $v.revisionNo;
      _calculationVersion = $v.calculationVersion;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SubmitEstimateRequest other) {
    _$v = other as _$SubmitEstimateRequest;
  }

  @override
  void update(void Function(SubmitEstimateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SubmitEstimateRequest build() => _build();

  _$SubmitEstimateRequest _build() {
    final _$result = _$v ??
        _$SubmitEstimateRequest._(
          revisionNo: revisionNo,
          calculationVersion: calculationVersion,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
