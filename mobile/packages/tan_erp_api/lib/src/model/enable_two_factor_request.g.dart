// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'enable_two_factor_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EnableTwoFactorRequest extends EnableTwoFactorRequest {
  @override
  final String? code;

  factory _$EnableTwoFactorRequest(
          [void Function(EnableTwoFactorRequestBuilder)? updates]) =>
      (EnableTwoFactorRequestBuilder()..update(updates))._build();

  _$EnableTwoFactorRequest._({this.code}) : super._();
  @override
  EnableTwoFactorRequest rebuild(
          void Function(EnableTwoFactorRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EnableTwoFactorRequestBuilder toBuilder() =>
      EnableTwoFactorRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EnableTwoFactorRequest && code == other.code;
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
    return (newBuiltValueToStringHelper(r'EnableTwoFactorRequest')
          ..add('code', code))
        .toString();
  }
}

class EnableTwoFactorRequestBuilder
    implements Builder<EnableTwoFactorRequest, EnableTwoFactorRequestBuilder> {
  _$EnableTwoFactorRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  EnableTwoFactorRequestBuilder() {
    EnableTwoFactorRequest._defaults(this);
  }

  EnableTwoFactorRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EnableTwoFactorRequest other) {
    _$v = other as _$EnableTwoFactorRequest;
  }

  @override
  void update(void Function(EnableTwoFactorRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EnableTwoFactorRequest build() => _build();

  _$EnableTwoFactorRequest _build() {
    final _$result = _$v ??
        _$EnableTwoFactorRequest._(
          code: code,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
