// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'category_attribute_template_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CategoryAttributeTemplateDto extends CategoryAttributeTemplateDto {
  @override
  final String? key;
  @override
  final LocalizedTextInput? name;
  @override
  final String? dataType;
  @override
  final String? unit;
  @override
  final bool? isRequired;
  @override
  final String? defaultValue;
  @override
  final BuiltList<CategoryAttributeOptionDto>? options;

  factory _$CategoryAttributeTemplateDto(
          [void Function(CategoryAttributeTemplateDtoBuilder)? updates]) =>
      (CategoryAttributeTemplateDtoBuilder()..update(updates))._build();

  _$CategoryAttributeTemplateDto._(
      {this.key,
      this.name,
      this.dataType,
      this.unit,
      this.isRequired,
      this.defaultValue,
      this.options})
      : super._();
  @override
  CategoryAttributeTemplateDto rebuild(
          void Function(CategoryAttributeTemplateDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CategoryAttributeTemplateDtoBuilder toBuilder() =>
      CategoryAttributeTemplateDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CategoryAttributeTemplateDto &&
        key == other.key &&
        name == other.name &&
        dataType == other.dataType &&
        unit == other.unit &&
        isRequired == other.isRequired &&
        defaultValue == other.defaultValue &&
        options == other.options;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, key.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, dataType.hashCode);
    _$hash = $jc(_$hash, unit.hashCode);
    _$hash = $jc(_$hash, isRequired.hashCode);
    _$hash = $jc(_$hash, defaultValue.hashCode);
    _$hash = $jc(_$hash, options.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CategoryAttributeTemplateDto')
          ..add('key', key)
          ..add('name', name)
          ..add('dataType', dataType)
          ..add('unit', unit)
          ..add('isRequired', isRequired)
          ..add('defaultValue', defaultValue)
          ..add('options', options))
        .toString();
  }
}

class CategoryAttributeTemplateDtoBuilder
    implements
        Builder<CategoryAttributeTemplateDto,
            CategoryAttributeTemplateDtoBuilder> {
  _$CategoryAttributeTemplateDto? _$v;

  String? _key;
  String? get key => _$this._key;
  set key(String? key) => _$this._key = key;

  LocalizedTextInputBuilder? _name;
  LocalizedTextInputBuilder get name =>
      _$this._name ??= LocalizedTextInputBuilder();
  set name(LocalizedTextInputBuilder? name) => _$this._name = name;

  String? _dataType;
  String? get dataType => _$this._dataType;
  set dataType(String? dataType) => _$this._dataType = dataType;

  String? _unit;
  String? get unit => _$this._unit;
  set unit(String? unit) => _$this._unit = unit;

  bool? _isRequired;
  bool? get isRequired => _$this._isRequired;
  set isRequired(bool? isRequired) => _$this._isRequired = isRequired;

  String? _defaultValue;
  String? get defaultValue => _$this._defaultValue;
  set defaultValue(String? defaultValue) => _$this._defaultValue = defaultValue;

  ListBuilder<CategoryAttributeOptionDto>? _options;
  ListBuilder<CategoryAttributeOptionDto> get options =>
      _$this._options ??= ListBuilder<CategoryAttributeOptionDto>();
  set options(ListBuilder<CategoryAttributeOptionDto>? options) =>
      _$this._options = options;

  CategoryAttributeTemplateDtoBuilder() {
    CategoryAttributeTemplateDto._defaults(this);
  }

  CategoryAttributeTemplateDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _key = $v.key;
      _name = $v.name?.toBuilder();
      _dataType = $v.dataType;
      _unit = $v.unit;
      _isRequired = $v.isRequired;
      _defaultValue = $v.defaultValue;
      _options = $v.options?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CategoryAttributeTemplateDto other) {
    _$v = other as _$CategoryAttributeTemplateDto;
  }

  @override
  void update(void Function(CategoryAttributeTemplateDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CategoryAttributeTemplateDto build() => _build();

  _$CategoryAttributeTemplateDto _build() {
    _$CategoryAttributeTemplateDto _$result;
    try {
      _$result = _$v ??
          _$CategoryAttributeTemplateDto._(
            key: key,
            name: _name?.build(),
            dataType: dataType,
            unit: unit,
            isRequired: isRequired,
            defaultValue: defaultValue,
            options: _options?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();

        _$failedField = 'options';
        _options?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CategoryAttributeTemplateDto', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
