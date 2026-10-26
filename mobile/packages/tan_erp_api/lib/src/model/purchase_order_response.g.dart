// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderResponse extends PurchaseOrderResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final String? currency;
  @override
  final double? totalAmount;
  @override
  final Date? expectedDeliveryDate;
  @override
  final String? note;
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
  final ProcurementPersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final String? decisionNote;
  @override
  final String? cancelReason;
  @override
  final String? rowVersion;
  @override
  final BuiltList<PurchaseOrderLineResponse>? lines;
  @override
  final BuiltList<GoodsReceiptSummaryResponse>? receipts;

  factory _$PurchaseOrderResponse(
          [void Function(PurchaseOrderResponseBuilder)? updates]) =>
      (PurchaseOrderResponseBuilder()..update(updates))._build();

  _$PurchaseOrderResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.status,
      this.currency,
      this.totalAmount,
      this.expectedDeliveryDate,
      this.note,
      this.supplier,
      this.project,
      this.createdBy,
      this.createdAtUtc,
      this.submittedAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.decisionNote,
      this.cancelReason,
      this.rowVersion,
      this.lines,
      this.receipts})
      : super._();
  @override
  PurchaseOrderResponse rebuild(
          void Function(PurchaseOrderResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderResponseBuilder toBuilder() =>
      PurchaseOrderResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        status == other.status &&
        currency == other.currency &&
        totalAmount == other.totalAmount &&
        expectedDeliveryDate == other.expectedDeliveryDate &&
        note == other.note &&
        supplier == other.supplier &&
        project == other.project &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        submittedAtUtc == other.submittedAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        decisionNote == other.decisionNote &&
        cancelReason == other.cancelReason &&
        rowVersion == other.rowVersion &&
        lines == other.lines &&
        receipts == other.receipts;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, totalAmount.hashCode);
    _$hash = $jc(_$hash, expectedDeliveryDate.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, supplier.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, submittedAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jc(_$hash, cancelReason.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jc(_$hash, receipts.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('status', status)
          ..add('currency', currency)
          ..add('totalAmount', totalAmount)
          ..add('expectedDeliveryDate', expectedDeliveryDate)
          ..add('note', note)
          ..add('supplier', supplier)
          ..add('project', project)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('submittedAtUtc', submittedAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('decisionNote', decisionNote)
          ..add('cancelReason', cancelReason)
          ..add('rowVersion', rowVersion)
          ..add('lines', lines)
          ..add('receipts', receipts))
        .toString();
  }
}

class PurchaseOrderResponseBuilder
    implements Builder<PurchaseOrderResponse, PurchaseOrderResponseBuilder> {
  _$PurchaseOrderResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  double? _totalAmount;
  double? get totalAmount => _$this._totalAmount;
  set totalAmount(double? totalAmount) => _$this._totalAmount = totalAmount;

  Date? _expectedDeliveryDate;
  Date? get expectedDeliveryDate => _$this._expectedDeliveryDate;
  set expectedDeliveryDate(Date? expectedDeliveryDate) =>
      _$this._expectedDeliveryDate = expectedDeliveryDate;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

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

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<PurchaseOrderLineResponse>? _lines;
  ListBuilder<PurchaseOrderLineResponse> get lines =>
      _$this._lines ??= ListBuilder<PurchaseOrderLineResponse>();
  set lines(ListBuilder<PurchaseOrderLineResponse>? lines) =>
      _$this._lines = lines;

  ListBuilder<GoodsReceiptSummaryResponse>? _receipts;
  ListBuilder<GoodsReceiptSummaryResponse> get receipts =>
      _$this._receipts ??= ListBuilder<GoodsReceiptSummaryResponse>();
  set receipts(ListBuilder<GoodsReceiptSummaryResponse>? receipts) =>
      _$this._receipts = receipts;

  PurchaseOrderResponseBuilder() {
    PurchaseOrderResponse._defaults(this);
  }

  PurchaseOrderResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _status = $v.status;
      _currency = $v.currency;
      _totalAmount = $v.totalAmount;
      _expectedDeliveryDate = $v.expectedDeliveryDate;
      _note = $v.note;
      _supplier = $v.supplier?.toBuilder();
      _project = $v.project?.toBuilder();
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _submittedAtUtc = $v.submittedAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _decisionNote = $v.decisionNote;
      _cancelReason = $v.cancelReason;
      _rowVersion = $v.rowVersion;
      _lines = $v.lines?.toBuilder();
      _receipts = $v.receipts?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderResponse other) {
    _$v = other as _$PurchaseOrderResponse;
  }

  @override
  void update(void Function(PurchaseOrderResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderResponse build() => _build();

  _$PurchaseOrderResponse _build() {
    _$PurchaseOrderResponse _$result;
    try {
      _$result = _$v ??
          _$PurchaseOrderResponse._(
            id: id,
            branchId: branchId,
            number: number,
            status: status,
            currency: currency,
            totalAmount: totalAmount,
            expectedDeliveryDate: expectedDeliveryDate,
            note: note,
            supplier: _supplier?.build(),
            project: _project?.build(),
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            submittedAtUtc: submittedAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            decisionNote: decisionNote,
            cancelReason: cancelReason,
            rowVersion: rowVersion,
            lines: _lines?.build(),
            receipts: _receipts?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'supplier';
        _supplier?.build();
        _$failedField = 'project';
        _project?.build();
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();

        _$failedField = 'lines';
        _lines?.build();
        _$failedField = 'receipts';
        _receipts?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PurchaseOrderResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
