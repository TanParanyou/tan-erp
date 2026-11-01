// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'return_stock_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReturnStockRequest extends ReturnStockRequest {
  @override
  final String? warehouseId;
  @override
  final String? projectId;
  @override
  final String? reason;
  @override
  final BuiltList<ReturnStockLineRequest>? lines;
  @override
  final String? locationId;
  @override
  final String? sourceIssueDocumentId;
  @override
  final DateTime? occurredAtUtc;

  factory _$ReturnStockRequest(
          [void Function(ReturnStockRequestBuilder)? updates]) =>
      (ReturnStockRequestBuilder()..update(updates))._build();

  _$ReturnStockRequest._(
      {this.warehouseId,
      this.projectId,
      this.reason,
      this.lines,
      this.locationId,
      this.sourceIssueDocumentId,
      this.occurredAtUtc})
      : super._();
  @override
  ReturnStockRequest rebuild(
          void Function(ReturnStockRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReturnStockRequestBuilder toBuilder() =>
      ReturnStockRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReturnStockRequest &&
        warehouseId == other.warehouseId &&
        projectId == other.projectId &&
        reason == other.reason &&
        lines == other.lines &&
        locationId == other.locationId &&
        sourceIssueDocumentId == other.sourceIssueDocumentId &&
        occurredAtUtc == other.occurredAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jc(_$hash, locationId.hashCode);
    _$hash = $jc(_$hash, sourceIssueDocumentId.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReturnStockRequest')
          ..add('warehouseId', warehouseId)
          ..add('projectId', projectId)
          ..add('reason', reason)
          ..add('lines', lines)
          ..add('locationId', locationId)
          ..add('sourceIssueDocumentId', sourceIssueDocumentId)
          ..add('occurredAtUtc', occurredAtUtc))
        .toString();
  }
}

class ReturnStockRequestBuilder
    implements Builder<ReturnStockRequest, ReturnStockRequestBuilder> {
  _$ReturnStockRequest? _$v;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  ListBuilder<ReturnStockLineRequest>? _lines;
  ListBuilder<ReturnStockLineRequest> get lines =>
      _$this._lines ??= ListBuilder<ReturnStockLineRequest>();
  set lines(ListBuilder<ReturnStockLineRequest>? lines) =>
      _$this._lines = lines;

  String? _locationId;
  String? get locationId => _$this._locationId;
  set locationId(String? locationId) => _$this._locationId = locationId;

  String? _sourceIssueDocumentId;
  String? get sourceIssueDocumentId => _$this._sourceIssueDocumentId;
  set sourceIssueDocumentId(String? sourceIssueDocumentId) =>
      _$this._sourceIssueDocumentId = sourceIssueDocumentId;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  ReturnStockRequestBuilder() {
    ReturnStockRequest._defaults(this);
  }

  ReturnStockRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _warehouseId = $v.warehouseId;
      _projectId = $v.projectId;
      _reason = $v.reason;
      _lines = $v.lines?.toBuilder();
      _locationId = $v.locationId;
      _sourceIssueDocumentId = $v.sourceIssueDocumentId;
      _occurredAtUtc = $v.occurredAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReturnStockRequest other) {
    _$v = other as _$ReturnStockRequest;
  }

  @override
  void update(void Function(ReturnStockRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReturnStockRequest build() => _build();

  _$ReturnStockRequest _build() {
    _$ReturnStockRequest _$result;
    try {
      _$result = _$v ??
          _$ReturnStockRequest._(
            warehouseId: warehouseId,
            projectId: projectId,
            reason: reason,
            lines: _lines?.build(),
            locationId: locationId,
            sourceIssueDocumentId: sourceIssueDocumentId,
            occurredAtUtc: occurredAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ReturnStockRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
