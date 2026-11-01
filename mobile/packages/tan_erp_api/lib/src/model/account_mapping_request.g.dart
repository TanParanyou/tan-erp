// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_mapping_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AccountMappingRequest extends AccountMappingRequest {
  @override
  final String? kind;
  @override
  final String? debitAccountCode;
  @override
  final String? debitAccountName;
  @override
  final String? creditAccountCode;
  @override
  final String? creditAccountName;

  factory _$AccountMappingRequest(
          [void Function(AccountMappingRequestBuilder)? updates]) =>
      (AccountMappingRequestBuilder()..update(updates))._build();

  _$AccountMappingRequest._(
      {this.kind,
      this.debitAccountCode,
      this.debitAccountName,
      this.creditAccountCode,
      this.creditAccountName})
      : super._();
  @override
  AccountMappingRequest rebuild(
          void Function(AccountMappingRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AccountMappingRequestBuilder toBuilder() =>
      AccountMappingRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AccountMappingRequest &&
        kind == other.kind &&
        debitAccountCode == other.debitAccountCode &&
        debitAccountName == other.debitAccountName &&
        creditAccountCode == other.creditAccountCode &&
        creditAccountName == other.creditAccountName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, debitAccountCode.hashCode);
    _$hash = $jc(_$hash, debitAccountName.hashCode);
    _$hash = $jc(_$hash, creditAccountCode.hashCode);
    _$hash = $jc(_$hash, creditAccountName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AccountMappingRequest')
          ..add('kind', kind)
          ..add('debitAccountCode', debitAccountCode)
          ..add('debitAccountName', debitAccountName)
          ..add('creditAccountCode', creditAccountCode)
          ..add('creditAccountName', creditAccountName))
        .toString();
  }
}

class AccountMappingRequestBuilder
    implements Builder<AccountMappingRequest, AccountMappingRequestBuilder> {
  _$AccountMappingRequest? _$v;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _debitAccountCode;
  String? get debitAccountCode => _$this._debitAccountCode;
  set debitAccountCode(String? debitAccountCode) =>
      _$this._debitAccountCode = debitAccountCode;

  String? _debitAccountName;
  String? get debitAccountName => _$this._debitAccountName;
  set debitAccountName(String? debitAccountName) =>
      _$this._debitAccountName = debitAccountName;

  String? _creditAccountCode;
  String? get creditAccountCode => _$this._creditAccountCode;
  set creditAccountCode(String? creditAccountCode) =>
      _$this._creditAccountCode = creditAccountCode;

  String? _creditAccountName;
  String? get creditAccountName => _$this._creditAccountName;
  set creditAccountName(String? creditAccountName) =>
      _$this._creditAccountName = creditAccountName;

  AccountMappingRequestBuilder() {
    AccountMappingRequest._defaults(this);
  }

  AccountMappingRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _kind = $v.kind;
      _debitAccountCode = $v.debitAccountCode;
      _debitAccountName = $v.debitAccountName;
      _creditAccountCode = $v.creditAccountCode;
      _creditAccountName = $v.creditAccountName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AccountMappingRequest other) {
    _$v = other as _$AccountMappingRequest;
  }

  @override
  void update(void Function(AccountMappingRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AccountMappingRequest build() => _build();

  _$AccountMappingRequest _build() {
    final _$result = _$v ??
        _$AccountMappingRequest._(
          kind: kind,
          debitAccountCode: debitAccountCode,
          debitAccountName: debitAccountName,
          creditAccountCode: creditAccountCode,
          creditAccountName: creditAccountName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
