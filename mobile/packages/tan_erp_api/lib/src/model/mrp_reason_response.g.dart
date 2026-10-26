// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_reason_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpReasonResponse extends MrpReasonResponse {
  @override
  final String? sourceType;
  @override
  final String? sourceRef;
  @override
  final double? quantity;
  @override
  final Date? needBy;

  factory _$MrpReasonResponse(
          [void Function(MrpReasonResponseBuilder)? updates]) =>
      (MrpReasonResponseBuilder()..update(updates))._build();

  _$MrpReasonResponse._(
      {this.sourceType, this.sourceRef, this.quantity, this.needBy})
      : super._();
  @override
  MrpReasonResponse rebuild(void Function(MrpReasonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpReasonResponseBuilder toBuilder() =>
      MrpReasonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpReasonResponse &&
        sourceType == other.sourceType &&
        sourceRef == other.sourceRef &&
        quantity == other.quantity &&
        needBy == other.needBy;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sourceType.hashCode);
    _$hash = $jc(_$hash, sourceRef.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, needBy.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpReasonResponse')
          ..add('sourceType', sourceType)
          ..add('sourceRef', sourceRef)
          ..add('quantity', quantity)
          ..add('needBy', needBy))
        .toString();
  }
}

class MrpReasonResponseBuilder
    implements Builder<MrpReasonResponse, MrpReasonResponseBuilder> {
  _$MrpReasonResponse? _$v;

  String? _sourceType;
  String? get sourceType => _$this._sourceType;
  set sourceType(String? sourceType) => _$this._sourceType = sourceType;

  String? _sourceRef;
  String? get sourceRef => _$this._sourceRef;
  set sourceRef(String? sourceRef) => _$this._sourceRef = sourceRef;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  Date? _needBy;
  Date? get needBy => _$this._needBy;
  set needBy(Date? needBy) => _$this._needBy = needBy;

  MrpReasonResponseBuilder() {
    MrpReasonResponse._defaults(this);
  }

  MrpReasonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sourceType = $v.sourceType;
      _sourceRef = $v.sourceRef;
      _quantity = $v.quantity;
      _needBy = $v.needBy;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpReasonResponse other) {
    _$v = other as _$MrpReasonResponse;
  }

  @override
  void update(void Function(MrpReasonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpReasonResponse build() => _build();

  _$MrpReasonResponse _build() {
    final _$result = _$v ??
        _$MrpReasonResponse._(
          sourceType: sourceType,
          sourceRef: sourceRef,
          quantity: quantity,
          needBy: needBy,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
