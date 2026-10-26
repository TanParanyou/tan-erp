// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_import_preview_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImportPreviewResponse extends ItemImportPreviewResponse {
  @override
  final String? contentSha256;
  @override
  final int? totalRows;
  @override
  final int? validRows;
  @override
  final int? invalidRows;
  @override
  final BuiltList<ItemImportRowResponse>? rows;
  @override
  final int? createCount;
  @override
  final int? updateCount;
  @override
  final int? unchangedCount;

  factory _$ItemImportPreviewResponse(
          [void Function(ItemImportPreviewResponseBuilder)? updates]) =>
      (ItemImportPreviewResponseBuilder()..update(updates))._build();

  _$ItemImportPreviewResponse._(
      {this.contentSha256,
      this.totalRows,
      this.validRows,
      this.invalidRows,
      this.rows,
      this.createCount,
      this.updateCount,
      this.unchangedCount})
      : super._();
  @override
  ItemImportPreviewResponse rebuild(
          void Function(ItemImportPreviewResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImportPreviewResponseBuilder toBuilder() =>
      ItemImportPreviewResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImportPreviewResponse &&
        contentSha256 == other.contentSha256 &&
        totalRows == other.totalRows &&
        validRows == other.validRows &&
        invalidRows == other.invalidRows &&
        rows == other.rows &&
        createCount == other.createCount &&
        updateCount == other.updateCount &&
        unchangedCount == other.unchangedCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, contentSha256.hashCode);
    _$hash = $jc(_$hash, totalRows.hashCode);
    _$hash = $jc(_$hash, validRows.hashCode);
    _$hash = $jc(_$hash, invalidRows.hashCode);
    _$hash = $jc(_$hash, rows.hashCode);
    _$hash = $jc(_$hash, createCount.hashCode);
    _$hash = $jc(_$hash, updateCount.hashCode);
    _$hash = $jc(_$hash, unchangedCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImportPreviewResponse')
          ..add('contentSha256', contentSha256)
          ..add('totalRows', totalRows)
          ..add('validRows', validRows)
          ..add('invalidRows', invalidRows)
          ..add('rows', rows)
          ..add('createCount', createCount)
          ..add('updateCount', updateCount)
          ..add('unchangedCount', unchangedCount))
        .toString();
  }
}

class ItemImportPreviewResponseBuilder
    implements
        Builder<ItemImportPreviewResponse, ItemImportPreviewResponseBuilder> {
  _$ItemImportPreviewResponse? _$v;

  String? _contentSha256;
  String? get contentSha256 => _$this._contentSha256;
  set contentSha256(String? contentSha256) =>
      _$this._contentSha256 = contentSha256;

  int? _totalRows;
  int? get totalRows => _$this._totalRows;
  set totalRows(int? totalRows) => _$this._totalRows = totalRows;

  int? _validRows;
  int? get validRows => _$this._validRows;
  set validRows(int? validRows) => _$this._validRows = validRows;

  int? _invalidRows;
  int? get invalidRows => _$this._invalidRows;
  set invalidRows(int? invalidRows) => _$this._invalidRows = invalidRows;

  ListBuilder<ItemImportRowResponse>? _rows;
  ListBuilder<ItemImportRowResponse> get rows =>
      _$this._rows ??= ListBuilder<ItemImportRowResponse>();
  set rows(ListBuilder<ItemImportRowResponse>? rows) => _$this._rows = rows;

  int? _createCount;
  int? get createCount => _$this._createCount;
  set createCount(int? createCount) => _$this._createCount = createCount;

  int? _updateCount;
  int? get updateCount => _$this._updateCount;
  set updateCount(int? updateCount) => _$this._updateCount = updateCount;

  int? _unchangedCount;
  int? get unchangedCount => _$this._unchangedCount;
  set unchangedCount(int? unchangedCount) =>
      _$this._unchangedCount = unchangedCount;

  ItemImportPreviewResponseBuilder() {
    ItemImportPreviewResponse._defaults(this);
  }

  ItemImportPreviewResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _contentSha256 = $v.contentSha256;
      _totalRows = $v.totalRows;
      _validRows = $v.validRows;
      _invalidRows = $v.invalidRows;
      _rows = $v.rows?.toBuilder();
      _createCount = $v.createCount;
      _updateCount = $v.updateCount;
      _unchangedCount = $v.unchangedCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImportPreviewResponse other) {
    _$v = other as _$ItemImportPreviewResponse;
  }

  @override
  void update(void Function(ItemImportPreviewResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImportPreviewResponse build() => _build();

  _$ItemImportPreviewResponse _build() {
    _$ItemImportPreviewResponse _$result;
    try {
      _$result = _$v ??
          _$ItemImportPreviewResponse._(
            contentSha256: contentSha256,
            totalRows: totalRows,
            validRows: validRows,
            invalidRows: invalidRows,
            rows: _rows?.build(),
            createCount: createCount,
            updateCount: updateCount,
            unchangedCount: unchangedCount,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'rows';
        _rows?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemImportPreviewResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
