// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_convert_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpConvertRequest extends MrpConvertRequest {
  @override
  final String? supplierId;
  @override
  final double? unitPrice;
  @override
  final String? warehouseId;

  factory _$MrpConvertRequest(
          [void Function(MrpConvertRequestBuilder)? updates]) =>
      (MrpConvertRequestBuilder()..update(updates))._build();

  _$MrpConvertRequest._({this.supplierId, this.unitPrice, this.warehouseId})
      : super._();
  @override
  MrpConvertRequest rebuild(void Function(MrpConvertRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpConvertRequestBuilder toBuilder() =>
      MrpConvertRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpConvertRequest &&
        supplierId == other.supplierId &&
        unitPrice == other.unitPrice &&
        warehouseId == other.warehouseId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, supplierId.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpConvertRequest')
          ..add('supplierId', supplierId)
          ..add('unitPrice', unitPrice)
          ..add('warehouseId', warehouseId))
        .toString();
  }
}

class MrpConvertRequestBuilder
    implements Builder<MrpConvertRequest, MrpConvertRequestBuilder> {
  _$MrpConvertRequest? _$v;

  String? _supplierId;
  String? get supplierId => _$this._supplierId;
  set supplierId(String? supplierId) => _$this._supplierId = supplierId;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  MrpConvertRequestBuilder() {
    MrpConvertRequest._defaults(this);
  }

  MrpConvertRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _supplierId = $v.supplierId;
      _unitPrice = $v.unitPrice;
      _warehouseId = $v.warehouseId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpConvertRequest other) {
    _$v = other as _$MrpConvertRequest;
  }

  @override
  void update(void Function(MrpConvertRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpConvertRequest build() => _build();

  _$MrpConvertRequest _build() {
    final _$result = _$v ??
        _$MrpConvertRequest._(
          supplierId: supplierId,
          unitPrice: unitPrice,
          warehouseId: warehouseId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
