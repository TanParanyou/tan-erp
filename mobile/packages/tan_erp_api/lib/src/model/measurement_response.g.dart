// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'measurement_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MeasurementResponse extends MeasurementResponse {
  @override
  final String? lineId;
  @override
  final String? workSubtype;
  @override
  final double? widthM;
  @override
  final double? heightM;
  @override
  final double? depthM;
  @override
  final double? quantity;

  factory _$MeasurementResponse(
          [void Function(MeasurementResponseBuilder)? updates]) =>
      (MeasurementResponseBuilder()..update(updates))._build();

  _$MeasurementResponse._(
      {this.lineId,
      this.workSubtype,
      this.widthM,
      this.heightM,
      this.depthM,
      this.quantity})
      : super._();
  @override
  MeasurementResponse rebuild(
          void Function(MeasurementResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MeasurementResponseBuilder toBuilder() =>
      MeasurementResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MeasurementResponse &&
        lineId == other.lineId &&
        workSubtype == other.workSubtype &&
        widthM == other.widthM &&
        heightM == other.heightM &&
        depthM == other.depthM &&
        quantity == other.quantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, lineId.hashCode);
    _$hash = $jc(_$hash, workSubtype.hashCode);
    _$hash = $jc(_$hash, widthM.hashCode);
    _$hash = $jc(_$hash, heightM.hashCode);
    _$hash = $jc(_$hash, depthM.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MeasurementResponse')
          ..add('lineId', lineId)
          ..add('workSubtype', workSubtype)
          ..add('widthM', widthM)
          ..add('heightM', heightM)
          ..add('depthM', depthM)
          ..add('quantity', quantity))
        .toString();
  }
}

class MeasurementResponseBuilder
    implements Builder<MeasurementResponse, MeasurementResponseBuilder> {
  _$MeasurementResponse? _$v;

  String? _lineId;
  String? get lineId => _$this._lineId;
  set lineId(String? lineId) => _$this._lineId = lineId;

  String? _workSubtype;
  String? get workSubtype => _$this._workSubtype;
  set workSubtype(String? workSubtype) => _$this._workSubtype = workSubtype;

  double? _widthM;
  double? get widthM => _$this._widthM;
  set widthM(double? widthM) => _$this._widthM = widthM;

  double? _heightM;
  double? get heightM => _$this._heightM;
  set heightM(double? heightM) => _$this._heightM = heightM;

  double? _depthM;
  double? get depthM => _$this._depthM;
  set depthM(double? depthM) => _$this._depthM = depthM;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  MeasurementResponseBuilder() {
    MeasurementResponse._defaults(this);
  }

  MeasurementResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _lineId = $v.lineId;
      _workSubtype = $v.workSubtype;
      _widthM = $v.widthM;
      _heightM = $v.heightM;
      _depthM = $v.depthM;
      _quantity = $v.quantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MeasurementResponse other) {
    _$v = other as _$MeasurementResponse;
  }

  @override
  void update(void Function(MeasurementResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MeasurementResponse build() => _build();

  _$MeasurementResponse _build() {
    final _$result = _$v ??
        _$MeasurementResponse._(
          lineId: lineId,
          workSubtype: workSubtype,
          widthM: widthM,
          heightM: heightM,
          depthM: depthM,
          quantity: quantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
