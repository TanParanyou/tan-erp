// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingListItemResponse extends BillingListItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? projectCode;
  @override
  final String? kind;
  @override
  final double? amount;
  @override
  final double? paidAmount;
  @override
  final String? status;
  @override
  final Date? dueDate;
  @override
  final DateTime? issuedAtUtc;

  factory _$BillingListItemResponse(
          [void Function(BillingListItemResponseBuilder)? updates]) =>
      (BillingListItemResponseBuilder()..update(updates))._build();

  _$BillingListItemResponse._(
      {this.id,
      this.number,
      this.projectCode,
      this.kind,
      this.amount,
      this.paidAmount,
      this.status,
      this.dueDate,
      this.issuedAtUtc})
      : super._();
  @override
  BillingListItemResponse rebuild(
          void Function(BillingListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingListItemResponseBuilder toBuilder() =>
      BillingListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingListItemResponse &&
        id == other.id &&
        number == other.number &&
        projectCode == other.projectCode &&
        kind == other.kind &&
        amount == other.amount &&
        paidAmount == other.paidAmount &&
        status == other.status &&
        dueDate == other.dueDate &&
        issuedAtUtc == other.issuedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, paidAmount.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, dueDate.hashCode);
    _$hash = $jc(_$hash, issuedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BillingListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('projectCode', projectCode)
          ..add('kind', kind)
          ..add('amount', amount)
          ..add('paidAmount', paidAmount)
          ..add('status', status)
          ..add('dueDate', dueDate)
          ..add('issuedAtUtc', issuedAtUtc))
        .toString();
  }
}

class BillingListItemResponseBuilder
    implements
        Builder<BillingListItemResponse, BillingListItemResponseBuilder> {
  _$BillingListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  double? _paidAmount;
  double? get paidAmount => _$this._paidAmount;
  set paidAmount(double? paidAmount) => _$this._paidAmount = paidAmount;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  Date? _dueDate;
  Date? get dueDate => _$this._dueDate;
  set dueDate(Date? dueDate) => _$this._dueDate = dueDate;

  DateTime? _issuedAtUtc;
  DateTime? get issuedAtUtc => _$this._issuedAtUtc;
  set issuedAtUtc(DateTime? issuedAtUtc) => _$this._issuedAtUtc = issuedAtUtc;

  BillingListItemResponseBuilder() {
    BillingListItemResponse._defaults(this);
  }

  BillingListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _projectCode = $v.projectCode;
      _kind = $v.kind;
      _amount = $v.amount;
      _paidAmount = $v.paidAmount;
      _status = $v.status;
      _dueDate = $v.dueDate;
      _issuedAtUtc = $v.issuedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingListItemResponse other) {
    _$v = other as _$BillingListItemResponse;
  }

  @override
  void update(void Function(BillingListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingListItemResponse build() => _build();

  _$BillingListItemResponse _build() {
    final _$result = _$v ??
        _$BillingListItemResponse._(
          id: id,
          number: number,
          projectCode: projectCode,
          kind: kind,
          amount: amount,
          paidAmount: paidAmount,
          status: status,
          dueDate: dueDate,
          issuedAtUtc: issuedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
