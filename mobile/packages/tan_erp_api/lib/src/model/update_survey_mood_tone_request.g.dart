// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_survey_mood_tone_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateSurveyMoodToneRequest extends UpdateSurveyMoodToneRequest {
  @override
  final SurveyLocalizedNameContract? name;
  @override
  final SurveyLocalizedNameContract? description;
  @override
  final BuiltList<String>? colorPalette;
  @override
  final int? sortOrder;
  @override
  final bool? isActive;

  factory _$UpdateSurveyMoodToneRequest(
          [void Function(UpdateSurveyMoodToneRequestBuilder)? updates]) =>
      (UpdateSurveyMoodToneRequestBuilder()..update(updates))._build();

  _$UpdateSurveyMoodToneRequest._(
      {this.name,
      this.description,
      this.colorPalette,
      this.sortOrder,
      this.isActive})
      : super._();
  @override
  UpdateSurveyMoodToneRequest rebuild(
          void Function(UpdateSurveyMoodToneRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateSurveyMoodToneRequestBuilder toBuilder() =>
      UpdateSurveyMoodToneRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateSurveyMoodToneRequest &&
        name == other.name &&
        description == other.description &&
        colorPalette == other.colorPalette &&
        sortOrder == other.sortOrder &&
        isActive == other.isActive;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, colorPalette.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateSurveyMoodToneRequest')
          ..add('name', name)
          ..add('description', description)
          ..add('colorPalette', colorPalette)
          ..add('sortOrder', sortOrder)
          ..add('isActive', isActive))
        .toString();
  }
}

class UpdateSurveyMoodToneRequestBuilder
    implements
        Builder<UpdateSurveyMoodToneRequest,
            UpdateSurveyMoodToneRequestBuilder> {
  _$UpdateSurveyMoodToneRequest? _$v;

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

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  UpdateSurveyMoodToneRequestBuilder() {
    UpdateSurveyMoodToneRequest._defaults(this);
  }

  UpdateSurveyMoodToneRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _colorPalette = $v.colorPalette?.toBuilder();
      _sortOrder = $v.sortOrder;
      _isActive = $v.isActive;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateSurveyMoodToneRequest other) {
    _$v = other as _$UpdateSurveyMoodToneRequest;
  }

  @override
  void update(void Function(UpdateSurveyMoodToneRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateSurveyMoodToneRequest build() => _build();

  _$UpdateSurveyMoodToneRequest _build() {
    _$UpdateSurveyMoodToneRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateSurveyMoodToneRequest._(
            name: _name?.build(),
            description: _description?.build(),
            colorPalette: _colorPalette?.build(),
            sortOrder: sortOrder,
            isActive: isActive,
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
            r'UpdateSurveyMoodToneRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
