// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_document_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockDocumentResponse extends StockDocumentResponse {
  @override
  final String? id;
  @override
  final String? documentType;
  @override
  final String? number;
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryWarehouseRefResponse? toWarehouse;
  @override
  final String? projectId;
  @override
  final String? sourceType;
  @override
  final String? sourceId;
  @override
  final String? reason;
  @override
  final DateTime? occurredAtUtc;
  @override
  final InventoryPersonResponse? postedBy;
  @override
  final DateTime? postedAtUtc;
  @override
  final BuiltList<StockMovementResponse>? movements;

  factory _$StockDocumentResponse(
          [void Function(StockDocumentResponseBuilder)? updates]) =>
      (StockDocumentResponseBuilder()..update(updates))._build();

  _$StockDocumentResponse._(
      {this.id,
      this.documentType,
      this.number,
      this.warehouse,
      this.toWarehouse,
      this.projectId,
      this.sourceType,
      this.sourceId,
      this.reason,
      this.occurredAtUtc,
      this.postedBy,
      this.postedAtUtc,
      this.movements})
      : super._();
  @override
  StockDocumentResponse rebuild(
          void Function(StockDocumentResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockDocumentResponseBuilder toBuilder() =>
      StockDocumentResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockDocumentResponse &&
        id == other.id &&
        documentType == other.documentType &&
        number == other.number &&
        warehouse == other.warehouse &&
        toWarehouse == other.toWarehouse &&
        projectId == other.projectId &&
        sourceType == other.sourceType &&
        sourceId == other.sourceId &&
        reason == other.reason &&
        occurredAtUtc == other.occurredAtUtc &&
        postedBy == other.postedBy &&
        postedAtUtc == other.postedAtUtc &&
        movements == other.movements;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, documentType.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, toWarehouse.hashCode);
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, sourceType.hashCode);
    _$hash = $jc(_$hash, sourceId.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jc(_$hash, postedBy.hashCode);
    _$hash = $jc(_$hash, postedAtUtc.hashCode);
    _$hash = $jc(_$hash, movements.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockDocumentResponse')
          ..add('id', id)
          ..add('documentType', documentType)
          ..add('number', number)
          ..add('warehouse', warehouse)
          ..add('toWarehouse', toWarehouse)
          ..add('projectId', projectId)
          ..add('sourceType', sourceType)
          ..add('sourceId', sourceId)
          ..add('reason', reason)
          ..add('occurredAtUtc', occurredAtUtc)
          ..add('postedBy', postedBy)
          ..add('postedAtUtc', postedAtUtc)
          ..add('movements', movements))
        .toString();
  }
}

class StockDocumentResponseBuilder
    implements Builder<StockDocumentResponse, StockDocumentResponseBuilder> {
  _$StockDocumentResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _documentType;
  String? get documentType => _$this._documentType;
  set documentType(String? documentType) => _$this._documentType = documentType;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  InventoryWarehouseRefResponseBuilder? _warehouse;
  InventoryWarehouseRefResponseBuilder get warehouse =>
      _$this._warehouse ??= InventoryWarehouseRefResponseBuilder();
  set warehouse(InventoryWarehouseRefResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

  InventoryWarehouseRefResponseBuilder? _toWarehouse;
  InventoryWarehouseRefResponseBuilder get toWarehouse =>
      _$this._toWarehouse ??= InventoryWarehouseRefResponseBuilder();
  set toWarehouse(InventoryWarehouseRefResponseBuilder? toWarehouse) =>
      _$this._toWarehouse = toWarehouse;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _sourceType;
  String? get sourceType => _$this._sourceType;
  set sourceType(String? sourceType) => _$this._sourceType = sourceType;

  String? _sourceId;
  String? get sourceId => _$this._sourceId;
  set sourceId(String? sourceId) => _$this._sourceId = sourceId;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  InventoryPersonResponseBuilder? _postedBy;
  InventoryPersonResponseBuilder get postedBy =>
      _$this._postedBy ??= InventoryPersonResponseBuilder();
  set postedBy(InventoryPersonResponseBuilder? postedBy) =>
      _$this._postedBy = postedBy;

  DateTime? _postedAtUtc;
  DateTime? get postedAtUtc => _$this._postedAtUtc;
  set postedAtUtc(DateTime? postedAtUtc) => _$this._postedAtUtc = postedAtUtc;

  ListBuilder<StockMovementResponse>? _movements;
  ListBuilder<StockMovementResponse> get movements =>
      _$this._movements ??= ListBuilder<StockMovementResponse>();
  set movements(ListBuilder<StockMovementResponse>? movements) =>
      _$this._movements = movements;

  StockDocumentResponseBuilder() {
    StockDocumentResponse._defaults(this);
  }

  StockDocumentResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _documentType = $v.documentType;
      _number = $v.number;
      _warehouse = $v.warehouse?.toBuilder();
      _toWarehouse = $v.toWarehouse?.toBuilder();
      _projectId = $v.projectId;
      _sourceType = $v.sourceType;
      _sourceId = $v.sourceId;
      _reason = $v.reason;
      _occurredAtUtc = $v.occurredAtUtc;
      _postedBy = $v.postedBy?.toBuilder();
      _postedAtUtc = $v.postedAtUtc;
      _movements = $v.movements?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockDocumentResponse other) {
    _$v = other as _$StockDocumentResponse;
  }

  @override
  void update(void Function(StockDocumentResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockDocumentResponse build() => _build();

  _$StockDocumentResponse _build() {
    _$StockDocumentResponse _$result;
    try {
      _$result = _$v ??
          _$StockDocumentResponse._(
            id: id,
            documentType: documentType,
            number: number,
            warehouse: _warehouse?.build(),
            toWarehouse: _toWarehouse?.build(),
            projectId: projectId,
            sourceType: sourceType,
            sourceId: sourceId,
            reason: reason,
            occurredAtUtc: occurredAtUtc,
            postedBy: _postedBy?.build(),
            postedAtUtc: postedAtUtc,
            movements: _movements?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'warehouse';
        _warehouse?.build();
        _$failedField = 'toWarehouse';
        _toWarehouse?.build();

        _$failedField = 'postedBy';
        _postedBy?.build();

        _$failedField = 'movements';
        _movements?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'StockDocumentResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
