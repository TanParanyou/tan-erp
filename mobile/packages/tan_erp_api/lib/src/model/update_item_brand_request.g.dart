// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_item_brand_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateItemBrandRequest extends UpdateItemBrandRequest {
  @override
  final String code;
  @override
  final LocalizedTextInput name;
  @override
  final LocalizedTextInput? description;
  @override
  final int? sortOrder;
  @override
  final String? imageFileId;

  factory _$UpdateItemBrandRequest(
          [void Function(UpdateItemBrandRequestBuilder)? updates]) =>
      (UpdateItemBrandRequestBuilder()..update(updates))._build();

  _$UpdateItemBrandRequest._(
      {required this.code,
      required this.name,
      this.description,
      this.sortOrder,
      this.imageFileId})
      : super._();
  @override
  UpdateItemBrandRequest rebuild(
          void Function(UpdateItemBrandRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateItemBrandRequestBuilder toBuilder() =>
      UpdateItemBrandRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateItemBrandRequest &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        sortOrder == other.sortOrder &&
        imageFileId == other.imageFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateItemBrandRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder)
          ..add('imageFileId', imageFileId))
        .toString();
  }
}

class UpdateItemBrandRequestBuilder
    implements Builder<UpdateItemBrandRequest, UpdateItemBrandRequestBuilder> {
  _$UpdateItemBrandRequest? _$v;

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

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  UpdateItemBrandRequestBuilder() {
    UpdateItemBrandRequest._defaults(this);
  }

  UpdateItemBrandRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name.toBuilder();
      _description = $v.description?.toBuilder();
      _sortOrder = $v.sortOrder;
      _imageFileId = $v.imageFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateItemBrandRequest other) {
    _$v = other as _$UpdateItemBrandRequest;
  }

  @override
  void update(void Function(UpdateItemBrandRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateItemBrandRequest build() => _build();

  _$UpdateItemBrandRequest _build() {
    _$UpdateItemBrandRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateItemBrandRequest._(
            code: BuiltValueNullFieldError.checkNotNull(
                code, r'UpdateItemBrandRequest', 'code'),
            name: name.build(),
            description: _description?.build(),
            sortOrder: sortOrder,
            imageFileId: imageFileId,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        name.build();
        _$failedField = 'description';
        _description?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateItemBrandRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
