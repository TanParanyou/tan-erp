// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_survey_design_style_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateSurveyDesignStyleRequest extends CreateSurveyDesignStyleRequest {
  @override
  final String? code;
  @override
  final SurveyLocalizedNameContract? name;
  @override
  final SurveyLocalizedNameContract? description;
  @override
  final int? sortOrder;

  factory _$CreateSurveyDesignStyleRequest(
          [void Function(CreateSurveyDesignStyleRequestBuilder)? updates]) =>
      (CreateSurveyDesignStyleRequestBuilder()..update(updates))._build();

  _$CreateSurveyDesignStyleRequest._(
      {this.code, this.name, this.description, this.sortOrder})
      : super._();
  @override
  CreateSurveyDesignStyleRequest rebuild(
          void Function(CreateSurveyDesignStyleRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateSurveyDesignStyleRequestBuilder toBuilder() =>
      CreateSurveyDesignStyleRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateSurveyDesignStyleRequest &&
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
    return (newBuiltValueToStringHelper(r'CreateSurveyDesignStyleRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class CreateSurveyDesignStyleRequestBuilder
    implements
        Builder<CreateSurveyDesignStyleRequest,
            CreateSurveyDesignStyleRequestBuilder> {
  _$CreateSurveyDesignStyleRequest? _$v;

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

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  CreateSurveyDesignStyleRequestBuilder() {
    CreateSurveyDesignStyleRequest._defaults(this);
  }

  CreateSurveyDesignStyleRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateSurveyDesignStyleRequest other) {
    _$v = other as _$CreateSurveyDesignStyleRequest;
  }

  @override
  void update(void Function(CreateSurveyDesignStyleRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateSurveyDesignStyleRequest build() => _build();

  _$CreateSurveyDesignStyleRequest _build() {
    _$CreateSurveyDesignStyleRequest _$result;
    try {
      _$result = _$v ??
          _$CreateSurveyDesignStyleRequest._(
            code: code,
            name: _name?.build(),
            description: _description?.build(),
            sortOrder: sortOrder,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
        _$failedField = 'description';
        _description?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateSurveyDesignStyleRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
