// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'finance_reconciliation_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$FinanceReconciliationRowResponse
    extends FinanceReconciliationRowResponse {
  @override
  final String? issue;
  @override
  final String? kind;
  @override
  final String? resourceNumber;
  @override
  final String? messageId;
  @override
  final double? erpAmount;
  @override
  final double? externalAmount;

  factory _$FinanceReconciliationRowResponse(
          [void Function(FinanceReconciliationRowResponseBuilder)? updates]) =>
      (FinanceReconciliationRowResponseBuilder()..update(updates))._build();

  _$FinanceReconciliationRowResponse._(
      {this.issue,
      this.kind,
      this.resourceNumber,
      this.messageId,
      this.erpAmount,
      this.externalAmount})
      : super._();
  @override
  FinanceReconciliationRowResponse rebuild(
          void Function(FinanceReconciliationRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  FinanceReconciliationRowResponseBuilder toBuilder() =>
      FinanceReconciliationRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is FinanceReconciliationRowResponse &&
        issue == other.issue &&
        kind == other.kind &&
        resourceNumber == other.resourceNumber &&
        messageId == other.messageId &&
        erpAmount == other.erpAmount &&
        externalAmount == other.externalAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, issue.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, resourceNumber.hashCode);
    _$hash = $jc(_$hash, messageId.hashCode);
    _$hash = $jc(_$hash, erpAmount.hashCode);
    _$hash = $jc(_$hash, externalAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'FinanceReconciliationRowResponse')
          ..add('issue', issue)
          ..add('kind', kind)
          ..add('resourceNumber', resourceNumber)
          ..add('messageId', messageId)
          ..add('erpAmount', erpAmount)
          ..add('externalAmount', externalAmount))
        .toString();
  }
}

class FinanceReconciliationRowResponseBuilder
    implements
        Builder<FinanceReconciliationRowResponse,
            FinanceReconciliationRowResponseBuilder> {
  _$FinanceReconciliationRowResponse? _$v;

  String? _issue;
  String? get issue => _$this._issue;
  set issue(String? issue) => _$this._issue = issue;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _resourceNumber;
  String? get resourceNumber => _$this._resourceNumber;
  set resourceNumber(String? resourceNumber) =>
      _$this._resourceNumber = resourceNumber;

  String? _messageId;
  String? get messageId => _$this._messageId;
  set messageId(String? messageId) => _$this._messageId = messageId;

  double? _erpAmount;
  double? get erpAmount => _$this._erpAmount;
  set erpAmount(double? erpAmount) => _$this._erpAmount = erpAmount;

  double? _externalAmount;
  double? get externalAmount => _$this._externalAmount;
  set externalAmount(double? externalAmount) =>
      _$this._externalAmount = externalAmount;

  FinanceReconciliationRowResponseBuilder() {
    FinanceReconciliationRowResponse._defaults(this);
  }

  FinanceReconciliationRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _issue = $v.issue;
      _kind = $v.kind;
      _resourceNumber = $v.resourceNumber;
      _messageId = $v.messageId;
      _erpAmount = $v.erpAmount;
      _externalAmount = $v.externalAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(FinanceReconciliationRowResponse other) {
    _$v = other as _$FinanceReconciliationRowResponse;
  }

  @override
  void update(void Function(FinanceReconciliationRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  FinanceReconciliationRowResponse build() => _build();

  _$FinanceReconciliationRowResponse _build() {
    final _$result = _$v ??
        _$FinanceReconciliationRowResponse._(
          issue: issue,
          kind: kind,
          resourceNumber: resourceNumber,
          messageId: messageId,
          erpAmount: erpAmount,
          externalAmount: externalAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
