// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_import_commit_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImportCommitResponse extends ItemImportCommitResponse {
  @override
  final String? batchId;
  @override
  final int? createdCount;
  @override
  final String? contentSha256;
  @override
  final bool? replayed;
  @override
  final int? updatedCount;
  @override
  final int? unchangedCount;

  factory _$ItemImportCommitResponse(
          [void Function(ItemImportCommitResponseBuilder)? updates]) =>
      (ItemImportCommitResponseBuilder()..update(updates))._build();

  _$ItemImportCommitResponse._(
      {this.batchId,
      this.createdCount,
      this.contentSha256,
      this.replayed,
      this.updatedCount,
      this.unchangedCount})
      : super._();
  @override
  ItemImportCommitResponse rebuild(
          void Function(ItemImportCommitResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImportCommitResponseBuilder toBuilder() =>
      ItemImportCommitResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImportCommitResponse &&
        batchId == other.batchId &&
        createdCount == other.createdCount &&
        contentSha256 == other.contentSha256 &&
        replayed == other.replayed &&
        updatedCount == other.updatedCount &&
        unchangedCount == other.unchangedCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, batchId.hashCode);
    _$hash = $jc(_$hash, createdCount.hashCode);
    _$hash = $jc(_$hash, contentSha256.hashCode);
    _$hash = $jc(_$hash, replayed.hashCode);
    _$hash = $jc(_$hash, updatedCount.hashCode);
    _$hash = $jc(_$hash, unchangedCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImportCommitResponse')
          ..add('batchId', batchId)
          ..add('createdCount', createdCount)
          ..add('contentSha256', contentSha256)
          ..add('replayed', replayed)
          ..add('updatedCount', updatedCount)
          ..add('unchangedCount', unchangedCount))
        .toString();
  }
}

class ItemImportCommitResponseBuilder
    implements
        Builder<ItemImportCommitResponse, ItemImportCommitResponseBuilder> {
  _$ItemImportCommitResponse? _$v;

  String? _batchId;
  String? get batchId => _$this._batchId;
  set batchId(String? batchId) => _$this._batchId = batchId;

  int? _createdCount;
  int? get createdCount => _$this._createdCount;
  set createdCount(int? createdCount) => _$this._createdCount = createdCount;

  String? _contentSha256;
  String? get contentSha256 => _$this._contentSha256;
  set contentSha256(String? contentSha256) =>
      _$this._contentSha256 = contentSha256;

  bool? _replayed;
  bool? get replayed => _$this._replayed;
  set replayed(bool? replayed) => _$this._replayed = replayed;

  int? _updatedCount;
  int? get updatedCount => _$this._updatedCount;
  set updatedCount(int? updatedCount) => _$this._updatedCount = updatedCount;

  int? _unchangedCount;
  int? get unchangedCount => _$this._unchangedCount;
  set unchangedCount(int? unchangedCount) =>
      _$this._unchangedCount = unchangedCount;

  ItemImportCommitResponseBuilder() {
    ItemImportCommitResponse._defaults(this);
  }

  ItemImportCommitResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _batchId = $v.batchId;
      _createdCount = $v.createdCount;
      _contentSha256 = $v.contentSha256;
      _replayed = $v.replayed;
      _updatedCount = $v.updatedCount;
      _unchangedCount = $v.unchangedCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImportCommitResponse other) {
    _$v = other as _$ItemImportCommitResponse;
  }

  @override
  void update(void Function(ItemImportCommitResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImportCommitResponse build() => _build();

  _$ItemImportCommitResponse _build() {
    final _$result = _$v ??
        _$ItemImportCommitResponse._(
          batchId: batchId,
          createdCount: createdCount,
          contentSha256: contentSha256,
          replayed: replayed,
          updatedCount: updatedCount,
          unchangedCount: unchangedCount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
