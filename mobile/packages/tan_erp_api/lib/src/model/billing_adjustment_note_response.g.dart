// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_adjustment_note_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingAdjustmentNoteResponse extends BillingAdjustmentNoteResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? noteType;
  @override
  final String? reason;
  @override
  final double? taxBaseDelta;
  @override
  final double? vatRate;
  @override
  final double? vatDelta;
  @override
  final double? totalDelta;
  @override
  final String? status;
  @override
  final FinancePersonResponse? createdBy;
  @override
  final FinancePersonResponse? decidedBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? decidedAtUtc;

  factory _$BillingAdjustmentNoteResponse(
          [void Function(BillingAdjustmentNoteResponseBuilder)? updates]) =>
      (BillingAdjustmentNoteResponseBuilder()..update(updates))._build();

  _$BillingAdjustmentNoteResponse._(
      {this.id,
      this.number,
      this.noteType,
      this.reason,
      this.taxBaseDelta,
      this.vatRate,
      this.vatDelta,
      this.totalDelta,
      this.status,
      this.createdBy,
      this.decidedBy,
      this.createdAtUtc,
      this.decidedAtUtc})
      : super._();
  @override
  BillingAdjustmentNoteResponse rebuild(
          void Function(BillingAdjustmentNoteResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingAdjustmentNoteResponseBuilder toBuilder() =>
      BillingAdjustmentNoteResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingAdjustmentNoteResponse &&
        id == other.id &&
        number == other.number &&
        noteType == other.noteType &&
        reason == other.reason &&
        taxBaseDelta == other.taxBaseDelta &&
        vatRate == other.vatRate &&
        vatDelta == other.vatDelta &&
        totalDelta == other.totalDelta &&
        status == other.status &&
        createdBy == other.createdBy &&
        decidedBy == other.decidedBy &&
        createdAtUtc == other.createdAtUtc &&
        decidedAtUtc == other.decidedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, noteType.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, taxBaseDelta.hashCode);
    _$hash = $jc(_$hash, vatRate.hashCode);
    _$hash = $jc(_$hash, vatDelta.hashCode);
    _$hash = $jc(_$hash, totalDelta.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BillingAdjustmentNoteResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('noteType', noteType)
          ..add('reason', reason)
          ..add('taxBaseDelta', taxBaseDelta)
          ..add('vatRate', vatRate)
          ..add('vatDelta', vatDelta)
          ..add('totalDelta', totalDelta)
          ..add('status', status)
          ..add('createdBy', createdBy)
          ..add('decidedBy', decidedBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('decidedAtUtc', decidedAtUtc))
        .toString();
  }
}

class BillingAdjustmentNoteResponseBuilder
    implements
        Builder<BillingAdjustmentNoteResponse,
            BillingAdjustmentNoteResponseBuilder> {
  _$BillingAdjustmentNoteResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _noteType;
  String? get noteType => _$this._noteType;
  set noteType(String? noteType) => _$this._noteType = noteType;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  double? _taxBaseDelta;
  double? get taxBaseDelta => _$this._taxBaseDelta;
  set taxBaseDelta(double? taxBaseDelta) => _$this._taxBaseDelta = taxBaseDelta;

  double? _vatRate;
  double? get vatRate => _$this._vatRate;
  set vatRate(double? vatRate) => _$this._vatRate = vatRate;

  double? _vatDelta;
  double? get vatDelta => _$this._vatDelta;
  set vatDelta(double? vatDelta) => _$this._vatDelta = vatDelta;

  double? _totalDelta;
  double? get totalDelta => _$this._totalDelta;
  set totalDelta(double? totalDelta) => _$this._totalDelta = totalDelta;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  FinancePersonResponseBuilder? _createdBy;
  FinancePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= FinancePersonResponseBuilder();
  set createdBy(FinancePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  FinancePersonResponseBuilder? _decidedBy;
  FinancePersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= FinancePersonResponseBuilder();
  set decidedBy(FinancePersonResponseBuilder? decidedBy) =>
      _$this._decidedBy = decidedBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  BillingAdjustmentNoteResponseBuilder() {
    BillingAdjustmentNoteResponse._defaults(this);
  }

  BillingAdjustmentNoteResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _noteType = $v.noteType;
      _reason = $v.reason;
      _taxBaseDelta = $v.taxBaseDelta;
      _vatRate = $v.vatRate;
      _vatDelta = $v.vatDelta;
      _totalDelta = $v.totalDelta;
      _status = $v.status;
      _createdBy = $v.createdBy?.toBuilder();
      _decidedBy = $v.decidedBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _decidedAtUtc = $v.decidedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingAdjustmentNoteResponse other) {
    _$v = other as _$BillingAdjustmentNoteResponse;
  }

  @override
  void update(void Function(BillingAdjustmentNoteResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingAdjustmentNoteResponse build() => _build();

  _$BillingAdjustmentNoteResponse _build() {
    _$BillingAdjustmentNoteResponse _$result;
    try {
      _$result = _$v ??
          _$BillingAdjustmentNoteResponse._(
            id: id,
            number: number,
            noteType: noteType,
            reason: reason,
            taxBaseDelta: taxBaseDelta,
            vatRate: vatRate,
            vatDelta: vatDelta,
            totalDelta: totalDelta,
            status: status,
            createdBy: _createdBy?.build(),
            decidedBy: _decidedBy?.build(),
            createdAtUtc: createdAtUtc,
            decidedAtUtc: decidedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();
        _$failedField = 'decidedBy';
        _decidedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BillingAdjustmentNoteResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
