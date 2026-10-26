// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'approval_limit_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ApprovalLimitResponse extends ApprovalLimitResponse {
  @override
  final String? id;
  @override
  final String? rowVersion;
  @override
  final AdminRefResponse? role;
  @override
  final String? documentType;
  @override
  final AdminRefResponse? branch;
  @override
  final String? currency;
  @override
  final bool? isUnlimited;
  @override
  final double? maxAmount;
  @override
  final bool? isActive;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;

  factory _$ApprovalLimitResponse(
          [void Function(ApprovalLimitResponseBuilder)? updates]) =>
      (ApprovalLimitResponseBuilder()..update(updates))._build();

  _$ApprovalLimitResponse._(
      {this.id,
      this.rowVersion,
      this.role,
      this.documentType,
      this.branch,
      this.currency,
      this.isUnlimited,
      this.maxAmount,
      this.isActive,
      this.createdAtUtc,
      this.updatedAtUtc})
      : super._();
  @override
  ApprovalLimitResponse rebuild(
          void Function(ApprovalLimitResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ApprovalLimitResponseBuilder toBuilder() =>
      ApprovalLimitResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ApprovalLimitResponse &&
        id == other.id &&
        rowVersion == other.rowVersion &&
        role == other.role &&
        documentType == other.documentType &&
        branch == other.branch &&
        currency == other.currency &&
        isUnlimited == other.isUnlimited &&
        maxAmount == other.maxAmount &&
        isActive == other.isActive &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, documentType.hashCode);
    _$hash = $jc(_$hash, branch.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, isUnlimited.hashCode);
    _$hash = $jc(_$hash, maxAmount.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ApprovalLimitResponse')
          ..add('id', id)
          ..add('rowVersion', rowVersion)
          ..add('role', role)
          ..add('documentType', documentType)
          ..add('branch', branch)
          ..add('currency', currency)
          ..add('isUnlimited', isUnlimited)
          ..add('maxAmount', maxAmount)
          ..add('isActive', isActive)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class ApprovalLimitResponseBuilder
    implements Builder<ApprovalLimitResponse, ApprovalLimitResponseBuilder> {
  _$ApprovalLimitResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  AdminRefResponseBuilder? _role;
  AdminRefResponseBuilder get role =>
      _$this._role ??= AdminRefResponseBuilder();
  set role(AdminRefResponseBuilder? role) => _$this._role = role;

  String? _documentType;
  String? get documentType => _$this._documentType;
  set documentType(String? documentType) => _$this._documentType = documentType;

  AdminRefResponseBuilder? _branch;
  AdminRefResponseBuilder get branch =>
      _$this._branch ??= AdminRefResponseBuilder();
  set branch(AdminRefResponseBuilder? branch) => _$this._branch = branch;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  bool? _isUnlimited;
  bool? get isUnlimited => _$this._isUnlimited;
  set isUnlimited(bool? isUnlimited) => _$this._isUnlimited = isUnlimited;

  double? _maxAmount;
  double? get maxAmount => _$this._maxAmount;
  set maxAmount(double? maxAmount) => _$this._maxAmount = maxAmount;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  ApprovalLimitResponseBuilder() {
    ApprovalLimitResponse._defaults(this);
  }

  ApprovalLimitResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _rowVersion = $v.rowVersion;
      _role = $v.role?.toBuilder();
      _documentType = $v.documentType;
      _branch = $v.branch?.toBuilder();
      _currency = $v.currency;
      _isUnlimited = $v.isUnlimited;
      _maxAmount = $v.maxAmount;
      _isActive = $v.isActive;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ApprovalLimitResponse other) {
    _$v = other as _$ApprovalLimitResponse;
  }

  @override
  void update(void Function(ApprovalLimitResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ApprovalLimitResponse build() => _build();

  _$ApprovalLimitResponse _build() {
    _$ApprovalLimitResponse _$result;
    try {
      _$result = _$v ??
          _$ApprovalLimitResponse._(
            id: id,
            rowVersion: rowVersion,
            role: _role?.build(),
            documentType: documentType,
            branch: _branch?.build(),
            currency: currency,
            isUnlimited: isUnlimited,
            maxAmount: maxAmount,
            isActive: isActive,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'role';
        _role?.build();

        _$failedField = 'branch';
        _branch?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ApprovalLimitResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
