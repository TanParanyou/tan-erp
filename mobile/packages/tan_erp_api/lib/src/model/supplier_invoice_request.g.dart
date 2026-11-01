// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceRequest extends SupplierInvoiceRequest {
  @override
  final String? purchaseOrderId;
  @override
  final String? supplierInvoiceNumber;
  @override
  final Date? invoiceDate;
  @override
  final Date? dueDate;
  @override
  final double? taxAmount;
  @override
  final String? note;
  @override
  final BuiltList<SupplierInvoiceLineRequest>? lines;

  factory _$SupplierInvoiceRequest(
          [void Function(SupplierInvoiceRequestBuilder)? updates]) =>
      (SupplierInvoiceRequestBuilder()..update(updates))._build();

  _$SupplierInvoiceRequest._(
      {this.purchaseOrderId,
      this.supplierInvoiceNumber,
      this.invoiceDate,
      this.dueDate,
      this.taxAmount,
      this.note,
      this.lines})
      : super._();
  @override
  SupplierInvoiceRequest rebuild(
          void Function(SupplierInvoiceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceRequestBuilder toBuilder() =>
      SupplierInvoiceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceRequest &&
        purchaseOrderId == other.purchaseOrderId &&
        supplierInvoiceNumber == other.supplierInvoiceNumber &&
        invoiceDate == other.invoiceDate &&
        dueDate == other.dueDate &&
        taxAmount == other.taxAmount &&
        note == other.note &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, purchaseOrderId.hashCode);
    _$hash = $jc(_$hash, supplierInvoiceNumber.hashCode);
    _$hash = $jc(_$hash, invoiceDate.hashCode);
    _$hash = $jc(_$hash, dueDate.hashCode);
    _$hash = $jc(_$hash, taxAmount.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierInvoiceRequest')
          ..add('purchaseOrderId', purchaseOrderId)
          ..add('supplierInvoiceNumber', supplierInvoiceNumber)
          ..add('invoiceDate', invoiceDate)
          ..add('dueDate', dueDate)
          ..add('taxAmount', taxAmount)
          ..add('note', note)
          ..add('lines', lines))
        .toString();
  }
}

class SupplierInvoiceRequestBuilder
    implements Builder<SupplierInvoiceRequest, SupplierInvoiceRequestBuilder> {
  _$SupplierInvoiceRequest? _$v;

  String? _purchaseOrderId;
  String? get purchaseOrderId => _$this._purchaseOrderId;
  set purchaseOrderId(String? purchaseOrderId) =>
      _$this._purchaseOrderId = purchaseOrderId;

  String? _supplierInvoiceNumber;
  String? get supplierInvoiceNumber => _$this._supplierInvoiceNumber;
  set supplierInvoiceNumber(String? supplierInvoiceNumber) =>
      _$this._supplierInvoiceNumber = supplierInvoiceNumber;

  Date? _invoiceDate;
  Date? get invoiceDate => _$this._invoiceDate;
  set invoiceDate(Date? invoiceDate) => _$this._invoiceDate = invoiceDate;

  Date? _dueDate;
  Date? get dueDate => _$this._dueDate;
  set dueDate(Date? dueDate) => _$this._dueDate = dueDate;

  double? _taxAmount;
  double? get taxAmount => _$this._taxAmount;
  set taxAmount(double? taxAmount) => _$this._taxAmount = taxAmount;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ListBuilder<SupplierInvoiceLineRequest>? _lines;
  ListBuilder<SupplierInvoiceLineRequest> get lines =>
      _$this._lines ??= ListBuilder<SupplierInvoiceLineRequest>();
  set lines(ListBuilder<SupplierInvoiceLineRequest>? lines) =>
      _$this._lines = lines;

  SupplierInvoiceRequestBuilder() {
    SupplierInvoiceRequest._defaults(this);
  }

  SupplierInvoiceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _purchaseOrderId = $v.purchaseOrderId;
      _supplierInvoiceNumber = $v.supplierInvoiceNumber;
      _invoiceDate = $v.invoiceDate;
      _dueDate = $v.dueDate;
      _taxAmount = $v.taxAmount;
      _note = $v.note;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierInvoiceRequest other) {
    _$v = other as _$SupplierInvoiceRequest;
  }

  @override
  void update(void Function(SupplierInvoiceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceRequest build() => _build();

  _$SupplierInvoiceRequest _build() {
    _$SupplierInvoiceRequest _$result;
    try {
      _$result = _$v ??
          _$SupplierInvoiceRequest._(
            purchaseOrderId: purchaseOrderId,
            supplierInvoiceNumber: supplierInvoiceNumber,
            invoiceDate: invoiceDate,
            dueDate: dueDate,
            taxAmount: taxAmount,
            note: note,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SupplierInvoiceRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
