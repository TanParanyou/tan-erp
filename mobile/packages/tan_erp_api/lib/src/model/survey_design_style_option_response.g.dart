// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_design_style_option_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyDesignStyleOptionResponse
    extends SurveyDesignStyleOptionResponse {
  @override
  final String? code;
  @override
  final SurveyLocalizedLabel? name;
  @override
  final String? description;
  @override
  final int? sortOrder;
  @override
  final String? imageFileId;
  @override
  final BuiltList<String>? imageFileIds;

  factory _$SurveyDesignStyleOptionResponse(
          [void Function(SurveyDesignStyleOptionResponseBuilder)? updates]) =>
      (SurveyDesignStyleOptionResponseBuilder()..update(updates))._build();

  _$SurveyDesignStyleOptionResponse._(
      {this.code,
      this.name,
      this.description,
      this.sortOrder,
      this.imageFileId,
      this.imageFileIds})
      : super._();
  @override
  SurveyDesignStyleOptionResponse rebuild(
          void Function(SurveyDesignStyleOptionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyDesignStyleOptionResponseBuilder toBuilder() =>
      SurveyDesignStyleOptionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyDesignStyleOptionResponse &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        sortOrder == other.sortOrder &&
        imageFileId == other.imageFileId &&
        imageFileIds == other.imageFileIds;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jc(_$hash, imageFileIds.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyDesignStyleOptionResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder)
          ..add('imageFileId', imageFileId)
          ..add('imageFileIds', imageFileIds))
        .toString();
  }
}

class SurveyDesignStyleOptionResponseBuilder
    implements
        Builder<SurveyDesignStyleOptionResponse,
            SurveyDesignStyleOptionResponseBuilder> {
  _$SurveyDesignStyleOptionResponse? _$v;

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

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  ListBuilder<String>? _imageFileIds;
  ListBuilder<String> get imageFileIds =>
      _$this._imageFileIds ??= ListBuilder<String>();
  set imageFileIds(ListBuilder<String>? imageFileIds) =>
      _$this._imageFileIds = imageFileIds;

  SurveyDesignStyleOptionResponseBuilder() {
    SurveyDesignStyleOptionResponse._defaults(this);
  }

  SurveyDesignStyleOptionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description;
      _sortOrder = $v.sortOrder;
      _imageFileId = $v.imageFileId;
      _imageFileIds = $v.imageFileIds?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyDesignStyleOptionResponse other) {
    _$v = other as _$SurveyDesignStyleOptionResponse;
  }

  @override
  void update(void Function(SurveyDesignStyleOptionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyDesignStyleOptionResponse build() => _build();

  _$SurveyDesignStyleOptionResponse _build() {
    _$SurveyDesignStyleOptionResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyDesignStyleOptionResponse._(
            code: code,
            name: _name?.build(),
            description: description,
            sortOrder: sortOrder,
            imageFileId: imageFileId,
            imageFileIds: _imageFileIds?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();

        _$failedField = 'imageFileIds';
        _imageFileIds?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SurveyDesignStyleOptionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
