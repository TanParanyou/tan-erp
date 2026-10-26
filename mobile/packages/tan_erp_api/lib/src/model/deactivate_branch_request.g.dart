// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deactivate_branch_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DeactivateBranchRequest extends DeactivateBranchRequest {
  @override
  final String? reason;

  factory _$DeactivateBranchRequest(
          [void Function(DeactivateBranchRequestBuilder)? updates]) =>
      (DeactivateBranchRequestBuilder()..update(updates))._build();

  _$DeactivateBranchRequest._({this.reason}) : super._();
  @override
  DeactivateBranchRequest rebuild(
          void Function(DeactivateBranchRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DeactivateBranchRequestBuilder toBuilder() =>
      DeactivateBranchRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DeactivateBranchRequest && reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DeactivateBranchRequest')
          ..add('reason', reason))
        .toString();
  }
}

class DeactivateBranchRequestBuilder
    implements
        Builder<DeactivateBranchRequest, DeactivateBranchRequestBuilder> {
  _$DeactivateBranchRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  DeactivateBranchRequestBuilder() {
    DeactivateBranchRequest._defaults(this);
  }

  DeactivateBranchRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DeactivateBranchRequest other) {
    _$v = other as _$DeactivateBranchRequest;
  }

  @override
  void update(void Function(DeactivateBranchRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DeactivateBranchRequest build() => _build();

  _$DeactivateBranchRequest _build() {
    final _$result = _$v ??
        _$DeactivateBranchRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
