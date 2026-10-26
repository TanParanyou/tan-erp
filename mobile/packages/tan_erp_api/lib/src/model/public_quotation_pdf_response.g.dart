// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_quotation_pdf_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicQuotationPdfResponse extends PublicQuotationPdfResponse {
  @override
  final String? sha256;
  @override
  final int? byteCount;

  factory _$PublicQuotationPdfResponse(
          [void Function(PublicQuotationPdfResponseBuilder)? updates]) =>
      (PublicQuotationPdfResponseBuilder()..update(updates))._build();

  _$PublicQuotationPdfResponse._({this.sha256, this.byteCount}) : super._();
  @override
  PublicQuotationPdfResponse rebuild(
          void Function(PublicQuotationPdfResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicQuotationPdfResponseBuilder toBuilder() =>
      PublicQuotationPdfResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicQuotationPdfResponse &&
        sha256 == other.sha256 &&
        byteCount == other.byteCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sha256.hashCode);
    _$hash = $jc(_$hash, byteCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicQuotationPdfResponse')
          ..add('sha256', sha256)
          ..add('byteCount', byteCount))
        .toString();
  }
}

class PublicQuotationPdfResponseBuilder
    implements
        Builder<PublicQuotationPdfResponse, PublicQuotationPdfResponseBuilder> {
  _$PublicQuotationPdfResponse? _$v;

  String? _sha256;
  String? get sha256 => _$this._sha256;
  set sha256(String? sha256) => _$this._sha256 = sha256;

  int? _byteCount;
  int? get byteCount => _$this._byteCount;
  set byteCount(int? byteCount) => _$this._byteCount = byteCount;

  PublicQuotationPdfResponseBuilder() {
    PublicQuotationPdfResponse._defaults(this);
  }

  PublicQuotationPdfResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sha256 = $v.sha256;
      _byteCount = $v.byteCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicQuotationPdfResponse other) {
    _$v = other as _$PublicQuotationPdfResponse;
  }

  @override
  void update(void Function(PublicQuotationPdfResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicQuotationPdfResponse build() => _build();

  _$PublicQuotationPdfResponse _build() {
    final _$result = _$v ??
        _$PublicQuotationPdfResponse._(
          sha256: sha256,
          byteCount: byteCount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
