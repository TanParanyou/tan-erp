// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteListResponse extends SiteListResponse {
  @override
  final BuiltList<SiteResponse>? items;

  factory _$SiteListResponse(
          [void Function(SiteListResponseBuilder)? updates]) =>
      (SiteListResponseBuilder()..update(updates))._build();

  _$SiteListResponse._({this.items}) : super._();
  @override
  SiteListResponse rebuild(void Function(SiteListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteListResponseBuilder toBuilder() =>
      SiteListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'SiteListResponse')
          ..add('items', items))
        .toString();
  }
}

class SiteListResponseBuilder
    implements Builder<SiteListResponse, SiteListResponseBuilder> {
  _$SiteListResponse? _$v;

  ListBuilder<SiteResponse>? _items;
  ListBuilder<SiteResponse> get items =>
      _$this._items ??= ListBuilder<SiteResponse>();
  set items(ListBuilder<SiteResponse>? items) => _$this._items = items;

  SiteListResponseBuilder() {
    SiteListResponse._defaults(this);
  }

  SiteListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteListResponse other) {
    _$v = other as _$SiteListResponse;
  }

  @override
  void update(void Function(SiteListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteListResponse build() => _build();

  _$SiteListResponse _build() {
    _$SiteListResponse _$result;
    try {
      _$result = _$v ??
          _$SiteListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SiteListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
