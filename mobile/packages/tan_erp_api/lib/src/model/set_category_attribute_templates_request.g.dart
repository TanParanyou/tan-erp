// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'set_category_attribute_templates_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SetCategoryAttributeTemplatesRequest
    extends SetCategoryAttributeTemplatesRequest {
  @override
  final BuiltList<CategoryAttributeTemplateDto>? templates;

  factory _$SetCategoryAttributeTemplatesRequest(
          [void Function(SetCategoryAttributeTemplatesRequestBuilder)?
              updates]) =>
      (SetCategoryAttributeTemplatesRequestBuilder()..update(updates))._build();

  _$SetCategoryAttributeTemplatesRequest._({this.templates}) : super._();
  @override
  SetCategoryAttributeTemplatesRequest rebuild(
          void Function(SetCategoryAttributeTemplatesRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SetCategoryAttributeTemplatesRequestBuilder toBuilder() =>
      SetCategoryAttributeTemplatesRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SetCategoryAttributeTemplatesRequest &&
        templates == other.templates;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, templates.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SetCategoryAttributeTemplatesRequest')
          ..add('templates', templates))
        .toString();
  }
}

class SetCategoryAttributeTemplatesRequestBuilder
    implements
        Builder<SetCategoryAttributeTemplatesRequest,
            SetCategoryAttributeTemplatesRequestBuilder> {
  _$SetCategoryAttributeTemplatesRequest? _$v;

  ListBuilder<CategoryAttributeTemplateDto>? _templates;
  ListBuilder<CategoryAttributeTemplateDto> get templates =>
      _$this._templates ??= ListBuilder<CategoryAttributeTemplateDto>();
  set templates(ListBuilder<CategoryAttributeTemplateDto>? templates) =>
      _$this._templates = templates;

  SetCategoryAttributeTemplatesRequestBuilder() {
    SetCategoryAttributeTemplatesRequest._defaults(this);
  }

  SetCategoryAttributeTemplatesRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _templates = $v.templates?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SetCategoryAttributeTemplatesRequest other) {
    _$v = other as _$SetCategoryAttributeTemplatesRequest;
  }

  @override
  void update(
      void Function(SetCategoryAttributeTemplatesRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SetCategoryAttributeTemplatesRequest build() => _build();

  _$SetCategoryAttributeTemplatesRequest _build() {
    _$SetCategoryAttributeTemplatesRequest _$result;
    try {
      _$result = _$v ??
          _$SetCategoryAttributeTemplatesRequest._(
            templates: _templates?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'templates';
        _templates?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SetCategoryAttributeTemplatesRequest',
            _$failedField,
            e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
