// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectListResponse extends ProjectListResponse {
  @override
  final BuiltList<ProjectListItemResponse>? items;
  @override
  final ProjectPaginationResponse? pagination;

  factory _$ProjectListResponse(
          [void Function(ProjectListResponseBuilder)? updates]) =>
      (ProjectListResponseBuilder()..update(updates))._build();

  _$ProjectListResponse._({this.items, this.pagination}) : super._();
  @override
  ProjectListResponse rebuild(
          void Function(ProjectListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectListResponseBuilder toBuilder() =>
      ProjectListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectListResponse &&
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
    return (newBuiltValueToStringHelper(r'ProjectListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class ProjectListResponseBuilder
    implements Builder<ProjectListResponse, ProjectListResponseBuilder> {
  _$ProjectListResponse? _$v;

  ListBuilder<ProjectListItemResponse>? _items;
  ListBuilder<ProjectListItemResponse> get items =>
      _$this._items ??= ListBuilder<ProjectListItemResponse>();
  set items(ListBuilder<ProjectListItemResponse>? items) =>
      _$this._items = items;

  ProjectPaginationResponseBuilder? _pagination;
  ProjectPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProjectPaginationResponseBuilder();
  set pagination(ProjectPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  ProjectListResponseBuilder() {
    ProjectListResponse._defaults(this);
  }

  ProjectListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectListResponse other) {
    _$v = other as _$ProjectListResponse;
  }

  @override
  void update(void Function(ProjectListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectListResponse build() => _build();

  _$ProjectListResponse _build() {
    _$ProjectListResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectListResponse._(
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
            r'ProjectListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
