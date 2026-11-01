// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_order_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceOrderRefResponse
    extends SupplierInvoiceOrderRefResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? status;

  factory _$SupplierInvoiceOrderRefResponse(
          [void Function(SupplierInvoiceOrderRefResponseBuilder)? updates]) =>
      (SupplierInvoiceOrderRefResponseBuilder()..update(updates))._build();

  _$SupplierInvoiceOrderRefResponse._({this.id, this.number, this.status})
      : super._();
  @override
  SupplierInvoiceOrderRefResponse rebuild(
          void Function(SupplierInvoiceOrderRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceOrderRefResponseBuilder toBuilder() =>
      SupplierInvoiceOrderRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceOrderRefResponse &&
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
    return (newBuiltValueToStringHelper(r'SupplierInvoiceOrderRefResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('status', status))
        .toString();
  }
}

class SupplierInvoiceOrderRefResponseBuilder
    implements
        Builder<SupplierInvoiceOrderRefResponse,
            SupplierInvoiceOrderRefResponseBuilder> {
  _$SupplierInvoiceOrderRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  SupplierInvoiceOrderRefResponseBuilder() {
    SupplierInvoiceOrderRefResponse._defaults(this);
  }

  SupplierInvoiceOrderRefResponseBuilder get _$this {
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
  void replace(SupplierInvoiceOrderRefResponse other) {
    _$v = other as _$SupplierInvoiceOrderRefResponse;
  }

  @override
  void update(void Function(SupplierInvoiceOrderRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceOrderRefResponse build() => _build();

  _$SupplierInvoiceOrderRefResponse _build() {
    final _$result = _$v ??
        _$SupplierInvoiceOrderRefResponse._(
          id: id,
          number: number,
          status: status,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
