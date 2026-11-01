// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'journal_export_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$JournalExportLineResponse extends JournalExportLineResponse {
  @override
  final String? batchRef;
  @override
  final String? messageId;
  @override
  final String? dedupeKey;
  @override
  final String? kind;
  @override
  final String? resourceNumber;
  @override
  final String? debitAccountCode;
  @override
  final String? debitAccountName;
  @override
  final String? creditAccountCode;
  @override
  final String? creditAccountName;
  @override
  final double? amount;
  @override
  final String? currency;
  @override
  final DateTime? createdAtUtc;

  factory _$JournalExportLineResponse(
          [void Function(JournalExportLineResponseBuilder)? updates]) =>
      (JournalExportLineResponseBuilder()..update(updates))._build();

  _$JournalExportLineResponse._(
      {this.batchRef,
      this.messageId,
      this.dedupeKey,
      this.kind,
      this.resourceNumber,
      this.debitAccountCode,
      this.debitAccountName,
      this.creditAccountCode,
      this.creditAccountName,
      this.amount,
      this.currency,
      this.createdAtUtc})
      : super._();
  @override
  JournalExportLineResponse rebuild(
          void Function(JournalExportLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  JournalExportLineResponseBuilder toBuilder() =>
      JournalExportLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is JournalExportLineResponse &&
        batchRef == other.batchRef &&
        messageId == other.messageId &&
        dedupeKey == other.dedupeKey &&
        kind == other.kind &&
        resourceNumber == other.resourceNumber &&
        debitAccountCode == other.debitAccountCode &&
        debitAccountName == other.debitAccountName &&
        creditAccountCode == other.creditAccountCode &&
        creditAccountName == other.creditAccountName &&
        amount == other.amount &&
        currency == other.currency &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, batchRef.hashCode);
    _$hash = $jc(_$hash, messageId.hashCode);
    _$hash = $jc(_$hash, dedupeKey.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, resourceNumber.hashCode);
    _$hash = $jc(_$hash, debitAccountCode.hashCode);
    _$hash = $jc(_$hash, debitAccountName.hashCode);
    _$hash = $jc(_$hash, creditAccountCode.hashCode);
    _$hash = $jc(_$hash, creditAccountName.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'JournalExportLineResponse')
          ..add('batchRef', batchRef)
          ..add('messageId', messageId)
          ..add('dedupeKey', dedupeKey)
          ..add('kind', kind)
          ..add('resourceNumber', resourceNumber)
          ..add('debitAccountCode', debitAccountCode)
          ..add('debitAccountName', debitAccountName)
          ..add('creditAccountCode', creditAccountCode)
          ..add('creditAccountName', creditAccountName)
          ..add('amount', amount)
          ..add('currency', currency)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class JournalExportLineResponseBuilder
    implements
        Builder<JournalExportLineResponse, JournalExportLineResponseBuilder> {
  _$JournalExportLineResponse? _$v;

  String? _batchRef;
  String? get batchRef => _$this._batchRef;
  set batchRef(String? batchRef) => _$this._batchRef = batchRef;

  String? _messageId;
  String? get messageId => _$this._messageId;
  set messageId(String? messageId) => _$this._messageId = messageId;

  String? _dedupeKey;
  String? get dedupeKey => _$this._dedupeKey;
  set dedupeKey(String? dedupeKey) => _$this._dedupeKey = dedupeKey;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _resourceNumber;
  String? get resourceNumber => _$this._resourceNumber;
  set resourceNumber(String? resourceNumber) =>
      _$this._resourceNumber = resourceNumber;

  String? _debitAccountCode;
  String? get debitAccountCode => _$this._debitAccountCode;
  set debitAccountCode(String? debitAccountCode) =>
      _$this._debitAccountCode = debitAccountCode;

  String? _debitAccountName;
  String? get debitAccountName => _$this._debitAccountName;
  set debitAccountName(String? debitAccountName) =>
      _$this._debitAccountName = debitAccountName;

  String? _creditAccountCode;
  String? get creditAccountCode => _$this._creditAccountCode;
  set creditAccountCode(String? creditAccountCode) =>
      _$this._creditAccountCode = creditAccountCode;

  String? _creditAccountName;
  String? get creditAccountName => _$this._creditAccountName;
  set creditAccountName(String? creditAccountName) =>
      _$this._creditAccountName = creditAccountName;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  JournalExportLineResponseBuilder() {
    JournalExportLineResponse._defaults(this);
  }

  JournalExportLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _batchRef = $v.batchRef;
      _messageId = $v.messageId;
      _dedupeKey = $v.dedupeKey;
      _kind = $v.kind;
      _resourceNumber = $v.resourceNumber;
      _debitAccountCode = $v.debitAccountCode;
      _debitAccountName = $v.debitAccountName;
      _creditAccountCode = $v.creditAccountCode;
      _creditAccountName = $v.creditAccountName;
      _amount = $v.amount;
      _currency = $v.currency;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(JournalExportLineResponse other) {
    _$v = other as _$JournalExportLineResponse;
  }

  @override
  void update(void Function(JournalExportLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  JournalExportLineResponse build() => _build();

  _$JournalExportLineResponse _build() {
    final _$result = _$v ??
        _$JournalExportLineResponse._(
          batchRef: batchRef,
          messageId: messageId,
          dedupeKey: dedupeKey,
          kind: kind,
          resourceNumber: resourceNumber,
          debitAccountCode: debitAccountCode,
          debitAccountName: debitAccountName,
          creditAccountCode: creditAccountCode,
          creditAccountName: creditAccountName,
          amount: amount,
          currency: currency,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
