// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'issue_stock_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$IssueStockRequest extends IssueStockRequest {
  @override
  final String? warehouseId;
  @override
  final String? projectId;
  @override
  final String? reason;
  @override
  final BuiltList<StockLineRequest>? lines;

  factory _$IssueStockRequest(
          [void Function(IssueStockRequestBuilder)? updates]) =>
      (IssueStockRequestBuilder()..update(updates))._build();

  _$IssueStockRequest._(
      {this.warehouseId, this.projectId, this.reason, this.lines})
      : super._();
  @override
  IssueStockRequest rebuild(void Function(IssueStockRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  IssueStockRequestBuilder toBuilder() =>
      IssueStockRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is IssueStockRequest &&
        warehouseId == other.warehouseId &&
        projectId == other.projectId &&
        reason == other.reason &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'IssueStockRequest')
          ..add('warehouseId', warehouseId)
          ..add('projectId', projectId)
          ..add('reason', reason)
          ..add('lines', lines))
        .toString();
  }
}

class IssueStockRequestBuilder
    implements Builder<IssueStockRequest, IssueStockRequestBuilder> {
  _$IssueStockRequest? _$v;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  ListBuilder<StockLineRequest>? _lines;
  ListBuilder<StockLineRequest> get lines =>
      _$this._lines ??= ListBuilder<StockLineRequest>();
  set lines(ListBuilder<StockLineRequest>? lines) => _$this._lines = lines;

  IssueStockRequestBuilder() {
    IssueStockRequest._defaults(this);
  }

  IssueStockRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _warehouseId = $v.warehouseId;
      _projectId = $v.projectId;
      _reason = $v.reason;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(IssueStockRequest other) {
    _$v = other as _$IssueStockRequest;
  }

  @override
  void update(void Function(IssueStockRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  IssueStockRequest build() => _build();

  _$IssueStockRequest _build() {
    _$IssueStockRequest _$result;
    try {
      _$result = _$v ??
          _$IssueStockRequest._(
            warehouseId: warehouseId,
            projectId: projectId,
            reason: reason,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'IssueStockRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
