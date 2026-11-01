// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_mapping_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AccountMappingResponse extends AccountMappingResponse {
  @override
  final String? id;
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
  @override
  final FinancePersonResponse? updatedBy;
  @override
  final DateTime? updatedAtUtc;
  @override
  final String? rowVersion;

  factory _$AccountMappingResponse(
          [void Function(AccountMappingResponseBuilder)? updates]) =>
      (AccountMappingResponseBuilder()..update(updates))._build();

  _$AccountMappingResponse._(
      {this.id,
      this.kind,
      this.debitAccountCode,
      this.debitAccountName,
      this.creditAccountCode,
      this.creditAccountName,
      this.updatedBy,
      this.updatedAtUtc,
      this.rowVersion})
      : super._();
  @override
  AccountMappingResponse rebuild(
          void Function(AccountMappingResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AccountMappingResponseBuilder toBuilder() =>
      AccountMappingResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AccountMappingResponse &&
        id == other.id &&
        kind == other.kind &&
        debitAccountCode == other.debitAccountCode &&
        debitAccountName == other.debitAccountName &&
        creditAccountCode == other.creditAccountCode &&
        creditAccountName == other.creditAccountName &&
        updatedBy == other.updatedBy &&
        updatedAtUtc == other.updatedAtUtc &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, debitAccountCode.hashCode);
    _$hash = $jc(_$hash, debitAccountName.hashCode);
    _$hash = $jc(_$hash, creditAccountCode.hashCode);
    _$hash = $jc(_$hash, creditAccountName.hashCode);
    _$hash = $jc(_$hash, updatedBy.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AccountMappingResponse')
          ..add('id', id)
          ..add('kind', kind)
          ..add('debitAccountCode', debitAccountCode)
          ..add('debitAccountName', debitAccountName)
          ..add('creditAccountCode', creditAccountCode)
          ..add('creditAccountName', creditAccountName)
          ..add('updatedBy', updatedBy)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class AccountMappingResponseBuilder
    implements Builder<AccountMappingResponse, AccountMappingResponseBuilder> {
  _$AccountMappingResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  FinancePersonResponseBuilder? _updatedBy;
  FinancePersonResponseBuilder get updatedBy =>
      _$this._updatedBy ??= FinancePersonResponseBuilder();
  set updatedBy(FinancePersonResponseBuilder? updatedBy) =>
      _$this._updatedBy = updatedBy;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  AccountMappingResponseBuilder() {
    AccountMappingResponse._defaults(this);
  }

  AccountMappingResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _kind = $v.kind;
      _debitAccountCode = $v.debitAccountCode;
      _debitAccountName = $v.debitAccountName;
      _creditAccountCode = $v.creditAccountCode;
      _creditAccountName = $v.creditAccountName;
      _updatedBy = $v.updatedBy?.toBuilder();
      _updatedAtUtc = $v.updatedAtUtc;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AccountMappingResponse other) {
    _$v = other as _$AccountMappingResponse;
  }

  @override
  void update(void Function(AccountMappingResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AccountMappingResponse build() => _build();

  _$AccountMappingResponse _build() {
    _$AccountMappingResponse _$result;
    try {
      _$result = _$v ??
          _$AccountMappingResponse._(
            id: id,
            kind: kind,
            debitAccountCode: debitAccountCode,
            debitAccountName: debitAccountName,
            creditAccountCode: creditAccountCode,
            creditAccountName: creditAccountName,
            updatedBy: _updatedBy?.build(),
            updatedAtUtc: updatedAtUtc,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'updatedBy';
        _updatedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AccountMappingResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
