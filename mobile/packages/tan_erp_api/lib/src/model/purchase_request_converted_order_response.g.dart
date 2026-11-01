// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_converted_order_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestConvertedOrderResponse
    extends PurchaseRequestConvertedOrderResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? status;

  factory _$PurchaseRequestConvertedOrderResponse(
          [void Function(PurchaseRequestConvertedOrderResponseBuilder)?
              updates]) =>
      (PurchaseRequestConvertedOrderResponseBuilder()..update(updates))
          ._build();

  _$PurchaseRequestConvertedOrderResponse._({this.id, this.number, this.status})
      : super._();
  @override
  PurchaseRequestConvertedOrderResponse rebuild(
          void Function(PurchaseRequestConvertedOrderResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestConvertedOrderResponseBuilder toBuilder() =>
      PurchaseRequestConvertedOrderResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestConvertedOrderResponse &&
        id == other.id &&
        number == other.number &&
        status == other.status;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(
            r'PurchaseRequestConvertedOrderResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('status', status))
        .toString();
  }
}

class PurchaseRequestConvertedOrderResponseBuilder
    implements
        Builder<PurchaseRequestConvertedOrderResponse,
            PurchaseRequestConvertedOrderResponseBuilder> {
  _$PurchaseRequestConvertedOrderResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  PurchaseRequestConvertedOrderResponseBuilder() {
    PurchaseRequestConvertedOrderResponse._defaults(this);
  }

  PurchaseRequestConvertedOrderResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _status = $v.status;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestConvertedOrderResponse other) {
    _$v = other as _$PurchaseRequestConvertedOrderResponse;
  }

  @override
  void update(
      void Function(PurchaseRequestConvertedOrderResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestConvertedOrderResponse build() => _build();

  _$PurchaseRequestConvertedOrderResponse _build() {
    final _$result = _$v ??
        _$PurchaseRequestConvertedOrderResponse._(
          id: id,
          number: number,
          status: status,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
