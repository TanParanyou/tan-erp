// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_mapping_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AccountMappingListResponse extends AccountMappingListResponse {
  @override
  final BuiltList<AccountMappingResponse>? items;

  factory _$AccountMappingListResponse(
          [void Function(AccountMappingListResponseBuilder)? updates]) =>
      (AccountMappingListResponseBuilder()..update(updates))._build();

  _$AccountMappingListResponse._({this.items}) : super._();
  @override
  AccountMappingListResponse rebuild(
          void Function(AccountMappingListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AccountMappingListResponseBuilder toBuilder() =>
      AccountMappingListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AccountMappingListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'AccountMappingListResponse')
          ..add('items', items))
        .toString();
  }
}

class AccountMappingListResponseBuilder
    implements
        Builder<AccountMappingListResponse, AccountMappingListResponseBuilder> {
  _$AccountMappingListResponse? _$v;

  ListBuilder<AccountMappingResponse>? _items;
  ListBuilder<AccountMappingResponse> get items =>
      _$this._items ??= ListBuilder<AccountMappingResponse>();
  set items(ListBuilder<AccountMappingResponse>? items) =>
      _$this._items = items;

  AccountMappingListResponseBuilder() {
    AccountMappingListResponse._defaults(this);
  }

  AccountMappingListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AccountMappingListResponse other) {
    _$v = other as _$AccountMappingListResponse;
  }

  @override
  void update(void Function(AccountMappingListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AccountMappingListResponse build() => _build();

  _$AccountMappingListResponse _build() {
    _$AccountMappingListResponse _$result;
    try {
      _$result = _$v ??
          _$AccountMappingListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AccountMappingListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
