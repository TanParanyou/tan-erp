// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'category_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CategorySummaryResponse extends CategorySummaryResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final String? parentCategoryId;
  @override
  final String? imageFileId;

  factory _$CategorySummaryResponse(
          [void Function(CategorySummaryResponseBuilder)? updates]) =>
      (CategorySummaryResponseBuilder()..update(updates))._build();

  _$CategorySummaryResponse._(
      {this.id, this.code, this.name, this.parentCategoryId, this.imageFileId})
      : super._();
  @override
  CategorySummaryResponse rebuild(
          void Function(CategorySummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CategorySummaryResponseBuilder toBuilder() =>
      CategorySummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CategorySummaryResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        parentCategoryId == other.parentCategoryId &&
        imageFileId == other.imageFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, parentCategoryId.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CategorySummaryResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('parentCategoryId', parentCategoryId)
          ..add('imageFileId', imageFileId))
        .toString();
  }
}

class CategorySummaryResponseBuilder
    implements
        Builder<CategorySummaryResponse, CategorySummaryResponseBuilder> {
  _$CategorySummaryResponse? _$v;

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

  String? _parentCategoryId;
  String? get parentCategoryId => _$this._parentCategoryId;
  set parentCategoryId(String? parentCategoryId) =>
      _$this._parentCategoryId = parentCategoryId;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  CategorySummaryResponseBuilder() {
    CategorySummaryResponse._defaults(this);
  }

  CategorySummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _parentCategoryId = $v.parentCategoryId;
      _imageFileId = $v.imageFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CategorySummaryResponse other) {
    _$v = other as _$CategorySummaryResponse;
  }

  @override
  void update(void Function(CategorySummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CategorySummaryResponse build() => _build();

  _$CategorySummaryResponse _build() {
    _$CategorySummaryResponse _$result;
    try {
      _$result = _$v ??
          _$CategorySummaryResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            parentCategoryId: parentCategoryId,
            imageFileId: imageFileId,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CategorySummaryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
