// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'audit_event_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AuditEventListResponse extends AuditEventListResponse {
  @override
  final BuiltList<AuditEventResponse>? items;
  @override
  final AdminPaginationResponse? pagination;

  factory _$AuditEventListResponse(
          [void Function(AuditEventListResponseBuilder)? updates]) =>
      (AuditEventListResponseBuilder()..update(updates))._build();

  _$AuditEventListResponse._({this.items, this.pagination}) : super._();
  @override
  AuditEventListResponse rebuild(
          void Function(AuditEventListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AuditEventListResponseBuilder toBuilder() =>
      AuditEventListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AuditEventListResponse &&
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
    return (newBuiltValueToStringHelper(r'AuditEventListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class AuditEventListResponseBuilder
    implements Builder<AuditEventListResponse, AuditEventListResponseBuilder> {
  _$AuditEventListResponse? _$v;

  ListBuilder<AuditEventResponse>? _items;
  ListBuilder<AuditEventResponse> get items =>
      _$this._items ??= ListBuilder<AuditEventResponse>();
  set items(ListBuilder<AuditEventResponse>? items) => _$this._items = items;

  AdminPaginationResponseBuilder? _pagination;
  AdminPaginationResponseBuilder get pagination =>
      _$this._pagination ??= AdminPaginationResponseBuilder();
  set pagination(AdminPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  AuditEventListResponseBuilder() {
    AuditEventListResponse._defaults(this);
  }

  AuditEventListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AuditEventListResponse other) {
    _$v = other as _$AuditEventListResponse;
  }

  @override
  void update(void Function(AuditEventListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AuditEventListResponse build() => _build();

  _$AuditEventListResponse _build() {
    _$AuditEventListResponse _$result;
    try {
      _$result = _$v ??
          _$AuditEventListResponse._(
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
            r'AuditEventListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
