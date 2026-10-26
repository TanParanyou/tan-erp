// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'brand_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BrandSummaryResponse extends BrandSummaryResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final String? imageFileId;

  factory _$BrandSummaryResponse(
          [void Function(BrandSummaryResponseBuilder)? updates]) =>
      (BrandSummaryResponseBuilder()..update(updates))._build();

  _$BrandSummaryResponse._({this.id, this.code, this.name, this.imageFileId})
      : super._();
  @override
  BrandSummaryResponse rebuild(
          void Function(BrandSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BrandSummaryResponseBuilder toBuilder() =>
      BrandSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BrandSummaryResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        imageFileId == other.imageFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BrandSummaryResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('imageFileId', imageFileId))
        .toString();
  }
}

class BrandSummaryResponseBuilder
    implements Builder<BrandSummaryResponse, BrandSummaryResponseBuilder> {
  _$BrandSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  BrandSummaryResponseBuilder() {
    BrandSummaryResponse._defaults(this);
  }

  BrandSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _imageFileId = $v.imageFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BrandSummaryResponse other) {
    _$v = other as _$BrandSummaryResponse;
  }

  @override
  void update(void Function(BrandSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BrandSummaryResponse build() => _build();

  _$BrandSummaryResponse _build() {
    _$BrandSummaryResponse _$result;
    try {
      _$result = _$v ??
          _$BrandSummaryResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            imageFileId: imageFileId,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BrandSummaryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
