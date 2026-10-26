// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_primary_image_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CatalogPrimaryImageResponse extends CatalogPrimaryImageResponse {
  @override
  final String? fileId;
  @override
  final LocalizedTextResponse? altText;

  factory _$CatalogPrimaryImageResponse(
          [void Function(CatalogPrimaryImageResponseBuilder)? updates]) =>
      (CatalogPrimaryImageResponseBuilder()..update(updates))._build();

  _$CatalogPrimaryImageResponse._({this.fileId, this.altText}) : super._();
  @override
  CatalogPrimaryImageResponse rebuild(
          void Function(CatalogPrimaryImageResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CatalogPrimaryImageResponseBuilder toBuilder() =>
      CatalogPrimaryImageResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CatalogPrimaryImageResponse &&
        fileId == other.fileId &&
        altText == other.altText;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, altText.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CatalogPrimaryImageResponse')
          ..add('fileId', fileId)
          ..add('altText', altText))
        .toString();
  }
}

class CatalogPrimaryImageResponseBuilder
    implements
        Builder<CatalogPrimaryImageResponse,
            CatalogPrimaryImageResponseBuilder> {
  _$CatalogPrimaryImageResponse? _$v;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  LocalizedTextResponseBuilder? _altText;
  LocalizedTextResponseBuilder get altText =>
      _$this._altText ??= LocalizedTextResponseBuilder();
  set altText(LocalizedTextResponseBuilder? altText) =>
      _$this._altText = altText;

  CatalogPrimaryImageResponseBuilder() {
    CatalogPrimaryImageResponse._defaults(this);
  }

  CatalogPrimaryImageResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fileId = $v.fileId;
      _altText = $v.altText?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CatalogPrimaryImageResponse other) {
    _$v = other as _$CatalogPrimaryImageResponse;
  }

  @override
  void update(void Function(CatalogPrimaryImageResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CatalogPrimaryImageResponse build() => _build();

  _$CatalogPrimaryImageResponse _build() {
    _$CatalogPrimaryImageResponse _$result;
    try {
      _$result = _$v ??
          _$CatalogPrimaryImageResponse._(
            fileId: fileId,
            altText: _altText?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'altText';
        _altText?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CatalogPrimaryImageResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
