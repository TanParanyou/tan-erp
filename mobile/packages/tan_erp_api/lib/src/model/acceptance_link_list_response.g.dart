// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'acceptance_link_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AcceptanceLinkListResponse extends AcceptanceLinkListResponse {
  @override
  final BuiltList<AcceptanceLinkResponse>? items;

  factory _$AcceptanceLinkListResponse(
          [void Function(AcceptanceLinkListResponseBuilder)? updates]) =>
      (AcceptanceLinkListResponseBuilder()..update(updates))._build();

  _$AcceptanceLinkListResponse._({this.items}) : super._();
  @override
  AcceptanceLinkListResponse rebuild(
          void Function(AcceptanceLinkListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AcceptanceLinkListResponseBuilder toBuilder() =>
      AcceptanceLinkListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AcceptanceLinkListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'AcceptanceLinkListResponse')
          ..add('items', items))
        .toString();
  }
}

class AcceptanceLinkListResponseBuilder
    implements
        Builder<AcceptanceLinkListResponse, AcceptanceLinkListResponseBuilder> {
  _$AcceptanceLinkListResponse? _$v;

  ListBuilder<AcceptanceLinkResponse>? _items;
  ListBuilder<AcceptanceLinkResponse> get items =>
      _$this._items ??= ListBuilder<AcceptanceLinkResponse>();
  set items(ListBuilder<AcceptanceLinkResponse>? items) =>
      _$this._items = items;

  AcceptanceLinkListResponseBuilder() {
    AcceptanceLinkListResponse._defaults(this);
  }

  AcceptanceLinkListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AcceptanceLinkListResponse other) {
    _$v = other as _$AcceptanceLinkListResponse;
  }

  @override
  void update(void Function(AcceptanceLinkListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AcceptanceLinkListResponse build() => _build();

  _$AcceptanceLinkListResponse _build() {
    _$AcceptanceLinkListResponse _$result;
    try {
      _$result = _$v ??
          _$AcceptanceLinkListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AcceptanceLinkListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
