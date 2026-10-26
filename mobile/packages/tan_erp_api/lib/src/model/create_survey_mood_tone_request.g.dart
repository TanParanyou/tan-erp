// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_survey_mood_tone_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateSurveyMoodToneRequest extends CreateSurveyMoodToneRequest {
  @override
  final String? code;
  @override
  final SurveyLocalizedNameContract? name;
  @override
  final SurveyLocalizedNameContract? description;
  @override
  final BuiltList<String>? colorPalette;
  @override
  final int? sortOrder;

  factory _$CreateSurveyMoodToneRequest(
          [void Function(CreateSurveyMoodToneRequestBuilder)? updates]) =>
      (CreateSurveyMoodToneRequestBuilder()..update(updates))._build();

  _$CreateSurveyMoodToneRequest._(
      {this.code,
      this.name,
      this.description,
      this.colorPalette,
      this.sortOrder})
      : super._();
  @override
  CreateSurveyMoodToneRequest rebuild(
          void Function(CreateSurveyMoodToneRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateSurveyMoodToneRequestBuilder toBuilder() =>
      CreateSurveyMoodToneRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateSurveyMoodToneRequest &&
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
    return (newBuiltValueToStringHelper(r'CreateSurveyMoodToneRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('colorPalette', colorPalette)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class CreateSurveyMoodToneRequestBuilder
    implements
        Builder<CreateSurveyMoodToneRequest,
            CreateSurveyMoodToneRequestBuilder> {
  _$CreateSurveyMoodToneRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  SurveyLocalizedNameContractBuilder? _name;
  SurveyLocalizedNameContractBuilder get name =>
      _$this._name ??= SurveyLocalizedNameContractBuilder();
  set name(SurveyLocalizedNameContractBuilder? name) => _$this._name = name;

  SurveyLocalizedNameContractBuilder? _description;
  SurveyLocalizedNameContractBuilder get description =>
      _$this._description ??= SurveyLocalizedNameContractBuilder();
  set description(SurveyLocalizedNameContractBuilder? description) =>
      _$this._description = description;

  ListBuilder<String>? _colorPalette;
  ListBuilder<String> get colorPalette =>
      _$this._colorPalette ??= ListBuilder<String>();
  set colorPalette(ListBuilder<String>? colorPalette) =>
      _$this._colorPalette = colorPalette;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  CreateSurveyMoodToneRequestBuilder() {
    CreateSurveyMoodToneRequest._defaults(this);
  }

  CreateSurveyMoodToneRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _colorPalette = $v.colorPalette?.toBuilder();
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateSurveyMoodToneRequest other) {
    _$v = other as _$CreateSurveyMoodToneRequest;
  }

  @override
  void update(void Function(CreateSurveyMoodToneRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateSurveyMoodToneRequest build() => _build();

  _$CreateSurveyMoodToneRequest _build() {
    _$CreateSurveyMoodToneRequest _$result;
    try {
      _$result = _$v ??
          _$CreateSurveyMoodToneRequest._(
            code: code,
            name: _name?.build(),
            description: _description?.build(),
            colorPalette: _colorPalette?.build(),
            sortOrder: sortOrder,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
        _$failedField = 'description';
        _description?.build();
        _$failedField = 'colorPalette';
        _colorPalette?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateSurveyMoodToneRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
