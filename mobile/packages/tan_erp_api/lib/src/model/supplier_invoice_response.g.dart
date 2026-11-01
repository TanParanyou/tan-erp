// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceResponse extends SupplierInvoiceResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final String? supplierInvoiceNumber;
  @override
  final String? status;
  @override
  final String? matchStatus;
  @override
  final String? currency;
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
  final double? totalPoAmount;
  @override
  final double? totalVarianceAmount;
  @override
  final String? note;
  @override
  final SupplierInvoiceOrderRefResponse? purchaseOrder;
  @override
  final PurchaseOrderSupplierResponse? supplier;
  @override
  final PurchaseOrderProjectResponse? project;
  @override
  final ProcurementPersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? submittedAtUtc;
  @override
  final DateTime? postedAtUtc;
  @override
  final ProcurementPersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final String? decisionNote;
  @override
  final String? cancelReason;
  @override
  final String? voidReason;
  @override
  final ProcurementPersonResponse? voidedBy;
  @override
  final DateTime? voidedAtUtc;
  @override
  final String? rowVersion;
  @override
  final BuiltList<SupplierInvoiceLineResponse>? lines;

  factory _$SupplierInvoiceResponse(
          [void Function(SupplierInvoiceResponseBuilder)? updates]) =>
      (SupplierInvoiceResponseBuilder()..update(updates))._build();

  _$SupplierInvoiceResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.supplierInvoiceNumber,
      this.status,
      this.matchStatus,
      this.currency,
      this.invoiceDate,
      this.dueDate,
      this.subtotalAmount,
      this.taxAmount,
      this.totalAmount,
      this.totalPoAmount,
      this.totalVarianceAmount,
      this.note,
      this.purchaseOrder,
      this.supplier,
      this.project,
      this.createdBy,
      this.createdAtUtc,
      this.submittedAtUtc,
      this.postedAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.decisionNote,
      this.cancelReason,
      this.voidReason,
      this.voidedBy,
      this.voidedAtUtc,
      this.rowVersion,
      this.lines})
      : super._();
  @override
  SupplierInvoiceResponse rebuild(
          void Function(SupplierInvoiceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceResponseBuilder toBuilder() =>
      SupplierInvoiceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        supplierInvoiceNumber == other.supplierInvoiceNumber &&
        status == other.status &&
        matchStatus == other.matchStatus &&
        currency == other.currency &&
        invoiceDate == other.invoiceDate &&
        dueDate == other.dueDate &&
        subtotalAmount == other.subtotalAmount &&
        taxAmount == other.taxAmount &&
        totalAmount == other.totalAmount &&
        totalPoAmount == other.totalPoAmount &&
        totalVarianceAmount == other.totalVarianceAmount &&
        note == other.note &&
        purchaseOrder == other.purchaseOrder &&
        supplier == other.supplier &&
        project == other.project &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        submittedAtUtc == other.submittedAtUtc &&
        postedAtUtc == other.postedAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        decisionNote == other.decisionNote &&
        cancelReason == other.cancelReason &&
        voidReason == other.voidReason &&
        voidedBy == other.voidedBy &&
        voidedAtUtc == other.voidedAtUtc &&
        rowVersion == other.rowVersion &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, supplierInvoiceNumber.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, matchStatus.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, invoiceDate.hashCode);
    _$hash = $jc(_$hash, dueDate.hashCode);
    _$hash = $jc(_$hash, subtotalAmount.hashCode);
    _$hash = $jc(_$hash, taxAmount.hashCode);
    _$hash = $jc(_$hash, totalAmount.hashCode);
    _$hash = $jc(_$hash, totalPoAmount.hashCode);
    _$hash = $jc(_$hash, totalVarianceAmount.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, purchaseOrder.hashCode);
    _$hash = $jc(_$hash, supplier.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, submittedAtUtc.hashCode);
    _$hash = $jc(_$hash, postedAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jc(_$hash, cancelReason.hashCode);
    _$hash = $jc(_$hash, voidReason.hashCode);
    _$hash = $jc(_$hash, voidedBy.hashCode);
    _$hash = $jc(_$hash, voidedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierInvoiceResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('supplierInvoiceNumber', supplierInvoiceNumber)
          ..add('status', status)
          ..add('matchStatus', matchStatus)
          ..add('currency', currency)
          ..add('invoiceDate', invoiceDate)
          ..add('dueDate', dueDate)
          ..add('subtotalAmount', subtotalAmount)
          ..add('taxAmount', taxAmount)
          ..add('totalAmount', totalAmount)
          ..add('totalPoAmount', totalPoAmount)
          ..add('totalVarianceAmount', totalVarianceAmount)
          ..add('note', note)
          ..add('purchaseOrder', purchaseOrder)
          ..add('supplier', supplier)
          ..add('project', project)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('submittedAtUtc', submittedAtUtc)
          ..add('postedAtUtc', postedAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('decisionNote', decisionNote)
          ..add('cancelReason', cancelReason)
          ..add('voidReason', voidReason)
          ..add('voidedBy', voidedBy)
          ..add('voidedAtUtc', voidedAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('lines', lines))
        .toString();
  }
}

class SupplierInvoiceResponseBuilder
    implements
        Builder<SupplierInvoiceResponse, SupplierInvoiceResponseBuilder> {
  _$SupplierInvoiceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

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

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

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

  double? _totalPoAmount;
  double? get totalPoAmount => _$this._totalPoAmount;
  set totalPoAmount(double? totalPoAmount) =>
      _$this._totalPoAmount = totalPoAmount;

  double? _totalVarianceAmount;
  double? get totalVarianceAmount => _$this._totalVarianceAmount;
  set totalVarianceAmount(double? totalVarianceAmount) =>
      _$this._totalVarianceAmount = totalVarianceAmount;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  SupplierInvoiceOrderRefResponseBuilder? _purchaseOrder;
  SupplierInvoiceOrderRefResponseBuilder get purchaseOrder =>
      _$this._purchaseOrder ??= SupplierInvoiceOrderRefResponseBuilder();
  set purchaseOrder(SupplierInvoiceOrderRefResponseBuilder? purchaseOrder) =>
      _$this._purchaseOrder = purchaseOrder;

  PurchaseOrderSupplierResponseBuilder? _supplier;
  PurchaseOrderSupplierResponseBuilder get supplier =>
      _$this._supplier ??= PurchaseOrderSupplierResponseBuilder();
  set supplier(PurchaseOrderSupplierResponseBuilder? supplier) =>
      _$this._supplier = supplier;

  PurchaseOrderProjectResponseBuilder? _project;
  PurchaseOrderProjectResponseBuilder get project =>
      _$this._project ??= PurchaseOrderProjectResponseBuilder();
  set project(PurchaseOrderProjectResponseBuilder? project) =>
      _$this._project = project;

  ProcurementPersonResponseBuilder? _createdBy;
  ProcurementPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ProcurementPersonResponseBuilder();
  set createdBy(ProcurementPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _submittedAtUtc;
  DateTime? get submittedAtUtc => _$this._submittedAtUtc;
  set submittedAtUtc(DateTime? submittedAtUtc) =>
      _$this._submittedAtUtc = submittedAtUtc;

  DateTime? _postedAtUtc;
  DateTime? get postedAtUtc => _$this._postedAtUtc;
  set postedAtUtc(DateTime? postedAtUtc) => _$this._postedAtUtc = postedAtUtc;

  ProcurementPersonResponseBuilder? _decidedBy;
  ProcurementPersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= ProcurementPersonResponseBuilder();
  set decidedBy(ProcurementPersonResponseBuilder? decidedBy) =>
      _$this._decidedBy = decidedBy;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  String? _decisionNote;
  String? get decisionNote => _$this._decisionNote;
  set decisionNote(String? decisionNote) => _$this._decisionNote = decisionNote;

  String? _cancelReason;
  String? get cancelReason => _$this._cancelReason;
  set cancelReason(String? cancelReason) => _$this._cancelReason = cancelReason;

  String? _voidReason;
  String? get voidReason => _$this._voidReason;
  set voidReason(String? voidReason) => _$this._voidReason = voidReason;

  ProcurementPersonResponseBuilder? _voidedBy;
  ProcurementPersonResponseBuilder get voidedBy =>
      _$this._voidedBy ??= ProcurementPersonResponseBuilder();
  set voidedBy(ProcurementPersonResponseBuilder? voidedBy) =>
      _$this._voidedBy = voidedBy;

  DateTime? _voidedAtUtc;
  DateTime? get voidedAtUtc => _$this._voidedAtUtc;
  set voidedAtUtc(DateTime? voidedAtUtc) => _$this._voidedAtUtc = voidedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<SupplierInvoiceLineResponse>? _lines;
  ListBuilder<SupplierInvoiceLineResponse> get lines =>
      _$this._lines ??= ListBuilder<SupplierInvoiceLineResponse>();
  set lines(ListBuilder<SupplierInvoiceLineResponse>? lines) =>
      _$this._lines = lines;

  SupplierInvoiceResponseBuilder() {
    SupplierInvoiceResponse._defaults(this);
  }

  SupplierInvoiceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _supplierInvoiceNumber = $v.supplierInvoiceNumber;
      _status = $v.status;
      _matchStatus = $v.matchStatus;
      _currency = $v.currency;
      _invoiceDate = $v.invoiceDate;
      _dueDate = $v.dueDate;
      _subtotalAmount = $v.subtotalAmount;
      _taxAmount = $v.taxAmount;
      _totalAmount = $v.totalAmount;
      _totalPoAmount = $v.totalPoAmount;
      _totalVarianceAmount = $v.totalVarianceAmount;
      _note = $v.note;
      _purchaseOrder = $v.purchaseOrder?.toBuilder();
      _supplier = $v.supplier?.toBuilder();
      _project = $v.project?.toBuilder();
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _submittedAtUtc = $v.submittedAtUtc;
      _postedAtUtc = $v.postedAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _decisionNote = $v.decisionNote;
      _cancelReason = $v.cancelReason;
      _voidReason = $v.voidReason;
      _voidedBy = $v.voidedBy?.toBuilder();
      _voidedAtUtc = $v.voidedAtUtc;
      _rowVersion = $v.rowVersion;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierInvoiceResponse other) {
    _$v = other as _$SupplierInvoiceResponse;
  }

  @override
  void update(void Function(SupplierInvoiceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceResponse build() => _build();

  _$SupplierInvoiceResponse _build() {
    _$SupplierInvoiceResponse _$result;
    try {
      _$result = _$v ??
          _$SupplierInvoiceResponse._(
            id: id,
            branchId: branchId,
            number: number,
            supplierInvoiceNumber: supplierInvoiceNumber,
            status: status,
            matchStatus: matchStatus,
            currency: currency,
            invoiceDate: invoiceDate,
            dueDate: dueDate,
            subtotalAmount: subtotalAmount,
            taxAmount: taxAmount,
            totalAmount: totalAmount,
            totalPoAmount: totalPoAmount,
            totalVarianceAmount: totalVarianceAmount,
            note: note,
            purchaseOrder: _purchaseOrder?.build(),
            supplier: _supplier?.build(),
            project: _project?.build(),
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            submittedAtUtc: submittedAtUtc,
            postedAtUtc: postedAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            decisionNote: decisionNote,
            cancelReason: cancelReason,
            voidReason: voidReason,
            voidedBy: _voidedBy?.build(),
            voidedAtUtc: voidedAtUtc,
            rowVersion: rowVersion,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'purchaseOrder';
        _purchaseOrder?.build();
        _$failedField = 'supplier';
        _supplier?.build();
        _$failedField = 'project';
        _project?.build();
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();

        _$failedField = 'voidedBy';
        _voidedBy?.build();

        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SupplierInvoiceResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
