// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderRequest extends PurchaseOrderRequest {
  @override
  final String? supplierId;
  @override
  final String? projectId;
  @override
  final Date? expectedDeliveryDate;
  @override
  final String? note;
  @override
  final BuiltList<PurchaseOrderLineRequest>? lines;

  factory _$PurchaseOrderRequest(
          [void Function(PurchaseOrderRequestBuilder)? updates]) =>
      (PurchaseOrderRequestBuilder()..update(updates))._build();

  _$PurchaseOrderRequest._(
      {this.supplierId,
      this.projectId,
      this.expectedDeliveryDate,
      this.note,
      this.lines})
      : super._();
  @override
  PurchaseOrderRequest rebuild(
          void Function(PurchaseOrderRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderRequestBuilder toBuilder() =>
      PurchaseOrderRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderRequest &&
        supplierId == other.supplierId &&
        projectId == other.projectId &&
        expectedDeliveryDate == other.expectedDeliveryDate &&
        note == other.note &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, supplierId.hashCode);
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, expectedDeliveryDate.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderRequest')
          ..add('supplierId', supplierId)
          ..add('projectId', projectId)
          ..add('expectedDeliveryDate', expectedDeliveryDate)
          ..add('note', note)
          ..add('lines', lines))
        .toString();
  }
}

class PurchaseOrderRequestBuilder
    implements Builder<PurchaseOrderRequest, PurchaseOrderRequestBuilder> {
  _$PurchaseOrderRequest? _$v;

  String? _supplierId;
  String? get supplierId => _$this._supplierId;
  set supplierId(String? supplierId) => _$this._supplierId = supplierId;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  Date? _expectedDeliveryDate;
  Date? get expectedDeliveryDate => _$this._expectedDeliveryDate;
  set expectedDeliveryDate(Date? expectedDeliveryDate) =>
      _$this._expectedDeliveryDate = expectedDeliveryDate;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ListBuilder<PurchaseOrderLineRequest>? _lines;
  ListBuilder<PurchaseOrderLineRequest> get lines =>
      _$this._lines ??= ListBuilder<PurchaseOrderLineRequest>();
  set lines(ListBuilder<PurchaseOrderLineRequest>? lines) =>
      _$this._lines = lines;

  PurchaseOrderRequestBuilder() {
    PurchaseOrderRequest._defaults(this);
  }

  PurchaseOrderRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _supplierId = $v.supplierId;
      _projectId = $v.projectId;
      _expectedDeliveryDate = $v.expectedDeliveryDate;
      _note = $v.note;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderRequest other) {
    _$v = other as _$PurchaseOrderRequest;
  }

  @override
  void update(void Function(PurchaseOrderRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderRequest build() => _build();

  _$PurchaseOrderRequest _build() {
    _$PurchaseOrderRequest _$result;
    try {
      _$result = _$v ??
          _$PurchaseOrderRequest._(
            supplierId: supplierId,
            projectId: projectId,
            expectedDeliveryDate: expectedDeliveryDate,
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
            r'PurchaseOrderRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
