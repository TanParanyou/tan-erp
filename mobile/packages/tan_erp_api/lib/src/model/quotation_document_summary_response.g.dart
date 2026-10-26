// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_document_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationDocumentSummaryResponse
    extends QuotationDocumentSummaryResponse {
  @override
  final String? sha256;
  @override
  final int? byteCount;
  @override
  final String? rendererVersion;
  @override
  final DateTime? renderedAtUtc;

  factory _$QuotationDocumentSummaryResponse(
          [void Function(QuotationDocumentSummaryResponseBuilder)? updates]) =>
      (QuotationDocumentSummaryResponseBuilder()..update(updates))._build();

  _$QuotationDocumentSummaryResponse._(
      {this.sha256, this.byteCount, this.rendererVersion, this.renderedAtUtc})
      : super._();
  @override
  QuotationDocumentSummaryResponse rebuild(
          void Function(QuotationDocumentSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationDocumentSummaryResponseBuilder toBuilder() =>
      QuotationDocumentSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationDocumentSummaryResponse &&
        sha256 == other.sha256 &&
        byteCount == other.byteCount &&
        rendererVersion == other.rendererVersion &&
        renderedAtUtc == other.renderedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sha256.hashCode);
    _$hash = $jc(_$hash, byteCount.hashCode);
    _$hash = $jc(_$hash, rendererVersion.hashCode);
    _$hash = $jc(_$hash, renderedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationDocumentSummaryResponse')
          ..add('sha256', sha256)
          ..add('byteCount', byteCount)
          ..add('rendererVersion', rendererVersion)
          ..add('renderedAtUtc', renderedAtUtc))
        .toString();
  }
}

class QuotationDocumentSummaryResponseBuilder
    implements
        Builder<QuotationDocumentSummaryResponse,
            QuotationDocumentSummaryResponseBuilder> {
  _$QuotationDocumentSummaryResponse? _$v;

  String? _sha256;
  String? get sha256 => _$this._sha256;
  set sha256(String? sha256) => _$this._sha256 = sha256;

  int? _byteCount;
  int? get byteCount => _$this._byteCount;
  set byteCount(int? byteCount) => _$this._byteCount = byteCount;

  String? _rendererVersion;
  String? get rendererVersion => _$this._rendererVersion;
  set rendererVersion(String? rendererVersion) =>
      _$this._rendererVersion = rendererVersion;

  DateTime? _renderedAtUtc;
  DateTime? get renderedAtUtc => _$this._renderedAtUtc;
  set renderedAtUtc(DateTime? renderedAtUtc) =>
      _$this._renderedAtUtc = renderedAtUtc;

  QuotationDocumentSummaryResponseBuilder() {
    QuotationDocumentSummaryResponse._defaults(this);
  }

  QuotationDocumentSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sha256 = $v.sha256;
      _byteCount = $v.byteCount;
      _rendererVersion = $v.rendererVersion;
      _renderedAtUtc = $v.renderedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationDocumentSummaryResponse other) {
    _$v = other as _$QuotationDocumentSummaryResponse;
  }

  @override
  void update(void Function(QuotationDocumentSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationDocumentSummaryResponse build() => _build();

  _$QuotationDocumentSummaryResponse _build() {
    final _$result = _$v ??
        _$QuotationDocumentSummaryResponse._(
          sha256: sha256,
          byteCount: byteCount,
          rendererVersion: rendererVersion,
          renderedAtUtc: renderedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
