// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_mood_tone_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyMoodToneResponse extends SurveyMoodToneResponse {
  @override
  final String? id;
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
  @override
  final bool? isActive;
  @override
  final DateTime? createdAtUtc;

  factory _$SurveyMoodToneResponse(
          [void Function(SurveyMoodToneResponseBuilder)? updates]) =>
      (SurveyMoodToneResponseBuilder()..update(updates))._build();

  _$SurveyMoodToneResponse._(
      {this.id,
      this.code,
      this.name,
      this.description,
      this.colorPalette,
      this.sortOrder,
      this.isActive,
      this.createdAtUtc})
      : super._();
  @override
  SurveyMoodToneResponse rebuild(
          void Function(SurveyMoodToneResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyMoodToneResponseBuilder toBuilder() =>
      SurveyMoodToneResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyMoodToneResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        colorPalette == other.colorPalette &&
        sortOrder == other.sortOrder &&
        isActive == other.isActive &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, colorPalette.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyMoodToneResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('colorPalette', colorPalette)
          ..add('sortOrder', sortOrder)
          ..add('isActive', isActive)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class SurveyMoodToneResponseBuilder
    implements Builder<SurveyMoodToneResponse, SurveyMoodToneResponseBuilder> {
  _$SurveyMoodToneResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  SurveyMoodToneResponseBuilder() {
    SurveyMoodToneResponse._defaults(this);
  }

  SurveyMoodToneResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _colorPalette = $v.colorPalette?.toBuilder();
      _sortOrder = $v.sortOrder;
      _isActive = $v.isActive;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyMoodToneResponse other) {
    _$v = other as _$SurveyMoodToneResponse;
  }

  @override
  void update(void Function(SurveyMoodToneResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyMoodToneResponse build() => _build();

  _$SurveyMoodToneResponse _build() {
    _$SurveyMoodToneResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyMoodToneResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            description: _description?.build(),
            colorPalette: _colorPalette?.build(),
            sortOrder: sortOrder,
            isActive: isActive,
            createdAtUtc: createdAtUtc,
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
            r'SurveyMoodToneResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
