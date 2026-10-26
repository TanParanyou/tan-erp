// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_demand_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpDemandRequest extends MrpDemandRequest {
  @override
  final String? itemId;
  @override
  final double? quantity;
  @override
  final Date? needBy;
  @override
  final String? reference;

  factory _$MrpDemandRequest(
          [void Function(MrpDemandRequestBuilder)? updates]) =>
      (MrpDemandRequestBuilder()..update(updates))._build();

  _$MrpDemandRequest._(
      {this.itemId, this.quantity, this.needBy, this.reference})
      : super._();
  @override
  MrpDemandRequest rebuild(void Function(MrpDemandRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpDemandRequestBuilder toBuilder() =>
      MrpDemandRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpDemandRequest &&
        itemId == other.itemId &&
        quantity == other.quantity &&
        needBy == other.needBy &&
        reference == other.reference;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, needBy.hashCode);
    _$hash = $jc(_$hash, reference.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpDemandRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity)
          ..add('needBy', needBy)
          ..add('reference', reference))
        .toString();
  }
}

class MrpDemandRequestBuilder
    implements Builder<MrpDemandRequest, MrpDemandRequestBuilder> {
  _$MrpDemandRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  Date? _needBy;
  Date? get needBy => _$this._needBy;
  set needBy(Date? needBy) => _$this._needBy = needBy;

  String? _reference;
  String? get reference => _$this._reference;
  set reference(String? reference) => _$this._reference = reference;

  MrpDemandRequestBuilder() {
    MrpDemandRequest._defaults(this);
  }

  MrpDemandRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
      _needBy = $v.needBy;
      _reference = $v.reference;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpDemandRequest other) {
    _$v = other as _$MrpDemandRequest;
  }

  @override
  void update(void Function(MrpDemandRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpDemandRequest build() => _build();

  _$MrpDemandRequest _build() {
    final _$result = _$v ??
        _$MrpDemandRequest._(
          itemId: itemId,
          quantity: quantity,
          needBy: needBy,
          reference: reference,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
