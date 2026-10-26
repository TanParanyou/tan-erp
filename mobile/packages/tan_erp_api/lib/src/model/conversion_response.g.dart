// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'conversion_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ConversionResponse extends ConversionResponse {
  @override
  final String? id;
  @override
  final int? sourceVersion;
  @override
  final String? officialEstimateId;
  @override
  final DateTime? createdAtUtc;
  @override
  final int? mappedLineCount;

  factory _$ConversionResponse(
          [void Function(ConversionResponseBuilder)? updates]) =>
      (ConversionResponseBuilder()..update(updates))._build();

  _$ConversionResponse._(
      {this.id,
      this.sourceVersion,
      this.officialEstimateId,
      this.createdAtUtc,
      this.mappedLineCount})
      : super._();
  @override
  ConversionResponse rebuild(
          void Function(ConversionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ConversionResponseBuilder toBuilder() =>
      ConversionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ConversionResponse &&
        id == other.id &&
        sourceVersion == other.sourceVersion &&
        officialEstimateId == other.officialEstimateId &&
        createdAtUtc == other.createdAtUtc &&
        mappedLineCount == other.mappedLineCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, sourceVersion.hashCode);
    _$hash = $jc(_$hash, officialEstimateId.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, mappedLineCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ConversionResponse')
          ..add('id', id)
          ..add('sourceVersion', sourceVersion)
          ..add('officialEstimateId', officialEstimateId)
          ..add('createdAtUtc', createdAtUtc)
          ..add('mappedLineCount', mappedLineCount))
        .toString();
  }
}

class ConversionResponseBuilder
    implements Builder<ConversionResponse, ConversionResponseBuilder> {
  _$ConversionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _sourceVersion;
  int? get sourceVersion => _$this._sourceVersion;
  set sourceVersion(int? sourceVersion) =>
      _$this._sourceVersion = sourceVersion;

  String? _officialEstimateId;
  String? get officialEstimateId => _$this._officialEstimateId;
  set officialEstimateId(String? officialEstimateId) =>
      _$this._officialEstimateId = officialEstimateId;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  int? _mappedLineCount;
  int? get mappedLineCount => _$this._mappedLineCount;
  set mappedLineCount(int? mappedLineCount) =>
      _$this._mappedLineCount = mappedLineCount;

  ConversionResponseBuilder() {
    ConversionResponse._defaults(this);
  }

  ConversionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _sourceVersion = $v.sourceVersion;
      _officialEstimateId = $v.officialEstimateId;
      _createdAtUtc = $v.createdAtUtc;
      _mappedLineCount = $v.mappedLineCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ConversionResponse other) {
    _$v = other as _$ConversionResponse;
  }

  @override
  void update(void Function(ConversionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ConversionResponse build() => _build();

  _$ConversionResponse _build() {
    final _$result = _$v ??
        _$ConversionResponse._(
          id: id,
          sourceVersion: sourceVersion,
          officialEstimateId: officialEstimateId,
          createdAtUtc: createdAtUtc,
          mappedLineCount: mappedLineCount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
