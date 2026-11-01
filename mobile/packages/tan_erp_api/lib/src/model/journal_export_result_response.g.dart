// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'journal_export_result_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$JournalExportResultResponse extends JournalExportResultResponse {
  @override
  final String? batchRef;
  @override
  final int? exportedCount;
  @override
  final int? deadCount;
  @override
  final BuiltList<JournalExportLineResponse>? lines;
  @override
  final String? csvContent;

  factory _$JournalExportResultResponse(
          [void Function(JournalExportResultResponseBuilder)? updates]) =>
      (JournalExportResultResponseBuilder()..update(updates))._build();

  _$JournalExportResultResponse._(
      {this.batchRef,
      this.exportedCount,
      this.deadCount,
      this.lines,
      this.csvContent})
      : super._();
  @override
  JournalExportResultResponse rebuild(
          void Function(JournalExportResultResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  JournalExportResultResponseBuilder toBuilder() =>
      JournalExportResultResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is JournalExportResultResponse &&
        batchRef == other.batchRef &&
        exportedCount == other.exportedCount &&
        deadCount == other.deadCount &&
        lines == other.lines &&
        csvContent == other.csvContent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, batchRef.hashCode);
    _$hash = $jc(_$hash, exportedCount.hashCode);
    _$hash = $jc(_$hash, deadCount.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jc(_$hash, csvContent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'JournalExportResultResponse')
          ..add('batchRef', batchRef)
          ..add('exportedCount', exportedCount)
          ..add('deadCount', deadCount)
          ..add('lines', lines)
          ..add('csvContent', csvContent))
        .toString();
  }
}

class JournalExportResultResponseBuilder
    implements
        Builder<JournalExportResultResponse,
            JournalExportResultResponseBuilder> {
  _$JournalExportResultResponse? _$v;

  String? _batchRef;
  String? get batchRef => _$this._batchRef;
  set batchRef(String? batchRef) => _$this._batchRef = batchRef;

  int? _exportedCount;
  int? get exportedCount => _$this._exportedCount;
  set exportedCount(int? exportedCount) =>
      _$this._exportedCount = exportedCount;

  int? _deadCount;
  int? get deadCount => _$this._deadCount;
  set deadCount(int? deadCount) => _$this._deadCount = deadCount;

  ListBuilder<JournalExportLineResponse>? _lines;
  ListBuilder<JournalExportLineResponse> get lines =>
      _$this._lines ??= ListBuilder<JournalExportLineResponse>();
  set lines(ListBuilder<JournalExportLineResponse>? lines) =>
      _$this._lines = lines;

  String? _csvContent;
  String? get csvContent => _$this._csvContent;
  set csvContent(String? csvContent) => _$this._csvContent = csvContent;

  JournalExportResultResponseBuilder() {
    JournalExportResultResponse._defaults(this);
  }

  JournalExportResultResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _batchRef = $v.batchRef;
      _exportedCount = $v.exportedCount;
      _deadCount = $v.deadCount;
      _lines = $v.lines?.toBuilder();
      _csvContent = $v.csvContent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(JournalExportResultResponse other) {
    _$v = other as _$JournalExportResultResponse;
  }

  @override
  void update(void Function(JournalExportResultResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  JournalExportResultResponse build() => _build();

  _$JournalExportResultResponse _build() {
    _$JournalExportResultResponse _$result;
    try {
      _$result = _$v ??
          _$JournalExportResultResponse._(
            batchRef: batchRef,
            exportedCount: exportedCount,
            deadCount: deadCount,
            lines: _lines?.build(),
            csvContent: csvContent,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'JournalExportResultResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
