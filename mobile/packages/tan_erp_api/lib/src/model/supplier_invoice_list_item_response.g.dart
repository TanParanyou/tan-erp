// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceListItemResponse
    extends SupplierInvoiceListItemResponse {
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
  final String? purchaseOrderId;
  @override
  final String? purchaseOrderNumber;
  @override
  final String? supplierCode;
  @override
  final String? supplierNameTh;
  @override
  final String? projectCode;
  @override
  final Date? invoiceDate;
  @override
  final Date? dueDate;
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

  factory _$SupplierInvoiceListItemResponse(
          [void Function(SupplierInvoiceListItemResponseBuilder)? updates]) =>
      (SupplierInvoiceListItemResponseBuilder()..update(updates))._build();

  _$SupplierInvoiceListItemResponse._(
      {this.id,
      this.number,
      this.supplierInvoiceNumber,
      this.status,
      this.matchStatus,
      this.purchaseOrderId,
      this.purchaseOrderNumber,
      this.supplierCode,
      this.supplierNameTh,
      this.projectCode,
      this.invoiceDate,
      this.dueDate,
      this.subtotalAmount,
      this.taxAmount,
      this.totalAmount,
      this.totalVarianceAmount,
      this.createdAtUtc})
      : super._();
  @override
  SupplierInvoiceListItemResponse rebuild(
          void Function(SupplierInvoiceListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceListItemResponseBuilder toBuilder() =>
      SupplierInvoiceListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceListItemResponse &&
        id == other.id &&
        number == other.number &&
        supplierInvoiceNumber == other.supplierInvoiceNumber &&
        status == other.status &&
        matchStatus == other.matchStatus &&
        purchaseOrderId == other.purchaseOrderId &&
        purchaseOrderNumber == other.purchaseOrderNumber &&
        supplierCode == other.supplierCode &&
        supplierNameTh == other.supplierNameTh &&
        projectCode == other.projectCode &&
        invoiceDate == other.invoiceDate &&
        dueDate == other.dueDate &&
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
    _$hash = $jc(_$hash, purchaseOrderId.hashCode);
    _$hash = $jc(_$hash, purchaseOrderNumber.hashCode);
    _$hash = $jc(_$hash, supplierCode.hashCode);
    _$hash = $jc(_$hash, supplierNameTh.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, invoiceDate.hashCode);
    _$hash = $jc(_$hash, dueDate.hashCode);
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
    return (newBuiltValueToStringHelper(r'SupplierInvoiceListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('supplierInvoiceNumber', supplierInvoiceNumber)
          ..add('status', status)
          ..add('matchStatus', matchStatus)
          ..add('purchaseOrderId', purchaseOrderId)
          ..add('purchaseOrderNumber', purchaseOrderNumber)
          ..add('supplierCode', supplierCode)
          ..add('supplierNameTh', supplierNameTh)
          ..add('projectCode', projectCode)
          ..add('invoiceDate', invoiceDate)
          ..add('dueDate', dueDate)
          ..add('subtotalAmount', subtotalAmount)
          ..add('taxAmount', taxAmount)
          ..add('totalAmount', totalAmount)
          ..add('totalVarianceAmount', totalVarianceAmount)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class SupplierInvoiceListItemResponseBuilder
    implements
        Builder<SupplierInvoiceListItemResponse,
            SupplierInvoiceListItemResponseBuilder> {
  _$SupplierInvoiceListItemResponse? _$v;

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

  String? _purchaseOrderId;
  String? get purchaseOrderId => _$this._purchaseOrderId;
  set purchaseOrderId(String? purchaseOrderId) =>
      _$this._purchaseOrderId = purchaseOrderId;

  String? _purchaseOrderNumber;
  String? get purchaseOrderNumber => _$this._purchaseOrderNumber;
  set purchaseOrderNumber(String? purchaseOrderNumber) =>
      _$this._purchaseOrderNumber = purchaseOrderNumber;

  String? _supplierCode;
  String? get supplierCode => _$this._supplierCode;
  set supplierCode(String? supplierCode) => _$this._supplierCode = supplierCode;

  String? _supplierNameTh;
  String? get supplierNameTh => _$this._supplierNameTh;
  set supplierNameTh(String? supplierNameTh) =>
      _$this._supplierNameTh = supplierNameTh;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  Date? _invoiceDate;
  Date? get invoiceDate => _$this._invoiceDate;
  set invoiceDate(Date? invoiceDate) => _$this._invoiceDate = invoiceDate;

  Date? _dueDate;
  Date? get dueDate => _$this._dueDate;
  set dueDate(Date? dueDate) => _$this._dueDate = dueDate;

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

  SupplierInvoiceListItemResponseBuilder() {
    SupplierInvoiceListItemResponse._defaults(this);
  }

  SupplierInvoiceListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _supplierInvoiceNumber = $v.supplierInvoiceNumber;
      _status = $v.status;
      _matchStatus = $v.matchStatus;
      _purchaseOrderId = $v.purchaseOrderId;
      _purchaseOrderNumber = $v.purchaseOrderNumber;
      _supplierCode = $v.supplierCode;
      _supplierNameTh = $v.supplierNameTh;
      _projectCode = $v.projectCode;
      _invoiceDate = $v.invoiceDate;
      _dueDate = $v.dueDate;
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
  void replace(SupplierInvoiceListItemResponse other) {
    _$v = other as _$SupplierInvoiceListItemResponse;
  }

  @override
  void update(void Function(SupplierInvoiceListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceListItemResponse build() => _build();

  _$SupplierInvoiceListItemResponse _build() {
    final _$result = _$v ??
        _$SupplierInvoiceListItemResponse._(
          id: id,
          number: number,
          supplierInvoiceNumber: supplierInvoiceNumber,
          status: status,
          matchStatus: matchStatus,
          purchaseOrderId: purchaseOrderId,
          purchaseOrderNumber: purchaseOrderNumber,
          supplierCode: supplierCode,
          supplierNameTh: supplierNameTh,
          projectCode: projectCode,
          invoiceDate: invoiceDate,
          dueDate: dueDate,
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
