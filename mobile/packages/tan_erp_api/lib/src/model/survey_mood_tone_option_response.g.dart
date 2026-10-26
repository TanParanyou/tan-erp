// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_mood_tone_option_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyMoodToneOptionResponse extends SurveyMoodToneOptionResponse {
  @override
  final String? code;
  @override
  final SurveyLocalizedLabel? name;
  @override
  final String? description;
  @override
  final BuiltList<String>? colorPalette;
  @override
  final int? sortOrder;

  factory _$SurveyMoodToneOptionResponse(
          [void Function(SurveyMoodToneOptionResponseBuilder)? updates]) =>
      (SurveyMoodToneOptionResponseBuilder()..update(updates))._build();

  _$SurveyMoodToneOptionResponse._(
      {this.code,
      this.name,
      this.description,
      this.colorPalette,
      this.sortOrder})
      : super._();
  @override
  SurveyMoodToneOptionResponse rebuild(
          void Function(SurveyMoodToneOptionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyMoodToneOptionResponseBuilder toBuilder() =>
      SurveyMoodToneOptionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyMoodToneOptionResponse &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        colorPalette == other.colorPalette &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, colorPalette.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyMoodToneOptionResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('colorPalette', colorPalette)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class SurveyMoodToneOptionResponseBuilder
    implements
        Builder<SurveyMoodToneOptionResponse,
            SurveyMoodToneOptionResponseBuilder> {
  _$SurveyMoodToneOptionResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  SurveyLocalizedLabelBuilder? _name;
  SurveyLocalizedLabelBuilder get name =>
      _$this._name ??= SurveyLocalizedLabelBuilder();
  set name(SurveyLocalizedLabelBuilder? name) => _$this._name = name;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  ListBuilder<String>? _colorPalette;
  ListBuilder<String> get colorPalette =>
      _$this._colorPalette ??= ListBuilder<String>();
  set colorPalette(ListBuilder<String>? colorPalette) =>
      _$this._colorPalette = colorPalette;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  SurveyMoodToneOptionResponseBuilder() {
    SurveyMoodToneOptionResponse._defaults(this);
  }

  SurveyMoodToneOptionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description;
      _colorPalette = $v.colorPalette?.toBuilder();
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyMoodToneOptionResponse other) {
    _$v = other as _$SurveyMoodToneOptionResponse;
  }

  @override
  void update(void Function(SurveyMoodToneOptionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyMoodToneOptionResponse build() => _build();

  _$SurveyMoodToneOptionResponse _build() {
    _$SurveyMoodToneOptionResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyMoodToneOptionResponse._(
            code: code,
            name: _name?.build(),
            description: description,
            colorPalette: _colorPalette?.build(),
            sortOrder: sortOrder,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();

        _$failedField = 'colorPalette';
        _colorPalette?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SurveyMoodToneOptionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
