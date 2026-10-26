// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_transaction_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderTransactionResponse extends WorkOrderTransactionResponse {
  @override
  final String? id;
  @override
  final String? kind;
  @override
  final String? stockDocumentId;
  @override
  final String? stockDocumentNumber;
  @override
  final double? quantity;
  @override
  final double? value;
  @override
  final ProductionPersonResponse? actor;
  @override
  final DateTime? createdAtUtc;

  factory _$WorkOrderTransactionResponse(
          [void Function(WorkOrderTransactionResponseBuilder)? updates]) =>
      (WorkOrderTransactionResponseBuilder()..update(updates))._build();

  _$WorkOrderTransactionResponse._(
      {this.id,
      this.kind,
      this.stockDocumentId,
      this.stockDocumentNumber,
      this.quantity,
      this.value,
      this.actor,
      this.createdAtUtc})
      : super._();
  @override
  WorkOrderTransactionResponse rebuild(
          void Function(WorkOrderTransactionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderTransactionResponseBuilder toBuilder() =>
      WorkOrderTransactionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderTransactionResponse &&
        id == other.id &&
        kind == other.kind &&
        stockDocumentId == other.stockDocumentId &&
        stockDocumentNumber == other.stockDocumentNumber &&
        quantity == other.quantity &&
        value == other.value &&
        actor == other.actor &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, stockDocumentId.hashCode);
    _$hash = $jc(_$hash, stockDocumentNumber.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, value.hashCode);
    _$hash = $jc(_$hash, actor.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderTransactionResponse')
          ..add('id', id)
          ..add('kind', kind)
          ..add('stockDocumentId', stockDocumentId)
          ..add('stockDocumentNumber', stockDocumentNumber)
          ..add('quantity', quantity)
          ..add('value', value)
          ..add('actor', actor)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class WorkOrderTransactionResponseBuilder
    implements
        Builder<WorkOrderTransactionResponse,
            WorkOrderTransactionResponseBuilder> {
  _$WorkOrderTransactionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _stockDocumentId;
  String? get stockDocumentId => _$this._stockDocumentId;
  set stockDocumentId(String? stockDocumentId) =>
      _$this._stockDocumentId = stockDocumentId;

  String? _stockDocumentNumber;
  String? get stockDocumentNumber => _$this._stockDocumentNumber;
  set stockDocumentNumber(String? stockDocumentNumber) =>
      _$this._stockDocumentNumber = stockDocumentNumber;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _value;
  double? get value => _$this._value;
  set value(double? value) => _$this._value = value;

  ProductionPersonResponseBuilder? _actor;
  ProductionPersonResponseBuilder get actor =>
      _$this._actor ??= ProductionPersonResponseBuilder();
  set actor(ProductionPersonResponseBuilder? actor) => _$this._actor = actor;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  WorkOrderTransactionResponseBuilder() {
    WorkOrderTransactionResponse._defaults(this);
  }

  WorkOrderTransactionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _kind = $v.kind;
      _stockDocumentId = $v.stockDocumentId;
      _stockDocumentNumber = $v.stockDocumentNumber;
      _quantity = $v.quantity;
      _value = $v.value;
      _actor = $v.actor?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderTransactionResponse other) {
    _$v = other as _$WorkOrderTransactionResponse;
  }

  @override
  void update(void Function(WorkOrderTransactionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderTransactionResponse build() => _build();

  _$WorkOrderTransactionResponse _build() {
    _$WorkOrderTransactionResponse _$result;
    try {
      _$result = _$v ??
          _$WorkOrderTransactionResponse._(
            id: id,
            kind: kind,
            stockDocumentId: stockDocumentId,
            stockDocumentNumber: stockDocumentNumber,
            quantity: quantity,
            value: value,
            actor: _actor?.build(),
            createdAtUtc: createdAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'actor';
        _actor?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkOrderTransactionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
