// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_approval_limit_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateApprovalLimitRequest extends CreateApprovalLimitRequest {
  @override
  final String? roleId;
  @override
  final String? documentType;
  @override
  final String? branchId;
  @override
  final String? currency;
  @override
  final bool? isUnlimited;
  @override
  final double? maxAmount;

  factory _$CreateApprovalLimitRequest(
          [void Function(CreateApprovalLimitRequestBuilder)? updates]) =>
      (CreateApprovalLimitRequestBuilder()..update(updates))._build();

  _$CreateApprovalLimitRequest._(
      {this.roleId,
      this.documentType,
      this.branchId,
      this.currency,
      this.isUnlimited,
      this.maxAmount})
      : super._();
  @override
  CreateApprovalLimitRequest rebuild(
          void Function(CreateApprovalLimitRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateApprovalLimitRequestBuilder toBuilder() =>
      CreateApprovalLimitRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateApprovalLimitRequest &&
        roleId == other.roleId &&
        documentType == other.documentType &&
        branchId == other.branchId &&
        currency == other.currency &&
        isUnlimited == other.isUnlimited &&
        maxAmount == other.maxAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, roleId.hashCode);
    _$hash = $jc(_$hash, documentType.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, isUnlimited.hashCode);
    _$hash = $jc(_$hash, maxAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateApprovalLimitRequest')
          ..add('roleId', roleId)
          ..add('documentType', documentType)
          ..add('branchId', branchId)
          ..add('currency', currency)
          ..add('isUnlimited', isUnlimited)
          ..add('maxAmount', maxAmount))
        .toString();
  }
}

class CreateApprovalLimitRequestBuilder
    implements
        Builder<CreateApprovalLimitRequest, CreateApprovalLimitRequestBuilder> {
  _$CreateApprovalLimitRequest? _$v;

  String? _roleId;
  String? get roleId => _$this._roleId;
  set roleId(String? roleId) => _$this._roleId = roleId;

  String? _documentType;
  String? get documentType => _$this._documentType;
  set documentType(String? documentType) => _$this._documentType = documentType;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  bool? _isUnlimited;
  bool? get isUnlimited => _$this._isUnlimited;
  set isUnlimited(bool? isUnlimited) => _$this._isUnlimited = isUnlimited;

  double? _maxAmount;
  double? get maxAmount => _$this._maxAmount;
  set maxAmount(double? maxAmount) => _$this._maxAmount = maxAmount;

  CreateApprovalLimitRequestBuilder() {
    CreateApprovalLimitRequest._defaults(this);
  }

  CreateApprovalLimitRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _roleId = $v.roleId;
      _documentType = $v.documentType;
      _branchId = $v.branchId;
      _currency = $v.currency;
      _isUnlimited = $v.isUnlimited;
      _maxAmount = $v.maxAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateApprovalLimitRequest other) {
    _$v = other as _$CreateApprovalLimitRequest;
  }

  @override
  void update(void Function(CreateApprovalLimitRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateApprovalLimitRequest build() => _build();

  _$CreateApprovalLimitRequest _build() {
    final _$result = _$v ??
        _$CreateApprovalLimitRequest._(
          roleId: roleId,
          documentType: documentType,
          branchId: branchId,
          currency: currency,
          isUnlimited: isUnlimited,
          maxAmount: maxAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
