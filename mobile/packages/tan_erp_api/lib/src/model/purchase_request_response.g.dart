// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestResponse extends PurchaseRequestResponse {
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
  final double? totalEstimatedAmount;
  @override
  final Date? requiredByDate;
  @override
  final String? note;
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
  final PurchaseRequestConvertedOrderResponse? convertedPurchaseOrder;
  @override
  final DateTime? convertedAtUtc;
  @override
  final String? rowVersion;
  @override
  final BuiltList<PurchaseRequestLineResponse>? lines;

  factory _$PurchaseRequestResponse(
          [void Function(PurchaseRequestResponseBuilder)? updates]) =>
      (PurchaseRequestResponseBuilder()..update(updates))._build();

  _$PurchaseRequestResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.status,
      this.currency,
      this.totalEstimatedAmount,
      this.requiredByDate,
      this.note,
      this.project,
      this.createdBy,
      this.createdAtUtc,
      this.submittedAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.decisionNote,
      this.cancelReason,
      this.convertedPurchaseOrder,
      this.convertedAtUtc,
      this.rowVersion,
      this.lines})
      : super._();
  @override
  PurchaseRequestResponse rebuild(
          void Function(PurchaseRequestResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestResponseBuilder toBuilder() =>
      PurchaseRequestResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        status == other.status &&
        currency == other.currency &&
        totalEstimatedAmount == other.totalEstimatedAmount &&
        requiredByDate == other.requiredByDate &&
        note == other.note &&
        project == other.project &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        submittedAtUtc == other.submittedAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        decisionNote == other.decisionNote &&
        cancelReason == other.cancelReason &&
        convertedPurchaseOrder == other.convertedPurchaseOrder &&
        convertedAtUtc == other.convertedAtUtc &&
        rowVersion == other.rowVersion &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, totalEstimatedAmount.hashCode);
    _$hash = $jc(_$hash, requiredByDate.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, submittedAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jc(_$hash, cancelReason.hashCode);
    _$hash = $jc(_$hash, convertedPurchaseOrder.hashCode);
    _$hash = $jc(_$hash, convertedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseRequestResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('status', status)
          ..add('currency', currency)
          ..add('totalEstimatedAmount', totalEstimatedAmount)
          ..add('requiredByDate', requiredByDate)
          ..add('note', note)
          ..add('project', project)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('submittedAtUtc', submittedAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('decisionNote', decisionNote)
          ..add('cancelReason', cancelReason)
          ..add('convertedPurchaseOrder', convertedPurchaseOrder)
          ..add('convertedAtUtc', convertedAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('lines', lines))
        .toString();
  }
}

class PurchaseRequestResponseBuilder
    implements
        Builder<PurchaseRequestResponse, PurchaseRequestResponseBuilder> {
  _$PurchaseRequestResponse? _$v;

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

  double? _totalEstimatedAmount;
  double? get totalEstimatedAmount => _$this._totalEstimatedAmount;
  set totalEstimatedAmount(double? totalEstimatedAmount) =>
      _$this._totalEstimatedAmount = totalEstimatedAmount;

  Date? _requiredByDate;
  Date? get requiredByDate => _$this._requiredByDate;
  set requiredByDate(Date? requiredByDate) =>
      _$this._requiredByDate = requiredByDate;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

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

  PurchaseRequestConvertedOrderResponseBuilder? _convertedPurchaseOrder;
  PurchaseRequestConvertedOrderResponseBuilder get convertedPurchaseOrder =>
      _$this._convertedPurchaseOrder ??=
          PurchaseRequestConvertedOrderResponseBuilder();
  set convertedPurchaseOrder(
          PurchaseRequestConvertedOrderResponseBuilder?
              convertedPurchaseOrder) =>
      _$this._convertedPurchaseOrder = convertedPurchaseOrder;

  DateTime? _convertedAtUtc;
  DateTime? get convertedAtUtc => _$this._convertedAtUtc;
  set convertedAtUtc(DateTime? convertedAtUtc) =>
      _$this._convertedAtUtc = convertedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<PurchaseRequestLineResponse>? _lines;
  ListBuilder<PurchaseRequestLineResponse> get lines =>
      _$this._lines ??= ListBuilder<PurchaseRequestLineResponse>();
  set lines(ListBuilder<PurchaseRequestLineResponse>? lines) =>
      _$this._lines = lines;

  PurchaseRequestResponseBuilder() {
    PurchaseRequestResponse._defaults(this);
  }

  PurchaseRequestResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _status = $v.status;
      _currency = $v.currency;
      _totalEstimatedAmount = $v.totalEstimatedAmount;
      _requiredByDate = $v.requiredByDate;
      _note = $v.note;
      _project = $v.project?.toBuilder();
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _submittedAtUtc = $v.submittedAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _decisionNote = $v.decisionNote;
      _cancelReason = $v.cancelReason;
      _convertedPurchaseOrder = $v.convertedPurchaseOrder?.toBuilder();
      _convertedAtUtc = $v.convertedAtUtc;
      _rowVersion = $v.rowVersion;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestResponse other) {
    _$v = other as _$PurchaseRequestResponse;
  }

  @override
  void update(void Function(PurchaseRequestResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestResponse build() => _build();

  _$PurchaseRequestResponse _build() {
    _$PurchaseRequestResponse _$result;
    try {
      _$result = _$v ??
          _$PurchaseRequestResponse._(
            id: id,
            branchId: branchId,
            number: number,
            status: status,
            currency: currency,
            totalEstimatedAmount: totalEstimatedAmount,
            requiredByDate: requiredByDate,
            note: note,
            project: _project?.build(),
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            submittedAtUtc: submittedAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            decisionNote: decisionNote,
            cancelReason: cancelReason,
            convertedPurchaseOrder: _convertedPurchaseOrder?.build(),
            convertedAtUtc: convertedAtUtc,
            rowVersion: rowVersion,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'project';
        _project?.build();
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();

        _$failedField = 'convertedPurchaseOrder';
        _convertedPurchaseOrder?.build();

        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PurchaseRequestResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
