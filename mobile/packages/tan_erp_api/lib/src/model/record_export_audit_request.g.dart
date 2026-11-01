// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'record_export_audit_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RecordExportAuditRequest extends RecordExportAuditRequest {
  @override
  final String? resourceType;
  @override
  final int? rowCount;
  @override
  final String? format;
  @override
  final String? filename;
  @override
  final String? filterSummary;

  factory _$RecordExportAuditRequest(
          [void Function(RecordExportAuditRequestBuilder)? updates]) =>
      (RecordExportAuditRequestBuilder()..update(updates))._build();

  _$RecordExportAuditRequest._(
      {this.resourceType,
      this.rowCount,
      this.format,
      this.filename,
      this.filterSummary})
      : super._();
  @override
  RecordExportAuditRequest rebuild(
          void Function(RecordExportAuditRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RecordExportAuditRequestBuilder toBuilder() =>
      RecordExportAuditRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RecordExportAuditRequest &&
        resourceType == other.resourceType &&
        rowCount == other.rowCount &&
        format == other.format &&
        filename == other.filename &&
        filterSummary == other.filterSummary;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, resourceType.hashCode);
    _$hash = $jc(_$hash, rowCount.hashCode);
    _$hash = $jc(_$hash, format.hashCode);
    _$hash = $jc(_$hash, filename.hashCode);
    _$hash = $jc(_$hash, filterSummary.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RecordExportAuditRequest')
          ..add('resourceType', resourceType)
          ..add('rowCount', rowCount)
          ..add('format', format)
          ..add('filename', filename)
          ..add('filterSummary', filterSummary))
        .toString();
  }
}

class RecordExportAuditRequestBuilder
    implements
        Builder<RecordExportAuditRequest, RecordExportAuditRequestBuilder> {
  _$RecordExportAuditRequest? _$v;

  String? _resourceType;
  String? get resourceType => _$this._resourceType;
  set resourceType(String? resourceType) => _$this._resourceType = resourceType;

  int? _rowCount;
  int? get rowCount => _$this._rowCount;
  set rowCount(int? rowCount) => _$this._rowCount = rowCount;

  String? _format;
  String? get format => _$this._format;
  set format(String? format) => _$this._format = format;

  String? _filename;
  String? get filename => _$this._filename;
  set filename(String? filename) => _$this._filename = filename;

  String? _filterSummary;
  String? get filterSummary => _$this._filterSummary;
  set filterSummary(String? filterSummary) =>
      _$this._filterSummary = filterSummary;

  RecordExportAuditRequestBuilder() {
    RecordExportAuditRequest._defaults(this);
  }

  RecordExportAuditRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _resourceType = $v.resourceType;
      _rowCount = $v.rowCount;
      _format = $v.format;
      _filename = $v.filename;
      _filterSummary = $v.filterSummary;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RecordExportAuditRequest other) {
    _$v = other as _$RecordExportAuditRequest;
  }

  @override
  void update(void Function(RecordExportAuditRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RecordExportAuditRequest build() => _build();

  _$RecordExportAuditRequest _build() {
    final _$result = _$v ??
        _$RecordExportAuditRequest._(
          resourceType: resourceType,
          rowCount: rowCount,
          format: format,
          filename: filename,
          filterSummary: filterSummary,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
