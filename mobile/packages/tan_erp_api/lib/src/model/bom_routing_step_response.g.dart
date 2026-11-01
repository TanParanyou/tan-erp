// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_routing_step_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomRoutingStepResponse extends BomRoutingStepResponse {
  @override
  final String? id;
  @override
  final int? sequence;
  @override
  final String? operationCode;
  @override
  final String? operationName;
  @override
  final WorkCenterRefResponse? workCenter;
  @override
  final double? setupHours;
  @override
  final double? runHours;
  @override
  final bool? isSubcontracted;
  @override
  final double? stdSubcontractCost;
  @override
  final String? note;

  factory _$BomRoutingStepResponse(
          [void Function(BomRoutingStepResponseBuilder)? updates]) =>
      (BomRoutingStepResponseBuilder()..update(updates))._build();

  _$BomRoutingStepResponse._(
      {this.id,
      this.sequence,
      this.operationCode,
      this.operationName,
      this.workCenter,
      this.setupHours,
      this.runHours,
      this.isSubcontracted,
      this.stdSubcontractCost,
      this.note})
      : super._();
  @override
  BomRoutingStepResponse rebuild(
          void Function(BomRoutingStepResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomRoutingStepResponseBuilder toBuilder() =>
      BomRoutingStepResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomRoutingStepResponse &&
        id == other.id &&
        sequence == other.sequence &&
        operationCode == other.operationCode &&
        operationName == other.operationName &&
        workCenter == other.workCenter &&
        setupHours == other.setupHours &&
        runHours == other.runHours &&
        isSubcontracted == other.isSubcontracted &&
        stdSubcontractCost == other.stdSubcontractCost &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, sequence.hashCode);
    _$hash = $jc(_$hash, operationCode.hashCode);
    _$hash = $jc(_$hash, operationName.hashCode);
    _$hash = $jc(_$hash, workCenter.hashCode);
    _$hash = $jc(_$hash, setupHours.hashCode);
    _$hash = $jc(_$hash, runHours.hashCode);
    _$hash = $jc(_$hash, isSubcontracted.hashCode);
    _$hash = $jc(_$hash, stdSubcontractCost.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomRoutingStepResponse')
          ..add('id', id)
          ..add('sequence', sequence)
          ..add('operationCode', operationCode)
          ..add('operationName', operationName)
          ..add('workCenter', workCenter)
          ..add('setupHours', setupHours)
          ..add('runHours', runHours)
          ..add('isSubcontracted', isSubcontracted)
          ..add('stdSubcontractCost', stdSubcontractCost)
          ..add('note', note))
        .toString();
  }
}

class BomRoutingStepResponseBuilder
    implements Builder<BomRoutingStepResponse, BomRoutingStepResponseBuilder> {
  _$BomRoutingStepResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _sequence;
  int? get sequence => _$this._sequence;
  set sequence(int? sequence) => _$this._sequence = sequence;

  String? _operationCode;
  String? get operationCode => _$this._operationCode;
  set operationCode(String? operationCode) =>
      _$this._operationCode = operationCode;

  String? _operationName;
  String? get operationName => _$this._operationName;
  set operationName(String? operationName) =>
      _$this._operationName = operationName;

  WorkCenterRefResponseBuilder? _workCenter;
  WorkCenterRefResponseBuilder get workCenter =>
      _$this._workCenter ??= WorkCenterRefResponseBuilder();
  set workCenter(WorkCenterRefResponseBuilder? workCenter) =>
      _$this._workCenter = workCenter;

  double? _setupHours;
  double? get setupHours => _$this._setupHours;
  set setupHours(double? setupHours) => _$this._setupHours = setupHours;

  double? _runHours;
  double? get runHours => _$this._runHours;
  set runHours(double? runHours) => _$this._runHours = runHours;

  bool? _isSubcontracted;
  bool? get isSubcontracted => _$this._isSubcontracted;
  set isSubcontracted(bool? isSubcontracted) =>
      _$this._isSubcontracted = isSubcontracted;

  double? _stdSubcontractCost;
  double? get stdSubcontractCost => _$this._stdSubcontractCost;
  set stdSubcontractCost(double? stdSubcontractCost) =>
      _$this._stdSubcontractCost = stdSubcontractCost;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  BomRoutingStepResponseBuilder() {
    BomRoutingStepResponse._defaults(this);
  }

  BomRoutingStepResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _sequence = $v.sequence;
      _operationCode = $v.operationCode;
      _operationName = $v.operationName;
      _workCenter = $v.workCenter?.toBuilder();
      _setupHours = $v.setupHours;
      _runHours = $v.runHours;
      _isSubcontracted = $v.isSubcontracted;
      _stdSubcontractCost = $v.stdSubcontractCost;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomRoutingStepResponse other) {
    _$v = other as _$BomRoutingStepResponse;
  }

  @override
  void update(void Function(BomRoutingStepResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomRoutingStepResponse build() => _build();

  _$BomRoutingStepResponse _build() {
    _$BomRoutingStepResponse _$result;
    try {
      _$result = _$v ??
          _$BomRoutingStepResponse._(
            id: id,
            sequence: sequence,
            operationCode: operationCode,
            operationName: operationName,
            workCenter: _workCenter?.build(),
            setupHours: setupHours,
            runHours: runHours,
            isSubcontracted: isSubcontracted,
            stdSubcontractCost: stdSubcontractCost,
            note: note,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workCenter';
        _workCenter?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BomRoutingStepResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
