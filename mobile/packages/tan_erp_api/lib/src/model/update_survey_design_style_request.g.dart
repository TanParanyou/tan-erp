// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_survey_design_style_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateSurveyDesignStyleRequest extends UpdateSurveyDesignStyleRequest {
  @override
  final SurveyLocalizedNameContract? name;
  @override
  final SurveyLocalizedNameContract? description;
  @override
  final int? sortOrder;
  @override
  final bool? isActive;

  factory _$UpdateSurveyDesignStyleRequest(
          [void Function(UpdateSurveyDesignStyleRequestBuilder)? updates]) =>
      (UpdateSurveyDesignStyleRequestBuilder()..update(updates))._build();

  _$UpdateSurveyDesignStyleRequest._(
      {this.name, this.description, this.sortOrder, this.isActive})
      : super._();
  @override
  UpdateSurveyDesignStyleRequest rebuild(
          void Function(UpdateSurveyDesignStyleRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateSurveyDesignStyleRequestBuilder toBuilder() =>
      UpdateSurveyDesignStyleRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateSurveyDesignStyleRequest &&
        name == other.name &&
        description == other.description &&
        sortOrder == other.sortOrder &&
        isActive == other.isActive;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateSurveyDesignStyleRequest')
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder)
          ..add('isActive', isActive))
        .toString();
  }
}

class UpdateSurveyDesignStyleRequestBuilder
    implements
        Builder<UpdateSurveyDesignStyleRequest,
            UpdateSurveyDesignStyleRequestBuilder> {
  _$UpdateSurveyDesignStyleRequest? _$v;

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

  UpdateSurveyDesignStyleRequestBuilder() {
    UpdateSurveyDesignStyleRequest._defaults(this);
  }

  UpdateSurveyDesignStyleRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _sortOrder = $v.sortOrder;
      _isActive = $v.isActive;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateSurveyDesignStyleRequest other) {
    _$v = other as _$UpdateSurveyDesignStyleRequest;
  }

  @override
  void update(void Function(UpdateSurveyDesignStyleRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateSurveyDesignStyleRequest build() => _build();

  _$UpdateSurveyDesignStyleRequest _build() {
    _$UpdateSurveyDesignStyleRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateSurveyDesignStyleRequest._(
            name: _name?.build(),
            description: _description?.build(),
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
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateSurveyDesignStyleRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
