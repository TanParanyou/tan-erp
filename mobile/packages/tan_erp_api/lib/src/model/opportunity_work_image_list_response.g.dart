// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'opportunity_work_image_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OpportunityWorkImageListResponse
    extends OpportunityWorkImageListResponse {
  @override
  final BuiltList<OpportunityWorkImageResponse>? items;
  @override
  final String? nextCursor;

  factory _$OpportunityWorkImageListResponse(
          [void Function(OpportunityWorkImageListResponseBuilder)? updates]) =>
      (OpportunityWorkImageListResponseBuilder()..update(updates))._build();

  _$OpportunityWorkImageListResponse._({this.items, this.nextCursor})
      : super._();
  @override
  OpportunityWorkImageListResponse rebuild(
          void Function(OpportunityWorkImageListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OpportunityWorkImageListResponseBuilder toBuilder() =>
      OpportunityWorkImageListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OpportunityWorkImageListResponse &&
        items == other.items &&
        nextCursor == other.nextCursor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, nextCursor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OpportunityWorkImageListResponse')
          ..add('items', items)
          ..add('nextCursor', nextCursor))
        .toString();
  }
}

class OpportunityWorkImageListResponseBuilder
    implements
        Builder<OpportunityWorkImageListResponse,
            OpportunityWorkImageListResponseBuilder> {
  _$OpportunityWorkImageListResponse? _$v;

  ListBuilder<OpportunityWorkImageResponse>? _items;
  ListBuilder<OpportunityWorkImageResponse> get items =>
      _$this._items ??= ListBuilder<OpportunityWorkImageResponse>();
  set items(ListBuilder<OpportunityWorkImageResponse>? items) =>
      _$this._items = items;

  String? _nextCursor;
  String? get nextCursor => _$this._nextCursor;
  set nextCursor(String? nextCursor) => _$this._nextCursor = nextCursor;

  OpportunityWorkImageListResponseBuilder() {
    OpportunityWorkImageListResponse._defaults(this);
  }

  OpportunityWorkImageListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _nextCursor = $v.nextCursor;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OpportunityWorkImageListResponse other) {
    _$v = other as _$OpportunityWorkImageListResponse;
  }

  @override
  void update(void Function(OpportunityWorkImageListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OpportunityWorkImageListResponse build() => _build();

  _$OpportunityWorkImageListResponse _build() {
    _$OpportunityWorkImageListResponse _$result;
    try {
      _$result = _$v ??
          _$OpportunityWorkImageListResponse._(
            items: _items?.build(),
            nextCursor: nextCursor,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'OpportunityWorkImageListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
