// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_item_category_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateItemCategoryRequest extends CreateItemCategoryRequest {
  @override
  final String? code;
  @override
  final LocalizedTextInput name;
  @override
  final LocalizedTextInput? description;
  @override
  final String? parentCategoryId;
  @override
  final BuiltList<String>? allowedItemTypes;
  @override
  final int? sortOrder;

  factory _$CreateItemCategoryRequest(
          [void Function(CreateItemCategoryRequestBuilder)? updates]) =>
      (CreateItemCategoryRequestBuilder()..update(updates))._build();

  _$CreateItemCategoryRequest._(
      {this.code,
      required this.name,
      this.description,
      this.parentCategoryId,
      this.allowedItemTypes,
      this.sortOrder})
      : super._();
  @override
  CreateItemCategoryRequest rebuild(
          void Function(CreateItemCategoryRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateItemCategoryRequestBuilder toBuilder() =>
      CreateItemCategoryRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateItemCategoryRequest &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        parentCategoryId == other.parentCategoryId &&
        allowedItemTypes == other.allowedItemTypes &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, parentCategoryId.hashCode);
    _$hash = $jc(_$hash, allowedItemTypes.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateItemCategoryRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('parentCategoryId', parentCategoryId)
          ..add('allowedItemTypes', allowedItemTypes)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class CreateItemCategoryRequestBuilder
    implements
        Builder<CreateItemCategoryRequest, CreateItemCategoryRequestBuilder> {
  _$CreateItemCategoryRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextInputBuilder? _name;
  LocalizedTextInputBuilder get name =>
      _$this._name ??= LocalizedTextInputBuilder();
  set name(LocalizedTextInputBuilder? name) => _$this._name = name;

  LocalizedTextInputBuilder? _description;
  LocalizedTextInputBuilder get description =>
      _$this._description ??= LocalizedTextInputBuilder();
  set description(LocalizedTextInputBuilder? description) =>
      _$this._description = description;

  String? _parentCategoryId;
  String? get parentCategoryId => _$this._parentCategoryId;
  set parentCategoryId(String? parentCategoryId) =>
      _$this._parentCategoryId = parentCategoryId;

  ListBuilder<String>? _allowedItemTypes;
  ListBuilder<String> get allowedItemTypes =>
      _$this._allowedItemTypes ??= ListBuilder<String>();
  set allowedItemTypes(ListBuilder<String>? allowedItemTypes) =>
      _$this._allowedItemTypes = allowedItemTypes;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  CreateItemCategoryRequestBuilder() {
    CreateItemCategoryRequest._defaults(this);
  }

  CreateItemCategoryRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name.toBuilder();
      _description = $v.description?.toBuilder();
      _parentCategoryId = $v.parentCategoryId;
      _allowedItemTypes = $v.allowedItemTypes?.toBuilder();
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateItemCategoryRequest other) {
    _$v = other as _$CreateItemCategoryRequest;
  }

  @override
  void update(void Function(CreateItemCategoryRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateItemCategoryRequest build() => _build();

  _$CreateItemCategoryRequest _build() {
    _$CreateItemCategoryRequest _$result;
    try {
      _$result = _$v ??
          _$CreateItemCategoryRequest._(
            code: code,
            name: name.build(),
            description: _description?.build(),
            parentCategoryId: parentCategoryId,
            allowedItemTypes: _allowedItemTypes?.build(),
            sortOrder: sortOrder,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        name.build();
        _$failedField = 'description';
        _description?.build();

        _$failedField = 'allowedItemTypes';
        _allowedItemTypes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateItemCategoryRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
