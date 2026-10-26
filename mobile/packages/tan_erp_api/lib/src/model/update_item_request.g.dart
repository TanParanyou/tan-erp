// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_item_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateItemRequest extends UpdateItemRequest {
  @override
  final String code;
  @override
  final String itemType;
  @override
  final String categoryId;
  @override
  final String? brandId;
  @override
  final LocalizedTextInput name;
  @override
  final LocalizedTextInput? description;
  @override
  final String baseUnitId;
  @override
  final String availabilityMode;
  @override
  final ItemCapabilitiesInput? capabilities;
  @override
  final BuiltList<String>? selectedBranchIds;
  @override
  final BuiltMap<String, String>? attributes;
  @override
  final int? attributesSchemaVersion;
  @override
  final String? taxCategoryCode;

  factory _$UpdateItemRequest(
          [void Function(UpdateItemRequestBuilder)? updates]) =>
      (UpdateItemRequestBuilder()..update(updates))._build();

  _$UpdateItemRequest._(
      {required this.code,
      required this.itemType,
      required this.categoryId,
      this.brandId,
      required this.name,
      this.description,
      required this.baseUnitId,
      required this.availabilityMode,
      this.capabilities,
      this.selectedBranchIds,
      this.attributes,
      this.attributesSchemaVersion,
      this.taxCategoryCode})
      : super._();
  @override
  UpdateItemRequest rebuild(void Function(UpdateItemRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateItemRequestBuilder toBuilder() =>
      UpdateItemRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateItemRequest &&
        code == other.code &&
        itemType == other.itemType &&
        categoryId == other.categoryId &&
        brandId == other.brandId &&
        name == other.name &&
        description == other.description &&
        baseUnitId == other.baseUnitId &&
        availabilityMode == other.availabilityMode &&
        capabilities == other.capabilities &&
        selectedBranchIds == other.selectedBranchIds &&
        attributes == other.attributes &&
        attributesSchemaVersion == other.attributesSchemaVersion &&
        taxCategoryCode == other.taxCategoryCode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, itemType.hashCode);
    _$hash = $jc(_$hash, categoryId.hashCode);
    _$hash = $jc(_$hash, brandId.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, baseUnitId.hashCode);
    _$hash = $jc(_$hash, availabilityMode.hashCode);
    _$hash = $jc(_$hash, capabilities.hashCode);
    _$hash = $jc(_$hash, selectedBranchIds.hashCode);
    _$hash = $jc(_$hash, attributes.hashCode);
    _$hash = $jc(_$hash, attributesSchemaVersion.hashCode);
    _$hash = $jc(_$hash, taxCategoryCode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateItemRequest')
          ..add('code', code)
          ..add('itemType', itemType)
          ..add('categoryId', categoryId)
          ..add('brandId', brandId)
          ..add('name', name)
          ..add('description', description)
          ..add('baseUnitId', baseUnitId)
          ..add('availabilityMode', availabilityMode)
          ..add('capabilities', capabilities)
          ..add('selectedBranchIds', selectedBranchIds)
          ..add('attributes', attributes)
          ..add('attributesSchemaVersion', attributesSchemaVersion)
          ..add('taxCategoryCode', taxCategoryCode))
        .toString();
  }
}

class UpdateItemRequestBuilder
    implements Builder<UpdateItemRequest, UpdateItemRequestBuilder> {
  _$UpdateItemRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _itemType;
  String? get itemType => _$this._itemType;
  set itemType(String? itemType) => _$this._itemType = itemType;

  String? _categoryId;
  String? get categoryId => _$this._categoryId;
  set categoryId(String? categoryId) => _$this._categoryId = categoryId;

  String? _brandId;
  String? get brandId => _$this._brandId;
  set brandId(String? brandId) => _$this._brandId = brandId;

  LocalizedTextInputBuilder? _name;
  LocalizedTextInputBuilder get name =>
      _$this._name ??= LocalizedTextInputBuilder();
  set name(LocalizedTextInputBuilder? name) => _$this._name = name;

  LocalizedTextInputBuilder? _description;
  LocalizedTextInputBuilder get description =>
      _$this._description ??= LocalizedTextInputBuilder();
  set description(LocalizedTextInputBuilder? description) =>
      _$this._description = description;

  String? _baseUnitId;
  String? get baseUnitId => _$this._baseUnitId;
  set baseUnitId(String? baseUnitId) => _$this._baseUnitId = baseUnitId;

  String? _availabilityMode;
  String? get availabilityMode => _$this._availabilityMode;
  set availabilityMode(String? availabilityMode) =>
      _$this._availabilityMode = availabilityMode;

  ItemCapabilitiesInputBuilder? _capabilities;
  ItemCapabilitiesInputBuilder get capabilities =>
      _$this._capabilities ??= ItemCapabilitiesInputBuilder();
  set capabilities(ItemCapabilitiesInputBuilder? capabilities) =>
      _$this._capabilities = capabilities;

  ListBuilder<String>? _selectedBranchIds;
  ListBuilder<String> get selectedBranchIds =>
      _$this._selectedBranchIds ??= ListBuilder<String>();
  set selectedBranchIds(ListBuilder<String>? selectedBranchIds) =>
      _$this._selectedBranchIds = selectedBranchIds;

  MapBuilder<String, String>? _attributes;
  MapBuilder<String, String> get attributes =>
      _$this._attributes ??= MapBuilder<String, String>();
  set attributes(MapBuilder<String, String>? attributes) =>
      _$this._attributes = attributes;

  int? _attributesSchemaVersion;
  int? get attributesSchemaVersion => _$this._attributesSchemaVersion;
  set attributesSchemaVersion(int? attributesSchemaVersion) =>
      _$this._attributesSchemaVersion = attributesSchemaVersion;

  String? _taxCategoryCode;
  String? get taxCategoryCode => _$this._taxCategoryCode;
  set taxCategoryCode(String? taxCategoryCode) =>
      _$this._taxCategoryCode = taxCategoryCode;

  UpdateItemRequestBuilder() {
    UpdateItemRequest._defaults(this);
  }

  UpdateItemRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _itemType = $v.itemType;
      _categoryId = $v.categoryId;
      _brandId = $v.brandId;
      _name = $v.name.toBuilder();
      _description = $v.description?.toBuilder();
      _baseUnitId = $v.baseUnitId;
      _availabilityMode = $v.availabilityMode;
      _capabilities = $v.capabilities?.toBuilder();
      _selectedBranchIds = $v.selectedBranchIds?.toBuilder();
      _attributes = $v.attributes?.toBuilder();
      _attributesSchemaVersion = $v.attributesSchemaVersion;
      _taxCategoryCode = $v.taxCategoryCode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateItemRequest other) {
    _$v = other as _$UpdateItemRequest;
  }

  @override
  void update(void Function(UpdateItemRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateItemRequest build() => _build();

  _$UpdateItemRequest _build() {
    _$UpdateItemRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateItemRequest._(
            code: BuiltValueNullFieldError.checkNotNull(
                code, r'UpdateItemRequest', 'code'),
            itemType: BuiltValueNullFieldError.checkNotNull(
                itemType, r'UpdateItemRequest', 'itemType'),
            categoryId: BuiltValueNullFieldError.checkNotNull(
                categoryId, r'UpdateItemRequest', 'categoryId'),
            brandId: brandId,
            name: name.build(),
            description: _description?.build(),
            baseUnitId: BuiltValueNullFieldError.checkNotNull(
                baseUnitId, r'UpdateItemRequest', 'baseUnitId'),
            availabilityMode: BuiltValueNullFieldError.checkNotNull(
                availabilityMode, r'UpdateItemRequest', 'availabilityMode'),
            capabilities: _capabilities?.build(),
            selectedBranchIds: _selectedBranchIds?.build(),
            attributes: _attributes?.build(),
            attributesSchemaVersion: attributesSchemaVersion,
            taxCategoryCode: taxCategoryCode,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        name.build();
        _$failedField = 'description';
        _description?.build();

        _$failedField = 'capabilities';
        _capabilities?.build();
        _$failedField = 'selectedBranchIds';
        _selectedBranchIds?.build();
        _$failedField = 'attributes';
        _attributes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateItemRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
