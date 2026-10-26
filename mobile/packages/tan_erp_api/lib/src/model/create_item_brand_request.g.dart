// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_item_brand_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateItemBrandRequest extends CreateItemBrandRequest {
  @override
  final String? code;
  @override
  final LocalizedTextInput name;
  @override
  final LocalizedTextInput? description;
  @override
  final int? sortOrder;

  factory _$CreateItemBrandRequest(
          [void Function(CreateItemBrandRequestBuilder)? updates]) =>
      (CreateItemBrandRequestBuilder()..update(updates))._build();

  _$CreateItemBrandRequest._(
      {this.code, required this.name, this.description, this.sortOrder})
      : super._();
  @override
  CreateItemBrandRequest rebuild(
          void Function(CreateItemBrandRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateItemBrandRequestBuilder toBuilder() =>
      CreateItemBrandRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateItemBrandRequest &&
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
    return (newBuiltValueToStringHelper(r'CreateItemBrandRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class CreateItemBrandRequestBuilder
    implements Builder<CreateItemBrandRequest, CreateItemBrandRequestBuilder> {
  _$CreateItemBrandRequest? _$v;

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

  CreateItemBrandRequestBuilder() {
    CreateItemBrandRequest._defaults(this);
  }

  CreateItemBrandRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name.toBuilder();
      _description = $v.description?.toBuilder();
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateItemBrandRequest other) {
    _$v = other as _$CreateItemBrandRequest;
  }

  @override
  void update(void Function(CreateItemBrandRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateItemBrandRequest build() => _build();

  _$CreateItemBrandRequest _build() {
    _$CreateItemBrandRequest _$result;
    try {
      _$result = _$v ??
          _$CreateItemBrandRequest._(
            code: code,
            name: name.build(),
            description: _description?.build(),
            sortOrder: sortOrder,
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
            r'CreateItemBrandRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
