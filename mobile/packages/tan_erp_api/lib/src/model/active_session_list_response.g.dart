// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'active_session_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ActiveSessionListResponse extends ActiveSessionListResponse {
  @override
  final BuiltList<ActiveSessionItemDto>? items;

  factory _$ActiveSessionListResponse(
          [void Function(ActiveSessionListResponseBuilder)? updates]) =>
      (ActiveSessionListResponseBuilder()..update(updates))._build();

  _$ActiveSessionListResponse._({this.items}) : super._();
  @override
  ActiveSessionListResponse rebuild(
          void Function(ActiveSessionListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ActiveSessionListResponseBuilder toBuilder() =>
      ActiveSessionListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ActiveSessionListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'ActiveSessionListResponse')
          ..add('items', items))
        .toString();
  }
}

class ActiveSessionListResponseBuilder
    implements
        Builder<ActiveSessionListResponse, ActiveSessionListResponseBuilder> {
  _$ActiveSessionListResponse? _$v;

  ListBuilder<ActiveSessionItemDto>? _items;
  ListBuilder<ActiveSessionItemDto> get items =>
      _$this._items ??= ListBuilder<ActiveSessionItemDto>();
  set items(ListBuilder<ActiveSessionItemDto>? items) => _$this._items = items;

  ActiveSessionListResponseBuilder() {
    ActiveSessionListResponse._defaults(this);
  }

  ActiveSessionListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ActiveSessionListResponse other) {
    _$v = other as _$ActiveSessionListResponse;
  }

  @override
  void update(void Function(ActiveSessionListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ActiveSessionListResponse build() => _build();

  _$ActiveSessionListResponse _build() {
    _$ActiveSessionListResponse _$result;
    try {
      _$result = _$v ??
          _$ActiveSessionListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ActiveSessionListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
