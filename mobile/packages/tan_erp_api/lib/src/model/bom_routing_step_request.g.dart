// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_routing_step_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomRoutingStepRequest extends BomRoutingStepRequest {
  @override
  final int? sequence;
  @override
  final String? operationCode;
  @override
  final String? operationName;
  @override
  final String? workCenterId;
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

  factory _$BomRoutingStepRequest(
          [void Function(BomRoutingStepRequestBuilder)? updates]) =>
      (BomRoutingStepRequestBuilder()..update(updates))._build();

  _$BomRoutingStepRequest._(
      {this.sequence,
      this.operationCode,
      this.operationName,
      this.workCenterId,
      this.setupHours,
      this.runHours,
      this.isSubcontracted,
      this.stdSubcontractCost,
      this.note})
      : super._();
  @override
  BomRoutingStepRequest rebuild(
          void Function(BomRoutingStepRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomRoutingStepRequestBuilder toBuilder() =>
      BomRoutingStepRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomRoutingStepRequest &&
        sequence == other.sequence &&
        operationCode == other.operationCode &&
        operationName == other.operationName &&
        workCenterId == other.workCenterId &&
        setupHours == other.setupHours &&
        runHours == other.runHours &&
        isSubcontracted == other.isSubcontracted &&
        stdSubcontractCost == other.stdSubcontractCost &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sequence.hashCode);
    _$hash = $jc(_$hash, operationCode.hashCode);
    _$hash = $jc(_$hash, operationName.hashCode);
    _$hash = $jc(_$hash, workCenterId.hashCode);
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
    return (newBuiltValueToStringHelper(r'BomRoutingStepRequest')
          ..add('sequence', sequence)
          ..add('operationCode', operationCode)
          ..add('operationName', operationName)
          ..add('workCenterId', workCenterId)
          ..add('setupHours', setupHours)
          ..add('runHours', runHours)
          ..add('isSubcontracted', isSubcontracted)
          ..add('stdSubcontractCost', stdSubcontractCost)
          ..add('note', note))
        .toString();
  }
}

class BomRoutingStepRequestBuilder
    implements Builder<BomRoutingStepRequest, BomRoutingStepRequestBuilder> {
  _$BomRoutingStepRequest? _$v;

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

  String? _workCenterId;
  String? get workCenterId => _$this._workCenterId;
  set workCenterId(String? workCenterId) => _$this._workCenterId = workCenterId;

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

  BomRoutingStepRequestBuilder() {
    BomRoutingStepRequest._defaults(this);
  }

  BomRoutingStepRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sequence = $v.sequence;
      _operationCode = $v.operationCode;
      _operationName = $v.operationName;
      _workCenterId = $v.workCenterId;
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
  void replace(BomRoutingStepRequest other) {
    _$v = other as _$BomRoutingStepRequest;
  }

  @override
  void update(void Function(BomRoutingStepRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomRoutingStepRequest build() => _build();

  _$BomRoutingStepRequest _build() {
    final _$result = _$v ??
        _$BomRoutingStepRequest._(
          sequence: sequence,
          operationCode: operationCode,
          operationName: operationName,
          workCenterId: workCenterId,
          setupHours: setupHours,
          runHours: runHours,
          isSubcontracted: isSubcontracted,
          stdSubcontractCost: stdSubcontractCost,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
