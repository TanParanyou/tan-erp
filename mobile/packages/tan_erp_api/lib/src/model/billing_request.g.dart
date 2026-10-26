// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingRequest extends BillingRequest {
  @override
  final String? projectId;
  @override
  final String? kind;
  @override
  final String? description;
  @override
  final double? amount;
  @override
  final Date? dueDate;

  factory _$BillingRequest([void Function(BillingRequestBuilder)? updates]) =>
      (BillingRequestBuilder()..update(updates))._build();

  _$BillingRequest._(
      {this.projectId, this.kind, this.description, this.amount, this.dueDate})
      : super._();
  @override
  BillingRequest rebuild(void Function(BillingRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingRequestBuilder toBuilder() => BillingRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingRequest &&
        projectId == other.projectId &&
        kind == other.kind &&
        description == other.description &&
        amount == other.amount &&
        dueDate == other.dueDate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, dueDate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BillingRequest')
          ..add('projectId', projectId)
          ..add('kind', kind)
          ..add('description', description)
          ..add('amount', amount)
          ..add('dueDate', dueDate))
        .toString();
  }
}

class BillingRequestBuilder
    implements Builder<BillingRequest, BillingRequestBuilder> {
  _$BillingRequest? _$v;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  Date? _dueDate;
  Date? get dueDate => _$this._dueDate;
  set dueDate(Date? dueDate) => _$this._dueDate = dueDate;

  BillingRequestBuilder() {
    BillingRequest._defaults(this);
  }

  BillingRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _projectId = $v.projectId;
      _kind = $v.kind;
      _description = $v.description;
      _amount = $v.amount;
      _dueDate = $v.dueDate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingRequest other) {
    _$v = other as _$BillingRequest;
  }

  @override
  void update(void Function(BillingRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingRequest build() => _build();

  _$BillingRequest _build() {
    final _$result = _$v ??
        _$BillingRequest._(
          projectId: projectId,
          kind: kind,
          description: description,
          amount: amount,
          dueDate: dueDate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
