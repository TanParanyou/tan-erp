// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomLineResponse extends BomLineResponse {
  @override
  final String? id;
  @override
  final ProductionItemResponse? component;
  @override
  final double? quantity;
  @override
  final double? scrapPercent;
  @override
  final double? grossQuantity;

  factory _$BomLineResponse([void Function(BomLineResponseBuilder)? updates]) =>
      (BomLineResponseBuilder()..update(updates))._build();

  _$BomLineResponse._(
      {this.id,
      this.component,
      this.quantity,
      this.scrapPercent,
      this.grossQuantity})
      : super._();
  @override
  BomLineResponse rebuild(void Function(BomLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomLineResponseBuilder toBuilder() => BomLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomLineResponse &&
        id == other.id &&
        component == other.component &&
        quantity == other.quantity &&
        scrapPercent == other.scrapPercent &&
        grossQuantity == other.grossQuantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, component.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, scrapPercent.hashCode);
    _$hash = $jc(_$hash, grossQuantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomLineResponse')
          ..add('id', id)
          ..add('component', component)
          ..add('quantity', quantity)
          ..add('scrapPercent', scrapPercent)
          ..add('grossQuantity', grossQuantity))
        .toString();
  }
}

class BomLineResponseBuilder
    implements Builder<BomLineResponse, BomLineResponseBuilder> {
  _$BomLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  ProductionItemResponseBuilder? _component;
  ProductionItemResponseBuilder get component =>
      _$this._component ??= ProductionItemResponseBuilder();
  set component(ProductionItemResponseBuilder? component) =>
      _$this._component = component;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _scrapPercent;
  double? get scrapPercent => _$this._scrapPercent;
  set scrapPercent(double? scrapPercent) => _$this._scrapPercent = scrapPercent;

  double? _grossQuantity;
  double? get grossQuantity => _$this._grossQuantity;
  set grossQuantity(double? grossQuantity) =>
      _$this._grossQuantity = grossQuantity;

  BomLineResponseBuilder() {
    BomLineResponse._defaults(this);
  }

  BomLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _component = $v.component?.toBuilder();
      _quantity = $v.quantity;
      _scrapPercent = $v.scrapPercent;
      _grossQuantity = $v.grossQuantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomLineResponse other) {
    _$v = other as _$BomLineResponse;
  }

  @override
  void update(void Function(BomLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomLineResponse build() => _build();

  _$BomLineResponse _build() {
    _$BomLineResponse _$result;
    try {
      _$result = _$v ??
          _$BomLineResponse._(
            id: id,
            component: _component?.build(),
            quantity: quantity,
            scrapPercent: scrapPercent,
            grossQuantity: grossQuantity,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'component';
        _component?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BomLineResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
