// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'opportunity_stage_history_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OpportunityStageHistoryListResponse
    extends OpportunityStageHistoryListResponse {
  @override
  final BuiltList<OpportunityStageHistoryItemResponse>? items;

  factory _$OpportunityStageHistoryListResponse(
          [void Function(OpportunityStageHistoryListResponseBuilder)?
              updates]) =>
      (OpportunityStageHistoryListResponseBuilder()..update(updates))._build();

  _$OpportunityStageHistoryListResponse._({this.items}) : super._();
  @override
  OpportunityStageHistoryListResponse rebuild(
          void Function(OpportunityStageHistoryListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OpportunityStageHistoryListResponseBuilder toBuilder() =>
      OpportunityStageHistoryListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OpportunityStageHistoryListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'OpportunityStageHistoryListResponse')
          ..add('items', items))
        .toString();
  }
}

class OpportunityStageHistoryListResponseBuilder
    implements
        Builder<OpportunityStageHistoryListResponse,
            OpportunityStageHistoryListResponseBuilder> {
  _$OpportunityStageHistoryListResponse? _$v;

  ListBuilder<OpportunityStageHistoryItemResponse>? _items;
  ListBuilder<OpportunityStageHistoryItemResponse> get items =>
      _$this._items ??= ListBuilder<OpportunityStageHistoryItemResponse>();
  set items(ListBuilder<OpportunityStageHistoryItemResponse>? items) =>
      _$this._items = items;

  OpportunityStageHistoryListResponseBuilder() {
    OpportunityStageHistoryListResponse._defaults(this);
  }

  OpportunityStageHistoryListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OpportunityStageHistoryListResponse other) {
    _$v = other as _$OpportunityStageHistoryListResponse;
  }

  @override
  void update(
      void Function(OpportunityStageHistoryListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OpportunityStageHistoryListResponse build() => _build();

  _$OpportunityStageHistoryListResponse _build() {
    _$OpportunityStageHistoryListResponse _$result;
    try {
      _$result = _$v ??
          _$OpportunityStageHistoryListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(r'OpportunityStageHistoryListResponse',
            _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
