// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_contact_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerContactListResponse extends CustomerContactListResponse {
  @override
  final BuiltList<CustomerContactDetailResponse>? items;

  factory _$CustomerContactListResponse(
          [void Function(CustomerContactListResponseBuilder)? updates]) =>
      (CustomerContactListResponseBuilder()..update(updates))._build();

  _$CustomerContactListResponse._({this.items}) : super._();
  @override
  CustomerContactListResponse rebuild(
          void Function(CustomerContactListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerContactListResponseBuilder toBuilder() =>
      CustomerContactListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerContactListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'CustomerContactListResponse')
          ..add('items', items))
        .toString();
  }
}

class CustomerContactListResponseBuilder
    implements
        Builder<CustomerContactListResponse,
            CustomerContactListResponseBuilder> {
  _$CustomerContactListResponse? _$v;

  ListBuilder<CustomerContactDetailResponse>? _items;
  ListBuilder<CustomerContactDetailResponse> get items =>
      _$this._items ??= ListBuilder<CustomerContactDetailResponse>();
  set items(ListBuilder<CustomerContactDetailResponse>? items) =>
      _$this._items = items;

  CustomerContactListResponseBuilder() {
    CustomerContactListResponse._defaults(this);
  }

  CustomerContactListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerContactListResponse other) {
    _$v = other as _$CustomerContactListResponse;
  }

  @override
  void update(void Function(CustomerContactListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerContactListResponse build() => _build();

  _$CustomerContactListResponse _build() {
    _$CustomerContactListResponse _$result;
    try {
      _$result = _$v ??
          _$CustomerContactListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CustomerContactListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
