// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calculation_policy_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CalculationPolicyListResponse extends CalculationPolicyListResponse {
  @override
  final BuiltList<CalculationPolicyVersionResponse>? items;

  factory _$CalculationPolicyListResponse(
          [void Function(CalculationPolicyListResponseBuilder)? updates]) =>
      (CalculationPolicyListResponseBuilder()..update(updates))._build();

  _$CalculationPolicyListResponse._({this.items}) : super._();
  @override
  CalculationPolicyListResponse rebuild(
          void Function(CalculationPolicyListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CalculationPolicyListResponseBuilder toBuilder() =>
      CalculationPolicyListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CalculationPolicyListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'CalculationPolicyListResponse')
          ..add('items', items))
        .toString();
  }
}

class CalculationPolicyListResponseBuilder
    implements
        Builder<CalculationPolicyListResponse,
            CalculationPolicyListResponseBuilder> {
  _$CalculationPolicyListResponse? _$v;

  ListBuilder<CalculationPolicyVersionResponse>? _items;
  ListBuilder<CalculationPolicyVersionResponse> get items =>
      _$this._items ??= ListBuilder<CalculationPolicyVersionResponse>();
  set items(ListBuilder<CalculationPolicyVersionResponse>? items) =>
      _$this._items = items;

  CalculationPolicyListResponseBuilder() {
    CalculationPolicyListResponse._defaults(this);
  }

  CalculationPolicyListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CalculationPolicyListResponse other) {
    _$v = other as _$CalculationPolicyListResponse;
  }

  @override
  void update(void Function(CalculationPolicyListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CalculationPolicyListResponse build() => _build();

  _$CalculationPolicyListResponse _build() {
    _$CalculationPolicyListResponse _$result;
    try {
      _$result = _$v ??
          _$CalculationPolicyListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CalculationPolicyListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
