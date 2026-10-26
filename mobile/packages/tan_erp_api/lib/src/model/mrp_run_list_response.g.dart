// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_run_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpRunListResponse extends MrpRunListResponse {
  @override
  final BuiltList<MrpRunListItemResponse>? items;
  @override
  final MrpPaginationResponse? pagination;

  factory _$MrpRunListResponse(
          [void Function(MrpRunListResponseBuilder)? updates]) =>
      (MrpRunListResponseBuilder()..update(updates))._build();

  _$MrpRunListResponse._({this.items, this.pagination}) : super._();
  @override
  MrpRunListResponse rebuild(
          void Function(MrpRunListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpRunListResponseBuilder toBuilder() =>
      MrpRunListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpRunListResponse &&
        items == other.items &&
        pagination == other.pagination;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, pagination.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpRunListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class MrpRunListResponseBuilder
    implements Builder<MrpRunListResponse, MrpRunListResponseBuilder> {
  _$MrpRunListResponse? _$v;

  ListBuilder<MrpRunListItemResponse>? _items;
  ListBuilder<MrpRunListItemResponse> get items =>
      _$this._items ??= ListBuilder<MrpRunListItemResponse>();
  set items(ListBuilder<MrpRunListItemResponse>? items) =>
      _$this._items = items;

  MrpPaginationResponseBuilder? _pagination;
  MrpPaginationResponseBuilder get pagination =>
      _$this._pagination ??= MrpPaginationResponseBuilder();
  set pagination(MrpPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  MrpRunListResponseBuilder() {
    MrpRunListResponse._defaults(this);
  }

  MrpRunListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpRunListResponse other) {
    _$v = other as _$MrpRunListResponse;
  }

  @override
  void update(void Function(MrpRunListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpRunListResponse build() => _build();

  _$MrpRunListResponse _build() {
    _$MrpRunListResponse _$result;
    try {
      _$result = _$v ??
          _$MrpRunListResponse._(
            items: _items?.build(),
            pagination: _pagination?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
        _$failedField = 'pagination';
        _pagination?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'MrpRunListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
