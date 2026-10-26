// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_category_option_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyCategoryOptionResponse extends SurveyCategoryOptionResponse {
  @override
  final String? code;
  @override
  final SurveyLocalizedLabel? name;
  @override
  final String? description;
  @override
  final int? sortOrder;

  factory _$SurveyCategoryOptionResponse(
          [void Function(SurveyCategoryOptionResponseBuilder)? updates]) =>
      (SurveyCategoryOptionResponseBuilder()..update(updates))._build();

  _$SurveyCategoryOptionResponse._(
      {this.code, this.name, this.description, this.sortOrder})
      : super._();
  @override
  SurveyCategoryOptionResponse rebuild(
          void Function(SurveyCategoryOptionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyCategoryOptionResponseBuilder toBuilder() =>
      SurveyCategoryOptionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyCategoryOptionResponse &&
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
    return (newBuiltValueToStringHelper(r'SurveyCategoryOptionResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class SurveyCategoryOptionResponseBuilder
    implements
        Builder<SurveyCategoryOptionResponse,
            SurveyCategoryOptionResponseBuilder> {
  _$SurveyCategoryOptionResponse? _$v;

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

  SurveyCategoryOptionResponseBuilder() {
    SurveyCategoryOptionResponse._defaults(this);
  }

  SurveyCategoryOptionResponseBuilder get _$this {
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
  void replace(SurveyCategoryOptionResponse other) {
    _$v = other as _$SurveyCategoryOptionResponse;
  }

  @override
  void update(void Function(SurveyCategoryOptionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyCategoryOptionResponse build() => _build();

  _$SurveyCategoryOptionResponse _build() {
    _$SurveyCategoryOptionResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyCategoryOptionResponse._(
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
            r'SurveyCategoryOptionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
