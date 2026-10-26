// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calculation_snapshot_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CalculationSnapshotResponse extends CalculationSnapshotResponse {
  @override
  final int? version;
  @override
  final String? snapshotJson;

  factory _$CalculationSnapshotResponse(
          [void Function(CalculationSnapshotResponseBuilder)? updates]) =>
      (CalculationSnapshotResponseBuilder()..update(updates))._build();

  _$CalculationSnapshotResponse._({this.version, this.snapshotJson})
      : super._();
  @override
  CalculationSnapshotResponse rebuild(
          void Function(CalculationSnapshotResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CalculationSnapshotResponseBuilder toBuilder() =>
      CalculationSnapshotResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CalculationSnapshotResponse &&
        version == other.version &&
        snapshotJson == other.snapshotJson;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, snapshotJson.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CalculationSnapshotResponse')
          ..add('version', version)
          ..add('snapshotJson', snapshotJson))
        .toString();
  }
}

class CalculationSnapshotResponseBuilder
    implements
        Builder<CalculationSnapshotResponse,
            CalculationSnapshotResponseBuilder> {
  _$CalculationSnapshotResponse? _$v;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _snapshotJson;
  String? get snapshotJson => _$this._snapshotJson;
  set snapshotJson(String? snapshotJson) => _$this._snapshotJson = snapshotJson;

  CalculationSnapshotResponseBuilder() {
    CalculationSnapshotResponse._defaults(this);
  }

  CalculationSnapshotResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _version = $v.version;
      _snapshotJson = $v.snapshotJson;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CalculationSnapshotResponse other) {
    _$v = other as _$CalculationSnapshotResponse;
  }

  @override
  void update(void Function(CalculationSnapshotResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CalculationSnapshotResponse build() => _build();

  _$CalculationSnapshotResponse _build() {
    final _$result = _$v ??
        _$CalculationSnapshotResponse._(
          version: version,
          snapshotJson: snapshotJson,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
