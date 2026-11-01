// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'credit_override_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreditOverrideListResponse extends CreditOverrideListResponse {
  @override
  final BuiltList<CreditOverrideResponse>? items;

  factory _$CreditOverrideListResponse(
          [void Function(CreditOverrideListResponseBuilder)? updates]) =>
      (CreditOverrideListResponseBuilder()..update(updates))._build();

  _$CreditOverrideListResponse._({this.items}) : super._();
  @override
  CreditOverrideListResponse rebuild(
          void Function(CreditOverrideListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreditOverrideListResponseBuilder toBuilder() =>
      CreditOverrideListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreditOverrideListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'CreditOverrideListResponse')
          ..add('items', items))
        .toString();
  }
}

class CreditOverrideListResponseBuilder
    implements
        Builder<CreditOverrideListResponse, CreditOverrideListResponseBuilder> {
  _$CreditOverrideListResponse? _$v;

  ListBuilder<CreditOverrideResponse>? _items;
  ListBuilder<CreditOverrideResponse> get items =>
      _$this._items ??= ListBuilder<CreditOverrideResponse>();
  set items(ListBuilder<CreditOverrideResponse>? items) =>
      _$this._items = items;

  CreditOverrideListResponseBuilder() {
    CreditOverrideListResponse._defaults(this);
  }

  CreditOverrideListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreditOverrideListResponse other) {
    _$v = other as _$CreditOverrideListResponse;
  }

  @override
  void update(void Function(CreditOverrideListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreditOverrideListResponse build() => _build();

  _$CreditOverrideListResponse _build() {
    _$CreditOverrideListResponse _$result;
    try {
      _$result = _$v ??
          _$CreditOverrideListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreditOverrideListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
