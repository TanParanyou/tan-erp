// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'category_attribute_option_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CategoryAttributeOptionDto extends CategoryAttributeOptionDto {
  @override
  final String? value;
  @override
  final LocalizedTextInput? label;

  factory _$CategoryAttributeOptionDto(
          [void Function(CategoryAttributeOptionDtoBuilder)? updates]) =>
      (CategoryAttributeOptionDtoBuilder()..update(updates))._build();

  _$CategoryAttributeOptionDto._({this.value, this.label}) : super._();
  @override
  CategoryAttributeOptionDto rebuild(
          void Function(CategoryAttributeOptionDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CategoryAttributeOptionDtoBuilder toBuilder() =>
      CategoryAttributeOptionDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CategoryAttributeOptionDto &&
        value == other.value &&
        label == other.label;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, value.hashCode);
    _$hash = $jc(_$hash, label.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CategoryAttributeOptionDto')
          ..add('value', value)
          ..add('label', label))
        .toString();
  }
}

class CategoryAttributeOptionDtoBuilder
    implements
        Builder<CategoryAttributeOptionDto, CategoryAttributeOptionDtoBuilder> {
  _$CategoryAttributeOptionDto? _$v;

  String? _value;
  String? get value => _$this._value;
  set value(String? value) => _$this._value = value;

  LocalizedTextInputBuilder? _label;
  LocalizedTextInputBuilder get label =>
      _$this._label ??= LocalizedTextInputBuilder();
  set label(LocalizedTextInputBuilder? label) => _$this._label = label;

  CategoryAttributeOptionDtoBuilder() {
    CategoryAttributeOptionDto._defaults(this);
  }

  CategoryAttributeOptionDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _value = $v.value;
      _label = $v.label?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CategoryAttributeOptionDto other) {
    _$v = other as _$CategoryAttributeOptionDto;
  }

  @override
  void update(void Function(CategoryAttributeOptionDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CategoryAttributeOptionDto build() => _build();

  _$CategoryAttributeOptionDto _build() {
    _$CategoryAttributeOptionDto _$result;
    try {
      _$result = _$v ??
          _$CategoryAttributeOptionDto._(
            value: value,
            label: _label?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'label';
        _label?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CategoryAttributeOptionDto', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
