// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'address_search_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AddressSearchResponse extends AddressSearchResponse {
  @override
  final BuiltList<AddressSearchResultItem>? items;

  factory _$AddressSearchResponse(
          [void Function(AddressSearchResponseBuilder)? updates]) =>
      (AddressSearchResponseBuilder()..update(updates))._build();

  _$AddressSearchResponse._({this.items}) : super._();
  @override
  AddressSearchResponse rebuild(
          void Function(AddressSearchResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AddressSearchResponseBuilder toBuilder() =>
      AddressSearchResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AddressSearchResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'AddressSearchResponse')
          ..add('items', items))
        .toString();
  }
}

class AddressSearchResponseBuilder
    implements Builder<AddressSearchResponse, AddressSearchResponseBuilder> {
  _$AddressSearchResponse? _$v;

  ListBuilder<AddressSearchResultItem>? _items;
  ListBuilder<AddressSearchResultItem> get items =>
      _$this._items ??= ListBuilder<AddressSearchResultItem>();
  set items(ListBuilder<AddressSearchResultItem>? items) =>
      _$this._items = items;

  AddressSearchResponseBuilder() {
    AddressSearchResponse._defaults(this);
  }

  AddressSearchResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AddressSearchResponse other) {
    _$v = other as _$AddressSearchResponse;
  }

  @override
  void update(void Function(AddressSearchResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AddressSearchResponse build() => _build();

  _$AddressSearchResponse _build() {
    _$AddressSearchResponse _$result;
    try {
      _$result = _$v ??
          _$AddressSearchResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AddressSearchResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
