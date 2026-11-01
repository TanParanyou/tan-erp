// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_planning_parameter_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpPlanningParameterListResponse
    extends MrpPlanningParameterListResponse {
  @override
  final BuiltList<MrpPlanningParameterResponse>? items;
  @override
  final MrpPaginationResponse? pagination;

  factory _$MrpPlanningParameterListResponse(
          [void Function(MrpPlanningParameterListResponseBuilder)? updates]) =>
      (MrpPlanningParameterListResponseBuilder()..update(updates))._build();

  _$MrpPlanningParameterListResponse._({this.items, this.pagination})
      : super._();
  @override
  MrpPlanningParameterListResponse rebuild(
          void Function(MrpPlanningParameterListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpPlanningParameterListResponseBuilder toBuilder() =>
      MrpPlanningParameterListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpPlanningParameterListResponse &&
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
    return (newBuiltValueToStringHelper(r'MrpPlanningParameterListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class MrpPlanningParameterListResponseBuilder
    implements
        Builder<MrpPlanningParameterListResponse,
            MrpPlanningParameterListResponseBuilder> {
  _$MrpPlanningParameterListResponse? _$v;

  ListBuilder<MrpPlanningParameterResponse>? _items;
  ListBuilder<MrpPlanningParameterResponse> get items =>
      _$this._items ??= ListBuilder<MrpPlanningParameterResponse>();
  set items(ListBuilder<MrpPlanningParameterResponse>? items) =>
      _$this._items = items;

  MrpPaginationResponseBuilder? _pagination;
  MrpPaginationResponseBuilder get pagination =>
      _$this._pagination ??= MrpPaginationResponseBuilder();
  set pagination(MrpPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  MrpPlanningParameterListResponseBuilder() {
    MrpPlanningParameterListResponse._defaults(this);
  }

  MrpPlanningParameterListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpPlanningParameterListResponse other) {
    _$v = other as _$MrpPlanningParameterListResponse;
  }

  @override
  void update(void Function(MrpPlanningParameterListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpPlanningParameterListResponse build() => _build();

  _$MrpPlanningParameterListResponse _build() {
    _$MrpPlanningParameterListResponse _$result;
    try {
      _$result = _$v ??
          _$MrpPlanningParameterListResponse._(
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
            r'MrpPlanningParameterListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
