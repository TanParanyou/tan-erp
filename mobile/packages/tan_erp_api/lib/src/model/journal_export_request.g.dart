// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'journal_export_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$JournalExportRequest extends JournalExportRequest {
  @override
  final int? batchSize;

  factory _$JournalExportRequest(
          [void Function(JournalExportRequestBuilder)? updates]) =>
      (JournalExportRequestBuilder()..update(updates))._build();

  _$JournalExportRequest._({this.batchSize}) : super._();
  @override
  JournalExportRequest rebuild(
          void Function(JournalExportRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  JournalExportRequestBuilder toBuilder() =>
      JournalExportRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is JournalExportRequest && batchSize == other.batchSize;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, batchSize.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'JournalExportRequest')
          ..add('batchSize', batchSize))
        .toString();
  }
}

class JournalExportRequestBuilder
    implements Builder<JournalExportRequest, JournalExportRequestBuilder> {
  _$JournalExportRequest? _$v;

  int? _batchSize;
  int? get batchSize => _$this._batchSize;
  set batchSize(int? batchSize) => _$this._batchSize = batchSize;

  JournalExportRequestBuilder() {
    JournalExportRequest._defaults(this);
  }

  JournalExportRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _batchSize = $v.batchSize;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(JournalExportRequest other) {
    _$v = other as _$JournalExportRequest;
  }

  @override
  void update(void Function(JournalExportRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  JournalExportRequest build() => _build();

  _$JournalExportRequest _build() {
    final _$result = _$v ??
        _$JournalExportRequest._(
          batchSize: batchSize,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
