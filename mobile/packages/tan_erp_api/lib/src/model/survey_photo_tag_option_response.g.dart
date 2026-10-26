// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_photo_tag_option_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyPhotoTagOptionResponse extends SurveyPhotoTagOptionResponse {
  @override
  final String? code;
  @override
  final SurveyLocalizedLabel? name;
  @override
  final String? description;
  @override
  final int? sortOrder;

  factory _$SurveyPhotoTagOptionResponse(
          [void Function(SurveyPhotoTagOptionResponseBuilder)? updates]) =>
      (SurveyPhotoTagOptionResponseBuilder()..update(updates))._build();

  _$SurveyPhotoTagOptionResponse._(
      {this.code, this.name, this.description, this.sortOrder})
      : super._();
  @override
  SurveyPhotoTagOptionResponse rebuild(
          void Function(SurveyPhotoTagOptionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyPhotoTagOptionResponseBuilder toBuilder() =>
      SurveyPhotoTagOptionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyPhotoTagOptionResponse &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyPhotoTagOptionResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class SurveyPhotoTagOptionResponseBuilder
    implements
        Builder<SurveyPhotoTagOptionResponse,
            SurveyPhotoTagOptionResponseBuilder> {
  _$SurveyPhotoTagOptionResponse? _$v;

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

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  SurveyPhotoTagOptionResponseBuilder() {
    SurveyPhotoTagOptionResponse._defaults(this);
  }

  SurveyPhotoTagOptionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description;
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyPhotoTagOptionResponse other) {
    _$v = other as _$SurveyPhotoTagOptionResponse;
  }

  @override
  void update(void Function(SurveyPhotoTagOptionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyPhotoTagOptionResponse build() => _build();

  _$SurveyPhotoTagOptionResponse _build() {
    _$SurveyPhotoTagOptionResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyPhotoTagOptionResponse._(
            code: code,
            name: _name?.build(),
            description: description,
            sortOrder: sortOrder,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SurveyPhotoTagOptionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
