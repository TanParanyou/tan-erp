// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'two_factor_backup_codes_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TwoFactorBackupCodesResponse extends TwoFactorBackupCodesResponse {
  @override
  final BuiltList<String>? backupCodes;

  factory _$TwoFactorBackupCodesResponse(
          [void Function(TwoFactorBackupCodesResponseBuilder)? updates]) =>
      (TwoFactorBackupCodesResponseBuilder()..update(updates))._build();

  _$TwoFactorBackupCodesResponse._({this.backupCodes}) : super._();
  @override
  TwoFactorBackupCodesResponse rebuild(
          void Function(TwoFactorBackupCodesResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TwoFactorBackupCodesResponseBuilder toBuilder() =>
      TwoFactorBackupCodesResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TwoFactorBackupCodesResponse &&
        backupCodes == other.backupCodes;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, backupCodes.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TwoFactorBackupCodesResponse')
          ..add('backupCodes', backupCodes))
        .toString();
  }
}

class TwoFactorBackupCodesResponseBuilder
    implements
        Builder<TwoFactorBackupCodesResponse,
            TwoFactorBackupCodesResponseBuilder> {
  _$TwoFactorBackupCodesResponse? _$v;

  ListBuilder<String>? _backupCodes;
  ListBuilder<String> get backupCodes =>
      _$this._backupCodes ??= ListBuilder<String>();
  set backupCodes(ListBuilder<String>? backupCodes) =>
      _$this._backupCodes = backupCodes;

  TwoFactorBackupCodesResponseBuilder() {
    TwoFactorBackupCodesResponse._defaults(this);
  }

  TwoFactorBackupCodesResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _backupCodes = $v.backupCodes?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TwoFactorBackupCodesResponse other) {
    _$v = other as _$TwoFactorBackupCodesResponse;
  }

  @override
  void update(void Function(TwoFactorBackupCodesResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TwoFactorBackupCodesResponse build() => _build();

  _$TwoFactorBackupCodesResponse _build() {
    _$TwoFactorBackupCodesResponse _$result;
    try {
      _$result = _$v ??
          _$TwoFactorBackupCodesResponse._(
            backupCodes: _backupCodes?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'backupCodes';
        _backupCodes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'TwoFactorBackupCodesResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
