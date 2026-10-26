// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_item_tax_category_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateItemTaxCategoryRequest extends UpdateItemTaxCategoryRequest {
  @override
  final String code;
  @override
  final LocalizedTextInput name;
  @override
  final int? sortOrder;

  factory _$UpdateItemTaxCategoryRequest(
          [void Function(UpdateItemTaxCategoryRequestBuilder)? updates]) =>
      (UpdateItemTaxCategoryRequestBuilder()..update(updates))._build();

  _$UpdateItemTaxCategoryRequest._(
      {required this.code, required this.name, this.sortOrder})
      : super._();
  @override
  UpdateItemTaxCategoryRequest rebuild(
          void Function(UpdateItemTaxCategoryRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateItemTaxCategoryRequestBuilder toBuilder() =>
      UpdateItemTaxCategoryRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateItemTaxCategoryRequest &&
        code == other.code &&
        name == other.name &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateItemTaxCategoryRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class UpdateItemTaxCategoryRequestBuilder
    implements
        Builder<UpdateItemTaxCategoryRequest,
            UpdateItemTaxCategoryRequestBuilder> {
  _$UpdateItemTaxCategoryRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextInputBuilder? _name;
  LocalizedTextInputBuilder get name =>
      _$this._name ??= LocalizedTextInputBuilder();
  set name(LocalizedTextInputBuilder? name) => _$this._name = name;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  UpdateItemTaxCategoryRequestBuilder() {
    UpdateItemTaxCategoryRequest._defaults(this);
  }

  UpdateItemTaxCategoryRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name.toBuilder();
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateItemTaxCategoryRequest other) {
    _$v = other as _$UpdateItemTaxCategoryRequest;
  }

  @override
  void update(void Function(UpdateItemTaxCategoryRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateItemTaxCategoryRequest build() => _build();

  _$UpdateItemTaxCategoryRequest _build() {
    _$UpdateItemTaxCategoryRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateItemTaxCategoryRequest._(
            code: BuiltValueNullFieldError.checkNotNull(
                code, r'UpdateItemTaxCategoryRequest', 'code'),
            name: name.build(),
            sortOrder: sortOrder,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        name.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateItemTaxCategoryRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
