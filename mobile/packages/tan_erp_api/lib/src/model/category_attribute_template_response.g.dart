// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'category_attribute_template_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CategoryAttributeTemplateResponse
    extends CategoryAttributeTemplateResponse {
  @override
  final String? categoryId;
  @override
  final String? categoryCode;
  @override
  final LocalizedTextResponse? categoryName;
  @override
  final BuiltList<CategoryAttributeTemplateDto>? templates;

  factory _$CategoryAttributeTemplateResponse(
          [void Function(CategoryAttributeTemplateResponseBuilder)? updates]) =>
      (CategoryAttributeTemplateResponseBuilder()..update(updates))._build();

  _$CategoryAttributeTemplateResponse._(
      {this.categoryId, this.categoryCode, this.categoryName, this.templates})
      : super._();
  @override
  CategoryAttributeTemplateResponse rebuild(
          void Function(CategoryAttributeTemplateResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CategoryAttributeTemplateResponseBuilder toBuilder() =>
      CategoryAttributeTemplateResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CategoryAttributeTemplateResponse &&
        categoryId == other.categoryId &&
        categoryCode == other.categoryCode &&
        categoryName == other.categoryName &&
        templates == other.templates;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, categoryId.hashCode);
    _$hash = $jc(_$hash, categoryCode.hashCode);
    _$hash = $jc(_$hash, categoryName.hashCode);
    _$hash = $jc(_$hash, templates.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CategoryAttributeTemplateResponse')
          ..add('categoryId', categoryId)
          ..add('categoryCode', categoryCode)
          ..add('categoryName', categoryName)
          ..add('templates', templates))
        .toString();
  }
}

class CategoryAttributeTemplateResponseBuilder
    implements
        Builder<CategoryAttributeTemplateResponse,
            CategoryAttributeTemplateResponseBuilder> {
  _$CategoryAttributeTemplateResponse? _$v;

  String? _categoryId;
  String? get categoryId => _$this._categoryId;
  set categoryId(String? categoryId) => _$this._categoryId = categoryId;

  String? _categoryCode;
  String? get categoryCode => _$this._categoryCode;
  set categoryCode(String? categoryCode) => _$this._categoryCode = categoryCode;

  LocalizedTextResponseBuilder? _categoryName;
  LocalizedTextResponseBuilder get categoryName =>
      _$this._categoryName ??= LocalizedTextResponseBuilder();
  set categoryName(LocalizedTextResponseBuilder? categoryName) =>
      _$this._categoryName = categoryName;

  ListBuilder<CategoryAttributeTemplateDto>? _templates;
  ListBuilder<CategoryAttributeTemplateDto> get templates =>
      _$this._templates ??= ListBuilder<CategoryAttributeTemplateDto>();
  set templates(ListBuilder<CategoryAttributeTemplateDto>? templates) =>
      _$this._templates = templates;

  CategoryAttributeTemplateResponseBuilder() {
    CategoryAttributeTemplateResponse._defaults(this);
  }

  CategoryAttributeTemplateResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _categoryId = $v.categoryId;
      _categoryCode = $v.categoryCode;
      _categoryName = $v.categoryName?.toBuilder();
      _templates = $v.templates?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CategoryAttributeTemplateResponse other) {
    _$v = other as _$CategoryAttributeTemplateResponse;
  }

  @override
  void update(
      void Function(CategoryAttributeTemplateResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CategoryAttributeTemplateResponse build() => _build();

  _$CategoryAttributeTemplateResponse _build() {
    _$CategoryAttributeTemplateResponse _$result;
    try {
      _$result = _$v ??
          _$CategoryAttributeTemplateResponse._(
            categoryId: categoryId,
            categoryCode: categoryCode,
            categoryName: _categoryName?.build(),
            templates: _templates?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'categoryName';
        _categoryName?.build();
        _$failedField = 'templates';
        _templates?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CategoryAttributeTemplateResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
