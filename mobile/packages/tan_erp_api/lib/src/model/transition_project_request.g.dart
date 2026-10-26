// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'transition_project_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TransitionProjectRequest extends TransitionProjectRequest {
  @override
  final String? targetStatus;
  @override
  final String? reason;

  factory _$TransitionProjectRequest(
          [void Function(TransitionProjectRequestBuilder)? updates]) =>
      (TransitionProjectRequestBuilder()..update(updates))._build();

  _$TransitionProjectRequest._({this.targetStatus, this.reason}) : super._();
  @override
  TransitionProjectRequest rebuild(
          void Function(TransitionProjectRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TransitionProjectRequestBuilder toBuilder() =>
      TransitionProjectRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TransitionProjectRequest &&
        targetStatus == other.targetStatus &&
        reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, targetStatus.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TransitionProjectRequest')
          ..add('targetStatus', targetStatus)
          ..add('reason', reason))
        .toString();
  }
}

class TransitionProjectRequestBuilder
    implements
        Builder<TransitionProjectRequest, TransitionProjectRequestBuilder> {
  _$TransitionProjectRequest? _$v;

  String? _targetStatus;
  String? get targetStatus => _$this._targetStatus;
  set targetStatus(String? targetStatus) => _$this._targetStatus = targetStatus;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  TransitionProjectRequestBuilder() {
    TransitionProjectRequest._defaults(this);
  }

  TransitionProjectRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _targetStatus = $v.targetStatus;
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TransitionProjectRequest other) {
    _$v = other as _$TransitionProjectRequest;
  }

  @override
  void update(void Function(TransitionProjectRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TransitionProjectRequest build() => _build();

  _$TransitionProjectRequest _build() {
    final _$result = _$v ??
        _$TransitionProjectRequest._(
          targetStatus: targetStatus,
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
