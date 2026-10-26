// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'approval_limit_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ApprovalLimitListResponse extends ApprovalLimitListResponse {
  @override
  final BuiltList<ApprovalLimitResponse>? items;
  @override
  final AdminPaginationResponse? pagination;

  factory _$ApprovalLimitListResponse(
          [void Function(ApprovalLimitListResponseBuilder)? updates]) =>
      (ApprovalLimitListResponseBuilder()..update(updates))._build();

  _$ApprovalLimitListResponse._({this.items, this.pagination}) : super._();
  @override
  ApprovalLimitListResponse rebuild(
          void Function(ApprovalLimitListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ApprovalLimitListResponseBuilder toBuilder() =>
      ApprovalLimitListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ApprovalLimitListResponse &&
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
    return (newBuiltValueToStringHelper(r'ApprovalLimitListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class ApprovalLimitListResponseBuilder
    implements
        Builder<ApprovalLimitListResponse, ApprovalLimitListResponseBuilder> {
  _$ApprovalLimitListResponse? _$v;

  ListBuilder<ApprovalLimitResponse>? _items;
  ListBuilder<ApprovalLimitResponse> get items =>
      _$this._items ??= ListBuilder<ApprovalLimitResponse>();
  set items(ListBuilder<ApprovalLimitResponse>? items) => _$this._items = items;

  AdminPaginationResponseBuilder? _pagination;
  AdminPaginationResponseBuilder get pagination =>
      _$this._pagination ??= AdminPaginationResponseBuilder();
  set pagination(AdminPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  ApprovalLimitListResponseBuilder() {
    ApprovalLimitListResponse._defaults(this);
  }

  ApprovalLimitListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ApprovalLimitListResponse other) {
    _$v = other as _$ApprovalLimitListResponse;
  }

  @override
  void update(void Function(ApprovalLimitListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ApprovalLimitListResponse build() => _build();

  _$ApprovalLimitListResponse _build() {
    _$ApprovalLimitListResponse _$result;
    try {
      _$result = _$v ??
          _$ApprovalLimitListResponse._(
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
            r'ApprovalLimitListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
