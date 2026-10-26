// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_history_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationHistoryResponse extends QuotationHistoryResponse {
  @override
  final String? estimateId;
  @override
  final BuiltList<QuotationHistoryItemResponse>? items;

  factory _$QuotationHistoryResponse(
          [void Function(QuotationHistoryResponseBuilder)? updates]) =>
      (QuotationHistoryResponseBuilder()..update(updates))._build();

  _$QuotationHistoryResponse._({this.estimateId, this.items}) : super._();
  @override
  QuotationHistoryResponse rebuild(
          void Function(QuotationHistoryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationHistoryResponseBuilder toBuilder() =>
      QuotationHistoryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationHistoryResponse &&
        estimateId == other.estimateId &&
        items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, estimateId.hashCode);
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationHistoryResponse')
          ..add('estimateId', estimateId)
          ..add('items', items))
        .toString();
  }
}

class QuotationHistoryResponseBuilder
    implements
        Builder<QuotationHistoryResponse, QuotationHistoryResponseBuilder> {
  _$QuotationHistoryResponse? _$v;

  String? _estimateId;
  String? get estimateId => _$this._estimateId;
  set estimateId(String? estimateId) => _$this._estimateId = estimateId;

  ListBuilder<QuotationHistoryItemResponse>? _items;
  ListBuilder<QuotationHistoryItemResponse> get items =>
      _$this._items ??= ListBuilder<QuotationHistoryItemResponse>();
  set items(ListBuilder<QuotationHistoryItemResponse>? items) =>
      _$this._items = items;

  QuotationHistoryResponseBuilder() {
    QuotationHistoryResponse._defaults(this);
  }

  QuotationHistoryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _estimateId = $v.estimateId;
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationHistoryResponse other) {
    _$v = other as _$QuotationHistoryResponse;
  }

  @override
  void update(void Function(QuotationHistoryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationHistoryResponse build() => _build();

  _$QuotationHistoryResponse _build() {
    _$QuotationHistoryResponse _$result;
    try {
      _$result = _$v ??
          _$QuotationHistoryResponse._(
            estimateId: estimateId,
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuotationHistoryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
