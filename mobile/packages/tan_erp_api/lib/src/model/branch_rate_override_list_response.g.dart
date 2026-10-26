// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_rate_override_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchRateOverrideListResponse extends BranchRateOverrideListResponse {
  @override
  final BuiltList<BranchRateOverrideResponse>? items;

  factory _$BranchRateOverrideListResponse(
          [void Function(BranchRateOverrideListResponseBuilder)? updates]) =>
      (BranchRateOverrideListResponseBuilder()..update(updates))._build();

  _$BranchRateOverrideListResponse._({this.items}) : super._();
  @override
  BranchRateOverrideListResponse rebuild(
          void Function(BranchRateOverrideListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchRateOverrideListResponseBuilder toBuilder() =>
      BranchRateOverrideListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchRateOverrideListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'BranchRateOverrideListResponse')
          ..add('items', items))
        .toString();
  }
}

class BranchRateOverrideListResponseBuilder
    implements
        Builder<BranchRateOverrideListResponse,
            BranchRateOverrideListResponseBuilder> {
  _$BranchRateOverrideListResponse? _$v;

  ListBuilder<BranchRateOverrideResponse>? _items;
  ListBuilder<BranchRateOverrideResponse> get items =>
      _$this._items ??= ListBuilder<BranchRateOverrideResponse>();
  set items(ListBuilder<BranchRateOverrideResponse>? items) =>
      _$this._items = items;

  BranchRateOverrideListResponseBuilder() {
    BranchRateOverrideListResponse._defaults(this);
  }

  BranchRateOverrideListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchRateOverrideListResponse other) {
    _$v = other as _$BranchRateOverrideListResponse;
  }

  @override
  void update(void Function(BranchRateOverrideListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchRateOverrideListResponse build() => _build();

  _$BranchRateOverrideListResponse _build() {
    _$BranchRateOverrideListResponse _$result;
    try {
      _$result = _$v ??
          _$BranchRateOverrideListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BranchRateOverrideListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
