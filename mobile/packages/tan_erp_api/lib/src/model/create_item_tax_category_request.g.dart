// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_item_tax_category_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateItemTaxCategoryRequest extends CreateItemTaxCategoryRequest {
  @override
  final String? code;
  @override
  final LocalizedTextInput name;
  @override
  final int? sortOrder;

  factory _$CreateItemTaxCategoryRequest(
          [void Function(CreateItemTaxCategoryRequestBuilder)? updates]) =>
      (CreateItemTaxCategoryRequestBuilder()..update(updates))._build();

  _$CreateItemTaxCategoryRequest._(
      {this.code, required this.name, this.sortOrder})
      : super._();
  @override
  CreateItemTaxCategoryRequest rebuild(
          void Function(CreateItemTaxCategoryRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateItemTaxCategoryRequestBuilder toBuilder() =>
      CreateItemTaxCategoryRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateItemTaxCategoryRequest &&
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
    return (newBuiltValueToStringHelper(r'CreateItemTaxCategoryRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class CreateItemTaxCategoryRequestBuilder
    implements
        Builder<CreateItemTaxCategoryRequest,
            CreateItemTaxCategoryRequestBuilder> {
  _$CreateItemTaxCategoryRequest? _$v;

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

  CreateItemTaxCategoryRequestBuilder() {
    CreateItemTaxCategoryRequest._defaults(this);
  }

  CreateItemTaxCategoryRequestBuilder get _$this {
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
  void replace(CreateItemTaxCategoryRequest other) {
    _$v = other as _$CreateItemTaxCategoryRequest;
  }

  @override
  void update(void Function(CreateItemTaxCategoryRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateItemTaxCategoryRequest build() => _build();

  _$CreateItemTaxCategoryRequest _build() {
    _$CreateItemTaxCategoryRequest _$result;
    try {
      _$result = _$v ??
          _$CreateItemTaxCategoryRequest._(
            code: code,
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
            r'CreateItemTaxCategoryRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
