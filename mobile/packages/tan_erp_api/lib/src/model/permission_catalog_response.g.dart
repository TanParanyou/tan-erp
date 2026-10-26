// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'permission_catalog_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PermissionCatalogResponse extends PermissionCatalogResponse {
  @override
  final BuiltList<PermissionCatalogEntryResponse>? items;

  factory _$PermissionCatalogResponse(
          [void Function(PermissionCatalogResponseBuilder)? updates]) =>
      (PermissionCatalogResponseBuilder()..update(updates))._build();

  _$PermissionCatalogResponse._({this.items}) : super._();
  @override
  PermissionCatalogResponse rebuild(
          void Function(PermissionCatalogResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PermissionCatalogResponseBuilder toBuilder() =>
      PermissionCatalogResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PermissionCatalogResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PermissionCatalogResponse')
          ..add('items', items))
        .toString();
  }
}

class PermissionCatalogResponseBuilder
    implements
        Builder<PermissionCatalogResponse, PermissionCatalogResponseBuilder> {
  _$PermissionCatalogResponse? _$v;

  ListBuilder<PermissionCatalogEntryResponse>? _items;
  ListBuilder<PermissionCatalogEntryResponse> get items =>
      _$this._items ??= ListBuilder<PermissionCatalogEntryResponse>();
  set items(ListBuilder<PermissionCatalogEntryResponse>? items) =>
      _$this._items = items;

  PermissionCatalogResponseBuilder() {
    PermissionCatalogResponse._defaults(this);
  }

  PermissionCatalogResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PermissionCatalogResponse other) {
    _$v = other as _$PermissionCatalogResponse;
  }

  @override
  void update(void Function(PermissionCatalogResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PermissionCatalogResponse build() => _build();

  _$PermissionCatalogResponse _build() {
    _$PermissionCatalogResponse _$result;
    try {
      _$result = _$v ??
          _$PermissionCatalogResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PermissionCatalogResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
