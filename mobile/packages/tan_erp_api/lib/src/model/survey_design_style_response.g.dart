// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_design_style_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyDesignStyleResponse extends SurveyDesignStyleResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final SurveyLocalizedNameContract? name;
  @override
  final SurveyLocalizedNameContract? description;
  @override
  final int? sortOrder;
  @override
  final bool? isActive;
  @override
  final DateTime? createdAtUtc;

  factory _$SurveyDesignStyleResponse(
          [void Function(SurveyDesignStyleResponseBuilder)? updates]) =>
      (SurveyDesignStyleResponseBuilder()..update(updates))._build();

  _$SurveyDesignStyleResponse._(
      {this.id,
      this.code,
      this.name,
      this.description,
      this.sortOrder,
      this.isActive,
      this.createdAtUtc})
      : super._();
  @override
  SurveyDesignStyleResponse rebuild(
          void Function(SurveyDesignStyleResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyDesignStyleResponseBuilder toBuilder() =>
      SurveyDesignStyleResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyDesignStyleResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
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
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyDesignStyleResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder)
          ..add('isActive', isActive)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class SurveyDesignStyleResponseBuilder
    implements
        Builder<SurveyDesignStyleResponse, SurveyDesignStyleResponseBuilder> {
  _$SurveyDesignStyleResponse? _$v;

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

  SurveyDesignStyleResponseBuilder() {
    SurveyDesignStyleResponse._defaults(this);
  }

  SurveyDesignStyleResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _sortOrder = $v.sortOrder;
      _isActive = $v.isActive;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyDesignStyleResponse other) {
    _$v = other as _$SurveyDesignStyleResponse;
  }

  @override
  void update(void Function(SurveyDesignStyleResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyDesignStyleResponse build() => _build();

  _$SurveyDesignStyleResponse _build() {
    _$SurveyDesignStyleResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyDesignStyleResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            description: _description?.build(),
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
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SurveyDesignStyleResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
