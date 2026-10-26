// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_address_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerAddressListResponse extends CustomerAddressListResponse {
  @override
  final BuiltList<CustomerAddressResponse>? items;

  factory _$CustomerAddressListResponse(
          [void Function(CustomerAddressListResponseBuilder)? updates]) =>
      (CustomerAddressListResponseBuilder()..update(updates))._build();

  _$CustomerAddressListResponse._({this.items}) : super._();
  @override
  CustomerAddressListResponse rebuild(
          void Function(CustomerAddressListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerAddressListResponseBuilder toBuilder() =>
      CustomerAddressListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerAddressListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'CustomerAddressListResponse')
          ..add('items', items))
        .toString();
  }
}

class CustomerAddressListResponseBuilder
    implements
        Builder<CustomerAddressListResponse,
            CustomerAddressListResponseBuilder> {
  _$CustomerAddressListResponse? _$v;

  ListBuilder<CustomerAddressResponse>? _items;
  ListBuilder<CustomerAddressResponse> get items =>
      _$this._items ??= ListBuilder<CustomerAddressResponse>();
  set items(ListBuilder<CustomerAddressResponse>? items) =>
      _$this._items = items;

  CustomerAddressListResponseBuilder() {
    CustomerAddressListResponse._defaults(this);
  }

  CustomerAddressListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerAddressListResponse other) {
    _$v = other as _$CustomerAddressListResponse;
  }

  @override
  void update(void Function(CustomerAddressListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerAddressListResponse build() => _build();

  _$CustomerAddressListResponse _build() {
    _$CustomerAddressListResponse _$result;
    try {
      _$result = _$v ??
          _$CustomerAddressListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CustomerAddressListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
