// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'disable_two_factor_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DisableTwoFactorRequest extends DisableTwoFactorRequest {
  @override
  final String? code;

  factory _$DisableTwoFactorRequest(
          [void Function(DisableTwoFactorRequestBuilder)? updates]) =>
      (DisableTwoFactorRequestBuilder()..update(updates))._build();

  _$DisableTwoFactorRequest._({this.code}) : super._();
  @override
  DisableTwoFactorRequest rebuild(
          void Function(DisableTwoFactorRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DisableTwoFactorRequestBuilder toBuilder() =>
      DisableTwoFactorRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DisableTwoFactorRequest && code == other.code;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DisableTwoFactorRequest')
          ..add('code', code))
        .toString();
  }
}

class DisableTwoFactorRequestBuilder
    implements
        Builder<DisableTwoFactorRequest, DisableTwoFactorRequestBuilder> {
  _$DisableTwoFactorRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  DisableTwoFactorRequestBuilder() {
    DisableTwoFactorRequest._defaults(this);
  }

  DisableTwoFactorRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DisableTwoFactorRequest other) {
    _$v = other as _$DisableTwoFactorRequest;
  }

  @override
  void update(void Function(DisableTwoFactorRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DisableTwoFactorRequest build() => _build();

  _$DisableTwoFactorRequest _build() {
    final _$result = _$v ??
        _$DisableTwoFactorRequest._(
          code: code,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
