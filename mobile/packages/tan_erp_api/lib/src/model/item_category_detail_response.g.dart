// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_category_detail_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemCategoryDetailResponse extends ItemCategoryDetailResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final LocalizedTextResponse? description;
  @override
  final String? parentCategoryId;
  @override
  final CategorySummaryResponse? parentCategory;
  @override
  final BuiltList<String>? allowedItemTypes;
  @override
  final int? sortOrder;
  @override
  final String? status;
  @override
  final String? imageFileId;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;

  factory _$ItemCategoryDetailResponse(
          [void Function(ItemCategoryDetailResponseBuilder)? updates]) =>
      (ItemCategoryDetailResponseBuilder()..update(updates))._build();

  _$ItemCategoryDetailResponse._(
      {this.id,
      this.organizationId,
      this.code,
      this.name,
      this.description,
      this.parentCategoryId,
      this.parentCategory,
      this.allowedItemTypes,
      this.sortOrder,
      this.status,
      this.imageFileId,
      this.rowVersion,
      this.createdAtUtc,
      this.updatedAtUtc})
      : super._();
  @override
  ItemCategoryDetailResponse rebuild(
          void Function(ItemCategoryDetailResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemCategoryDetailResponseBuilder toBuilder() =>
      ItemCategoryDetailResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemCategoryDetailResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        parentCategoryId == other.parentCategoryId &&
        parentCategory == other.parentCategory &&
        allowedItemTypes == other.allowedItemTypes &&
        sortOrder == other.sortOrder &&
        status == other.status &&
        imageFileId == other.imageFileId &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, parentCategoryId.hashCode);
    _$hash = $jc(_$hash, parentCategory.hashCode);
    _$hash = $jc(_$hash, allowedItemTypes.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemCategoryDetailResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('parentCategoryId', parentCategoryId)
          ..add('parentCategory', parentCategory)
          ..add('allowedItemTypes', allowedItemTypes)
          ..add('sortOrder', sortOrder)
          ..add('status', status)
          ..add('imageFileId', imageFileId)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class ItemCategoryDetailResponseBuilder
    implements
        Builder<ItemCategoryDetailResponse, ItemCategoryDetailResponseBuilder> {
  _$ItemCategoryDetailResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  LocalizedTextResponseBuilder? _description;
  LocalizedTextResponseBuilder get description =>
      _$this._description ??= LocalizedTextResponseBuilder();
  set description(LocalizedTextResponseBuilder? description) =>
      _$this._description = description;

  String? _parentCategoryId;
  String? get parentCategoryId => _$this._parentCategoryId;
  set parentCategoryId(String? parentCategoryId) =>
      _$this._parentCategoryId = parentCategoryId;

  CategorySummaryResponseBuilder? _parentCategory;
  CategorySummaryResponseBuilder get parentCategory =>
      _$this._parentCategory ??= CategorySummaryResponseBuilder();
  set parentCategory(CategorySummaryResponseBuilder? parentCategory) =>
      _$this._parentCategory = parentCategory;

  ListBuilder<String>? _allowedItemTypes;
  ListBuilder<String> get allowedItemTypes =>
      _$this._allowedItemTypes ??= ListBuilder<String>();
  set allowedItemTypes(ListBuilder<String>? allowedItemTypes) =>
      _$this._allowedItemTypes = allowedItemTypes;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  ItemCategoryDetailResponseBuilder() {
    ItemCategoryDetailResponse._defaults(this);
  }

  ItemCategoryDetailResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _parentCategoryId = $v.parentCategoryId;
      _parentCategory = $v.parentCategory?.toBuilder();
      _allowedItemTypes = $v.allowedItemTypes?.toBuilder();
      _sortOrder = $v.sortOrder;
      _status = $v.status;
      _imageFileId = $v.imageFileId;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemCategoryDetailResponse other) {
    _$v = other as _$ItemCategoryDetailResponse;
  }

  @override
  void update(void Function(ItemCategoryDetailResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemCategoryDetailResponse build() => _build();

  _$ItemCategoryDetailResponse _build() {
    _$ItemCategoryDetailResponse _$result;
    try {
      _$result = _$v ??
          _$ItemCategoryDetailResponse._(
            id: id,
            organizationId: organizationId,
            code: code,
            name: _name?.build(),
            description: _description?.build(),
            parentCategoryId: parentCategoryId,
            parentCategory: _parentCategory?.build(),
            allowedItemTypes: _allowedItemTypes?.build(),
            sortOrder: sortOrder,
            status: status,
            imageFileId: imageFileId,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
        _$failedField = 'description';
        _description?.build();

        _$failedField = 'parentCategory';
        _parentCategory?.build();
        _$failedField = 'allowedItemTypes';
        _allowedItemTypes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemCategoryDetailResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
