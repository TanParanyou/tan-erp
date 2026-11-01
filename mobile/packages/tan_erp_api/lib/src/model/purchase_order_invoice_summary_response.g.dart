// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_invoice_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderInvoiceSummaryResponse
    extends PurchaseOrderInvoiceSummaryResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? supplierInvoiceNumber;
  @override
  final String? status;
  @override
  final String? matchStatus;
  @override
  final Date? invoiceDate;
  @override
  final double? subtotalAmount;
  @override
  final double? taxAmount;
  @override
  final double? totalAmount;
  @override
  final double? totalVarianceAmount;
  @override
  final DateTime? createdAtUtc;

  factory _$PurchaseOrderInvoiceSummaryResponse(
          [void Function(PurchaseOrderInvoiceSummaryResponseBuilder)?
              updates]) =>
      (PurchaseOrderInvoiceSummaryResponseBuilder()..update(updates))._build();

  _$PurchaseOrderInvoiceSummaryResponse._(
      {this.id,
      this.number,
      this.supplierInvoiceNumber,
      this.status,
      this.matchStatus,
      this.invoiceDate,
      this.subtotalAmount,
      this.taxAmount,
      this.totalAmount,
      this.totalVarianceAmount,
      this.createdAtUtc})
      : super._();
  @override
  PurchaseOrderInvoiceSummaryResponse rebuild(
          void Function(PurchaseOrderInvoiceSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderInvoiceSummaryResponseBuilder toBuilder() =>
      PurchaseOrderInvoiceSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderInvoiceSummaryResponse &&
        id == other.id &&
        number == other.number &&
        supplierInvoiceNumber == other.supplierInvoiceNumber &&
        status == other.status &&
        matchStatus == other.matchStatus &&
        invoiceDate == other.invoiceDate &&
        subtotalAmount == other.subtotalAmount &&
        taxAmount == other.taxAmount &&
        totalAmount == other.totalAmount &&
        totalVarianceAmount == other.totalVarianceAmount &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, supplierInvoiceNumber.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, matchStatus.hashCode);
    _$hash = $jc(_$hash, invoiceDate.hashCode);
    _$hash = $jc(_$hash, subtotalAmount.hashCode);
    _$hash = $jc(_$hash, taxAmount.hashCode);
    _$hash = $jc(_$hash, totalAmount.hashCode);
    _$hash = $jc(_$hash, totalVarianceAmount.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderInvoiceSummaryResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('supplierInvoiceNumber', supplierInvoiceNumber)
          ..add('status', status)
          ..add('matchStatus', matchStatus)
          ..add('invoiceDate', invoiceDate)
          ..add('subtotalAmount', subtotalAmount)
          ..add('taxAmount', taxAmount)
          ..add('totalAmount', totalAmount)
          ..add('totalVarianceAmount', totalVarianceAmount)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class PurchaseOrderInvoiceSummaryResponseBuilder
    implements
        Builder<PurchaseOrderInvoiceSummaryResponse,
            PurchaseOrderInvoiceSummaryResponseBuilder> {
  _$PurchaseOrderInvoiceSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _supplierInvoiceNumber;
  String? get supplierInvoiceNumber => _$this._supplierInvoiceNumber;
  set supplierInvoiceNumber(String? supplierInvoiceNumber) =>
      _$this._supplierInvoiceNumber = supplierInvoiceNumber;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _matchStatus;
  String? get matchStatus => _$this._matchStatus;
  set matchStatus(String? matchStatus) => _$this._matchStatus = matchStatus;

  Date? _invoiceDate;
  Date? get invoiceDate => _$this._invoiceDate;
  set invoiceDate(Date? invoiceDate) => _$this._invoiceDate = invoiceDate;

  double? _subtotalAmount;
  double? get subtotalAmount => _$this._subtotalAmount;
  set subtotalAmount(double? subtotalAmount) =>
      _$this._subtotalAmount = subtotalAmount;

  double? _taxAmount;
  double? get taxAmount => _$this._taxAmount;
  set taxAmount(double? taxAmount) => _$this._taxAmount = taxAmount;

  double? _totalAmount;
  double? get totalAmount => _$this._totalAmount;
  set totalAmount(double? totalAmount) => _$this._totalAmount = totalAmount;

  double? _totalVarianceAmount;
  double? get totalVarianceAmount => _$this._totalVarianceAmount;
  set totalVarianceAmount(double? totalVarianceAmount) =>
      _$this._totalVarianceAmount = totalVarianceAmount;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  PurchaseOrderInvoiceSummaryResponseBuilder() {
    PurchaseOrderInvoiceSummaryResponse._defaults(this);
  }

  PurchaseOrderInvoiceSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _supplierInvoiceNumber = $v.supplierInvoiceNumber;
      _status = $v.status;
      _matchStatus = $v.matchStatus;
      _invoiceDate = $v.invoiceDate;
      _subtotalAmount = $v.subtotalAmount;
      _taxAmount = $v.taxAmount;
      _totalAmount = $v.totalAmount;
      _totalVarianceAmount = $v.totalVarianceAmount;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderInvoiceSummaryResponse other) {
    _$v = other as _$PurchaseOrderInvoiceSummaryResponse;
  }

  @override
  void update(
      void Function(PurchaseOrderInvoiceSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderInvoiceSummaryResponse build() => _build();

  _$PurchaseOrderInvoiceSummaryResponse _build() {
    final _$result = _$v ??
        _$PurchaseOrderInvoiceSummaryResponse._(
          id: id,
          number: number,
          supplierInvoiceNumber: supplierInvoiceNumber,
          status: status,
          matchStatus: matchStatus,
          invoiceDate: invoiceDate,
          subtotalAmount: subtotalAmount,
          taxAmount: taxAmount,
          totalAmount: totalAmount,
          totalVarianceAmount: totalVarianceAmount,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
