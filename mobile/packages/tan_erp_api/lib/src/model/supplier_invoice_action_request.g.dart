// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_action_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceActionRequest extends SupplierInvoiceActionRequest {
  @override
  final String? note;

  factory _$SupplierInvoiceActionRequest(
          [void Function(SupplierInvoiceActionRequestBuilder)? updates]) =>
      (SupplierInvoiceActionRequestBuilder()..update(updates))._build();

  _$SupplierInvoiceActionRequest._({this.note}) : super._();
  @override
  SupplierInvoiceActionRequest rebuild(
          void Function(SupplierInvoiceActionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceActionRequestBuilder toBuilder() =>
      SupplierInvoiceActionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceActionRequest && note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierInvoiceActionRequest')
          ..add('note', note))
        .toString();
  }
}

class SupplierInvoiceActionRequestBuilder
    implements
        Builder<SupplierInvoiceActionRequest,
            SupplierInvoiceActionRequestBuilder> {
  _$SupplierInvoiceActionRequest? _$v;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  SupplierInvoiceActionRequestBuilder() {
    SupplierInvoiceActionRequest._defaults(this);
  }

  SupplierInvoiceActionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierInvoiceActionRequest other) {
    _$v = other as _$SupplierInvoiceActionRequest;
  }

  @override
  void update(void Function(SupplierInvoiceActionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceActionRequest build() => _build();

  _$SupplierInvoiceActionRequest _build() {
    final _$result = _$v ??
        _$SupplierInvoiceActionRequest._(
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
