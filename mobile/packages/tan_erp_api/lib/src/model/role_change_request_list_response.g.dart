// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_change_request_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RoleChangeRequestListResponse extends RoleChangeRequestListResponse {
  @override
  final BuiltList<RoleChangeRequestResponse>? items;

  factory _$RoleChangeRequestListResponse(
          [void Function(RoleChangeRequestListResponseBuilder)? updates]) =>
      (RoleChangeRequestListResponseBuilder()..update(updates))._build();

  _$RoleChangeRequestListResponse._({this.items}) : super._();
  @override
  RoleChangeRequestListResponse rebuild(
          void Function(RoleChangeRequestListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RoleChangeRequestListResponseBuilder toBuilder() =>
      RoleChangeRequestListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RoleChangeRequestListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'RoleChangeRequestListResponse')
          ..add('items', items))
        .toString();
  }
}

class RoleChangeRequestListResponseBuilder
    implements
        Builder<RoleChangeRequestListResponse,
            RoleChangeRequestListResponseBuilder> {
  _$RoleChangeRequestListResponse? _$v;

  ListBuilder<RoleChangeRequestResponse>? _items;
  ListBuilder<RoleChangeRequestResponse> get items =>
      _$this._items ??= ListBuilder<RoleChangeRequestResponse>();
  set items(ListBuilder<RoleChangeRequestResponse>? items) =>
      _$this._items = items;

  RoleChangeRequestListResponseBuilder() {
    RoleChangeRequestListResponse._defaults(this);
  }

  RoleChangeRequestListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RoleChangeRequestListResponse other) {
    _$v = other as _$RoleChangeRequestListResponse;
  }

  @override
  void update(void Function(RoleChangeRequestListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RoleChangeRequestListResponse build() => _build();

  _$RoleChangeRequestListResponse _build() {
    _$RoleChangeRequestListResponse _$result;
    try {
      _$result = _$v ??
          _$RoleChangeRequestListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'RoleChangeRequestListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
