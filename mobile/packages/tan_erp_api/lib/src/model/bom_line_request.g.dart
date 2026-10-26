// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomLineRequest extends BomLineRequest {
  @override
  final String? componentItemId;
  @override
  final double? quantity;
  @override
  final double? scrapPercent;

  factory _$BomLineRequest([void Function(BomLineRequestBuilder)? updates]) =>
      (BomLineRequestBuilder()..update(updates))._build();

  _$BomLineRequest._({this.componentItemId, this.quantity, this.scrapPercent})
      : super._();
  @override
  BomLineRequest rebuild(void Function(BomLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomLineRequestBuilder toBuilder() => BomLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomLineRequest &&
        componentItemId == other.componentItemId &&
        quantity == other.quantity &&
        scrapPercent == other.scrapPercent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, componentItemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, scrapPercent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomLineRequest')
          ..add('componentItemId', componentItemId)
          ..add('quantity', quantity)
          ..add('scrapPercent', scrapPercent))
        .toString();
  }
}

class BomLineRequestBuilder
    implements Builder<BomLineRequest, BomLineRequestBuilder> {
  _$BomLineRequest? _$v;

  String? _componentItemId;
  String? get componentItemId => _$this._componentItemId;
  set componentItemId(String? componentItemId) =>
      _$this._componentItemId = componentItemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _scrapPercent;
  double? get scrapPercent => _$this._scrapPercent;
  set scrapPercent(double? scrapPercent) => _$this._scrapPercent = scrapPercent;

  BomLineRequestBuilder() {
    BomLineRequest._defaults(this);
  }

  BomLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _componentItemId = $v.componentItemId;
      _quantity = $v.quantity;
      _scrapPercent = $v.scrapPercent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomLineRequest other) {
    _$v = other as _$BomLineRequest;
  }

  @override
  void update(void Function(BomLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomLineRequest build() => _build();

  _$BomLineRequest _build() {
    final _$result = _$v ??
        _$BomLineRequest._(
          componentItemId: componentItemId,
          quantity: quantity,
          scrapPercent: scrapPercent,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
