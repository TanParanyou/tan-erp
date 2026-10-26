// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_import_job_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImportJobResponse extends ItemImportJobResponse {
  @override
  final String? id;
  @override
  final String? kind;
  @override
  final String? mode;
  @override
  final String? status;
  @override
  final String? contentSha256;
  @override
  final int? totalRows;
  @override
  final int? processedRows;
  @override
  final int? batchSize;
  @override
  final int? totalBatches;
  @override
  final int? completedBatches;
  @override
  final int? rejectedBatches;
  @override
  final int? createdCount;
  @override
  final int? updatedCount;
  @override
  final int? unchangedCount;
  @override
  final int? errorRowCount;
  @override
  final bool? errorsTruncated;
  @override
  final BuiltList<ItemImportJobRowResponse>? errors;
  @override
  final String? failureCode;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? startedAtUtc;
  @override
  final DateTime? completedAtUtc;
  @override
  final bool? replayed;

  factory _$ItemImportJobResponse(
          [void Function(ItemImportJobResponseBuilder)? updates]) =>
      (ItemImportJobResponseBuilder()..update(updates))._build();

  _$ItemImportJobResponse._(
      {this.id,
      this.kind,
      this.mode,
      this.status,
      this.contentSha256,
      this.totalRows,
      this.processedRows,
      this.batchSize,
      this.totalBatches,
      this.completedBatches,
      this.rejectedBatches,
      this.createdCount,
      this.updatedCount,
      this.unchangedCount,
      this.errorRowCount,
      this.errorsTruncated,
      this.errors,
      this.failureCode,
      this.createdAtUtc,
      this.startedAtUtc,
      this.completedAtUtc,
      this.replayed})
      : super._();
  @override
  ItemImportJobResponse rebuild(
          void Function(ItemImportJobResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImportJobResponseBuilder toBuilder() =>
      ItemImportJobResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImportJobResponse &&
        id == other.id &&
        kind == other.kind &&
        mode == other.mode &&
        status == other.status &&
        contentSha256 == other.contentSha256 &&
        totalRows == other.totalRows &&
        processedRows == other.processedRows &&
        batchSize == other.batchSize &&
        totalBatches == other.totalBatches &&
        completedBatches == other.completedBatches &&
        rejectedBatches == other.rejectedBatches &&
        createdCount == other.createdCount &&
        updatedCount == other.updatedCount &&
        unchangedCount == other.unchangedCount &&
        errorRowCount == other.errorRowCount &&
        errorsTruncated == other.errorsTruncated &&
        errors == other.errors &&
        failureCode == other.failureCode &&
        createdAtUtc == other.createdAtUtc &&
        startedAtUtc == other.startedAtUtc &&
        completedAtUtc == other.completedAtUtc &&
        replayed == other.replayed;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, mode.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, contentSha256.hashCode);
    _$hash = $jc(_$hash, totalRows.hashCode);
    _$hash = $jc(_$hash, processedRows.hashCode);
    _$hash = $jc(_$hash, batchSize.hashCode);
    _$hash = $jc(_$hash, totalBatches.hashCode);
    _$hash = $jc(_$hash, completedBatches.hashCode);
    _$hash = $jc(_$hash, rejectedBatches.hashCode);
    _$hash = $jc(_$hash, createdCount.hashCode);
    _$hash = $jc(_$hash, updatedCount.hashCode);
    _$hash = $jc(_$hash, unchangedCount.hashCode);
    _$hash = $jc(_$hash, errorRowCount.hashCode);
    _$hash = $jc(_$hash, errorsTruncated.hashCode);
    _$hash = $jc(_$hash, errors.hashCode);
    _$hash = $jc(_$hash, failureCode.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, startedAtUtc.hashCode);
    _$hash = $jc(_$hash, completedAtUtc.hashCode);
    _$hash = $jc(_$hash, replayed.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImportJobResponse')
          ..add('id', id)
          ..add('kind', kind)
          ..add('mode', mode)
          ..add('status', status)
          ..add('contentSha256', contentSha256)
          ..add('totalRows', totalRows)
          ..add('processedRows', processedRows)
          ..add('batchSize', batchSize)
          ..add('totalBatches', totalBatches)
          ..add('completedBatches', completedBatches)
          ..add('rejectedBatches', rejectedBatches)
          ..add('createdCount', createdCount)
          ..add('updatedCount', updatedCount)
          ..add('unchangedCount', unchangedCount)
          ..add('errorRowCount', errorRowCount)
          ..add('errorsTruncated', errorsTruncated)
          ..add('errors', errors)
          ..add('failureCode', failureCode)
          ..add('createdAtUtc', createdAtUtc)
          ..add('startedAtUtc', startedAtUtc)
          ..add('completedAtUtc', completedAtUtc)
          ..add('replayed', replayed))
        .toString();
  }
}

class ItemImportJobResponseBuilder
    implements Builder<ItemImportJobResponse, ItemImportJobResponseBuilder> {
  _$ItemImportJobResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _mode;
  String? get mode => _$this._mode;
  set mode(String? mode) => _$this._mode = mode;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _contentSha256;
  String? get contentSha256 => _$this._contentSha256;
  set contentSha256(String? contentSha256) =>
      _$this._contentSha256 = contentSha256;

  int? _totalRows;
  int? get totalRows => _$this._totalRows;
  set totalRows(int? totalRows) => _$this._totalRows = totalRows;

  int? _processedRows;
  int? get processedRows => _$this._processedRows;
  set processedRows(int? processedRows) =>
      _$this._processedRows = processedRows;

  int? _batchSize;
  int? get batchSize => _$this._batchSize;
  set batchSize(int? batchSize) => _$this._batchSize = batchSize;

  int? _totalBatches;
  int? get totalBatches => _$this._totalBatches;
  set totalBatches(int? totalBatches) => _$this._totalBatches = totalBatches;

  int? _completedBatches;
  int? get completedBatches => _$this._completedBatches;
  set completedBatches(int? completedBatches) =>
      _$this._completedBatches = completedBatches;

  int? _rejectedBatches;
  int? get rejectedBatches => _$this._rejectedBatches;
  set rejectedBatches(int? rejectedBatches) =>
      _$this._rejectedBatches = rejectedBatches;

  int? _createdCount;
  int? get createdCount => _$this._createdCount;
  set createdCount(int? createdCount) => _$this._createdCount = createdCount;

  int? _updatedCount;
  int? get updatedCount => _$this._updatedCount;
  set updatedCount(int? updatedCount) => _$this._updatedCount = updatedCount;

  int? _unchangedCount;
  int? get unchangedCount => _$this._unchangedCount;
  set unchangedCount(int? unchangedCount) =>
      _$this._unchangedCount = unchangedCount;

  int? _errorRowCount;
  int? get errorRowCount => _$this._errorRowCount;
  set errorRowCount(int? errorRowCount) =>
      _$this._errorRowCount = errorRowCount;

  bool? _errorsTruncated;
  bool? get errorsTruncated => _$this._errorsTruncated;
  set errorsTruncated(bool? errorsTruncated) =>
      _$this._errorsTruncated = errorsTruncated;

  ListBuilder<ItemImportJobRowResponse>? _errors;
  ListBuilder<ItemImportJobRowResponse> get errors =>
      _$this._errors ??= ListBuilder<ItemImportJobRowResponse>();
  set errors(ListBuilder<ItemImportJobRowResponse>? errors) =>
      _$this._errors = errors;

  String? _failureCode;
  String? get failureCode => _$this._failureCode;
  set failureCode(String? failureCode) => _$this._failureCode = failureCode;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _startedAtUtc;
  DateTime? get startedAtUtc => _$this._startedAtUtc;
  set startedAtUtc(DateTime? startedAtUtc) =>
      _$this._startedAtUtc = startedAtUtc;

  DateTime? _completedAtUtc;
  DateTime? get completedAtUtc => _$this._completedAtUtc;
  set completedAtUtc(DateTime? completedAtUtc) =>
      _$this._completedAtUtc = completedAtUtc;

  bool? _replayed;
  bool? get replayed => _$this._replayed;
  set replayed(bool? replayed) => _$this._replayed = replayed;

  ItemImportJobResponseBuilder() {
    ItemImportJobResponse._defaults(this);
  }

  ItemImportJobResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _kind = $v.kind;
      _mode = $v.mode;
      _status = $v.status;
      _contentSha256 = $v.contentSha256;
      _totalRows = $v.totalRows;
      _processedRows = $v.processedRows;
      _batchSize = $v.batchSize;
      _totalBatches = $v.totalBatches;
      _completedBatches = $v.completedBatches;
      _rejectedBatches = $v.rejectedBatches;
      _createdCount = $v.createdCount;
      _updatedCount = $v.updatedCount;
      _unchangedCount = $v.unchangedCount;
      _errorRowCount = $v.errorRowCount;
      _errorsTruncated = $v.errorsTruncated;
      _errors = $v.errors?.toBuilder();
      _failureCode = $v.failureCode;
      _createdAtUtc = $v.createdAtUtc;
      _startedAtUtc = $v.startedAtUtc;
      _completedAtUtc = $v.completedAtUtc;
      _replayed = $v.replayed;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImportJobResponse other) {
    _$v = other as _$ItemImportJobResponse;
  }

  @override
  void update(void Function(ItemImportJobResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImportJobResponse build() => _build();

  _$ItemImportJobResponse _build() {
    _$ItemImportJobResponse _$result;
    try {
      _$result = _$v ??
          _$ItemImportJobResponse._(
            id: id,
            kind: kind,
            mode: mode,
            status: status,
            contentSha256: contentSha256,
            totalRows: totalRows,
            processedRows: processedRows,
            batchSize: batchSize,
            totalBatches: totalBatches,
            completedBatches: completedBatches,
            rejectedBatches: rejectedBatches,
            createdCount: createdCount,
            updatedCount: updatedCount,
            unchangedCount: unchangedCount,
            errorRowCount: errorRowCount,
            errorsTruncated: errorsTruncated,
            errors: _errors?.build(),
            failureCode: failureCode,
            createdAtUtc: createdAtUtc,
            startedAtUtc: startedAtUtc,
            completedAtUtc: completedAtUtc,
            replayed: replayed,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'errors';
        _errors?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemImportJobResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
