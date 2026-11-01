// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'regenerate_backup_codes_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RegenerateBackupCodesRequest extends RegenerateBackupCodesRequest {
  @override
  final String? code;

  factory _$RegenerateBackupCodesRequest(
          [void Function(RegenerateBackupCodesRequestBuilder)? updates]) =>
      (RegenerateBackupCodesRequestBuilder()..update(updates))._build();

  _$RegenerateBackupCodesRequest._({this.code}) : super._();
  @override
  RegenerateBackupCodesRequest rebuild(
          void Function(RegenerateBackupCodesRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RegenerateBackupCodesRequestBuilder toBuilder() =>
      RegenerateBackupCodesRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RegenerateBackupCodesRequest && code == other.code;
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
    return (newBuiltValueToStringHelper(r'RegenerateBackupCodesRequest')
          ..add('code', code))
        .toString();
  }
}

class RegenerateBackupCodesRequestBuilder
    implements
        Builder<RegenerateBackupCodesRequest,
            RegenerateBackupCodesRequestBuilder> {
  _$RegenerateBackupCodesRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  RegenerateBackupCodesRequestBuilder() {
    RegenerateBackupCodesRequest._defaults(this);
  }

  RegenerateBackupCodesRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RegenerateBackupCodesRequest other) {
    _$v = other as _$RegenerateBackupCodesRequest;
  }

  @override
  void update(void Function(RegenerateBackupCodesRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RegenerateBackupCodesRequest build() => _build();

  _$RegenerateBackupCodesRequest _build() {
    final _$result = _$v ??
        _$RegenerateBackupCodesRequest._(
          code: code,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
