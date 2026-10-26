// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'approval_limit_options_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ApprovalLimitOptionsResponse extends ApprovalLimitOptionsResponse {
  @override
  final BuiltList<AdminRefResponse>? roles;
  @override
  final BuiltList<AdminRefResponse>? branches;
  @override
  final BuiltList<String>? documentTypes;
  @override
  final BuiltList<String>? currencies;

  factory _$ApprovalLimitOptionsResponse(
          [void Function(ApprovalLimitOptionsResponseBuilder)? updates]) =>
      (ApprovalLimitOptionsResponseBuilder()..update(updates))._build();

  _$ApprovalLimitOptionsResponse._(
      {this.roles, this.branches, this.documentTypes, this.currencies})
      : super._();
  @override
  ApprovalLimitOptionsResponse rebuild(
          void Function(ApprovalLimitOptionsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ApprovalLimitOptionsResponseBuilder toBuilder() =>
      ApprovalLimitOptionsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ApprovalLimitOptionsResponse &&
        roles == other.roles &&
        branches == other.branches &&
        documentTypes == other.documentTypes &&
        currencies == other.currencies;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, roles.hashCode);
    _$hash = $jc(_$hash, branches.hashCode);
    _$hash = $jc(_$hash, documentTypes.hashCode);
    _$hash = $jc(_$hash, currencies.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ApprovalLimitOptionsResponse')
          ..add('roles', roles)
          ..add('branches', branches)
          ..add('documentTypes', documentTypes)
          ..add('currencies', currencies))
        .toString();
  }
}

class ApprovalLimitOptionsResponseBuilder
    implements
        Builder<ApprovalLimitOptionsResponse,
            ApprovalLimitOptionsResponseBuilder> {
  _$ApprovalLimitOptionsResponse? _$v;

  ListBuilder<AdminRefResponse>? _roles;
  ListBuilder<AdminRefResponse> get roles =>
      _$this._roles ??= ListBuilder<AdminRefResponse>();
  set roles(ListBuilder<AdminRefResponse>? roles) => _$this._roles = roles;

  ListBuilder<AdminRefResponse>? _branches;
  ListBuilder<AdminRefResponse> get branches =>
      _$this._branches ??= ListBuilder<AdminRefResponse>();
  set branches(ListBuilder<AdminRefResponse>? branches) =>
      _$this._branches = branches;

  ListBuilder<String>? _documentTypes;
  ListBuilder<String> get documentTypes =>
      _$this._documentTypes ??= ListBuilder<String>();
  set documentTypes(ListBuilder<String>? documentTypes) =>
      _$this._documentTypes = documentTypes;

  ListBuilder<String>? _currencies;
  ListBuilder<String> get currencies =>
      _$this._currencies ??= ListBuilder<String>();
  set currencies(ListBuilder<String>? currencies) =>
      _$this._currencies = currencies;

  ApprovalLimitOptionsResponseBuilder() {
    ApprovalLimitOptionsResponse._defaults(this);
  }

  ApprovalLimitOptionsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _roles = $v.roles?.toBuilder();
      _branches = $v.branches?.toBuilder();
      _documentTypes = $v.documentTypes?.toBuilder();
      _currencies = $v.currencies?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ApprovalLimitOptionsResponse other) {
    _$v = other as _$ApprovalLimitOptionsResponse;
  }

  @override
  void update(void Function(ApprovalLimitOptionsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ApprovalLimitOptionsResponse build() => _build();

  _$ApprovalLimitOptionsResponse _build() {
    _$ApprovalLimitOptionsResponse _$result;
    try {
      _$result = _$v ??
          _$ApprovalLimitOptionsResponse._(
            roles: _roles?.build(),
            branches: _branches?.build(),
            documentTypes: _documentTypes?.build(),
            currencies: _currencies?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'roles';
        _roles?.build();
        _$failedField = 'branches';
        _branches?.build();
        _$failedField = 'documentTypes';
        _documentTypes?.build();
        _$failedField = 'currencies';
        _currencies?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ApprovalLimitOptionsResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
