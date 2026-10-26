// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_alias_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemAliasResponse extends ItemAliasResponse {
  @override
  final String? id;
  @override
  final LocalizedTextResponse? alias;
  @override
  final String? status;

  factory _$ItemAliasResponse(
          [void Function(ItemAliasResponseBuilder)? updates]) =>
      (ItemAliasResponseBuilder()..update(updates))._build();

  _$ItemAliasResponse._({this.id, this.alias, this.status}) : super._();
  @override
  ItemAliasResponse rebuild(void Function(ItemAliasResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemAliasResponseBuilder toBuilder() =>
      ItemAliasResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemAliasResponse &&
        id == other.id &&
        alias == other.alias &&
        status == other.status;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, alias.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemAliasResponse')
          ..add('id', id)
          ..add('alias', alias)
          ..add('status', status))
        .toString();
  }
}

class ItemAliasResponseBuilder
    implements Builder<ItemAliasResponse, ItemAliasResponseBuilder> {
  _$ItemAliasResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  LocalizedTextResponseBuilder? _alias;
  LocalizedTextResponseBuilder get alias =>
      _$this._alias ??= LocalizedTextResponseBuilder();
  set alias(LocalizedTextResponseBuilder? alias) => _$this._alias = alias;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  ItemAliasResponseBuilder() {
    ItemAliasResponse._defaults(this);
  }

  ItemAliasResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _alias = $v.alias?.toBuilder();
      _status = $v.status;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemAliasResponse other) {
    _$v = other as _$ItemAliasResponse;
  }

  @override
  void update(void Function(ItemAliasResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemAliasResponse build() => _build();

  _$ItemAliasResponse _build() {
    _$ItemAliasResponse _$result;
    try {
      _$result = _$v ??
          _$ItemAliasResponse._(
            id: id,
            alias: _alias?.build(),
            status: status,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'alias';
        _alias?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemAliasResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
