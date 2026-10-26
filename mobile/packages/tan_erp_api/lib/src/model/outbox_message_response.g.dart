// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'outbox_message_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OutboxMessageResponse extends OutboxMessageResponse {
  @override
  final String? id;
  @override
  final String? kind;
  @override
  final String? resourceNumber;
  @override
  final double? amount;
  @override
  final String? status;
  @override
  final int? attempts;
  @override
  final DateTime? nextAttemptAtUtc;
  @override
  final String? lastError;
  @override
  final String? externalRef;
  @override
  final double? externalAmount;
  @override
  final DateTime? confirmedAtUtc;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? sentAtUtc;
  @override
  final String? rowVersion;

  factory _$OutboxMessageResponse(
          [void Function(OutboxMessageResponseBuilder)? updates]) =>
      (OutboxMessageResponseBuilder()..update(updates))._build();

  _$OutboxMessageResponse._(
      {this.id,
      this.kind,
      this.resourceNumber,
      this.amount,
      this.status,
      this.attempts,
      this.nextAttemptAtUtc,
      this.lastError,
      this.externalRef,
      this.externalAmount,
      this.confirmedAtUtc,
      this.createdAtUtc,
      this.sentAtUtc,
      this.rowVersion})
      : super._();
  @override
  OutboxMessageResponse rebuild(
          void Function(OutboxMessageResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OutboxMessageResponseBuilder toBuilder() =>
      OutboxMessageResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OutboxMessageResponse &&
        id == other.id &&
        kind == other.kind &&
        resourceNumber == other.resourceNumber &&
        amount == other.amount &&
        status == other.status &&
        attempts == other.attempts &&
        nextAttemptAtUtc == other.nextAttemptAtUtc &&
        lastError == other.lastError &&
        externalRef == other.externalRef &&
        externalAmount == other.externalAmount &&
        confirmedAtUtc == other.confirmedAtUtc &&
        createdAtUtc == other.createdAtUtc &&
        sentAtUtc == other.sentAtUtc &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, resourceNumber.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, attempts.hashCode);
    _$hash = $jc(_$hash, nextAttemptAtUtc.hashCode);
    _$hash = $jc(_$hash, lastError.hashCode);
    _$hash = $jc(_$hash, externalRef.hashCode);
    _$hash = $jc(_$hash, externalAmount.hashCode);
    _$hash = $jc(_$hash, confirmedAtUtc.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, sentAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OutboxMessageResponse')
          ..add('id', id)
          ..add('kind', kind)
          ..add('resourceNumber', resourceNumber)
          ..add('amount', amount)
          ..add('status', status)
          ..add('attempts', attempts)
          ..add('nextAttemptAtUtc', nextAttemptAtUtc)
          ..add('lastError', lastError)
          ..add('externalRef', externalRef)
          ..add('externalAmount', externalAmount)
          ..add('confirmedAtUtc', confirmedAtUtc)
          ..add('createdAtUtc', createdAtUtc)
          ..add('sentAtUtc', sentAtUtc)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class OutboxMessageResponseBuilder
    implements Builder<OutboxMessageResponse, OutboxMessageResponseBuilder> {
  _$OutboxMessageResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _resourceNumber;
  String? get resourceNumber => _$this._resourceNumber;
  set resourceNumber(String? resourceNumber) =>
      _$this._resourceNumber = resourceNumber;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  int? _attempts;
  int? get attempts => _$this._attempts;
  set attempts(int? attempts) => _$this._attempts = attempts;

  DateTime? _nextAttemptAtUtc;
  DateTime? get nextAttemptAtUtc => _$this._nextAttemptAtUtc;
  set nextAttemptAtUtc(DateTime? nextAttemptAtUtc) =>
      _$this._nextAttemptAtUtc = nextAttemptAtUtc;

  String? _lastError;
  String? get lastError => _$this._lastError;
  set lastError(String? lastError) => _$this._lastError = lastError;

  String? _externalRef;
  String? get externalRef => _$this._externalRef;
  set externalRef(String? externalRef) => _$this._externalRef = externalRef;

  double? _externalAmount;
  double? get externalAmount => _$this._externalAmount;
  set externalAmount(double? externalAmount) =>
      _$this._externalAmount = externalAmount;

  DateTime? _confirmedAtUtc;
  DateTime? get confirmedAtUtc => _$this._confirmedAtUtc;
  set confirmedAtUtc(DateTime? confirmedAtUtc) =>
      _$this._confirmedAtUtc = confirmedAtUtc;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _sentAtUtc;
  DateTime? get sentAtUtc => _$this._sentAtUtc;
  set sentAtUtc(DateTime? sentAtUtc) => _$this._sentAtUtc = sentAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  OutboxMessageResponseBuilder() {
    OutboxMessageResponse._defaults(this);
  }

  OutboxMessageResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _kind = $v.kind;
      _resourceNumber = $v.resourceNumber;
      _amount = $v.amount;
      _status = $v.status;
      _attempts = $v.attempts;
      _nextAttemptAtUtc = $v.nextAttemptAtUtc;
      _lastError = $v.lastError;
      _externalRef = $v.externalRef;
      _externalAmount = $v.externalAmount;
      _confirmedAtUtc = $v.confirmedAtUtc;
      _createdAtUtc = $v.createdAtUtc;
      _sentAtUtc = $v.sentAtUtc;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OutboxMessageResponse other) {
    _$v = other as _$OutboxMessageResponse;
  }

  @override
  void update(void Function(OutboxMessageResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OutboxMessageResponse build() => _build();

  _$OutboxMessageResponse _build() {
    final _$result = _$v ??
        _$OutboxMessageResponse._(
          id: id,
          kind: kind,
          resourceNumber: resourceNumber,
          amount: amount,
          status: status,
          attempts: attempts,
          nextAttemptAtUtc: nextAttemptAtUtc,
          lastError: lastError,
          externalRef: externalRef,
          externalAmount: externalAmount,
          confirmedAtUtc: confirmedAtUtc,
          createdAtUtc: createdAtUtc,
          sentAtUtc: sentAtUtc,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
