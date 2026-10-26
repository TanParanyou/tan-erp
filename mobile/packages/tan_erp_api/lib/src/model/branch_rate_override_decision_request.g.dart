// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_rate_override_decision_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchRateOverrideDecisionRequest
    extends BranchRateOverrideDecisionRequest {
  @override
  final String? decision;
  @override
  final String? note;

  factory _$BranchRateOverrideDecisionRequest(
          [void Function(BranchRateOverrideDecisionRequestBuilder)? updates]) =>
      (BranchRateOverrideDecisionRequestBuilder()..update(updates))._build();

  _$BranchRateOverrideDecisionRequest._({this.decision, this.note}) : super._();
  @override
  BranchRateOverrideDecisionRequest rebuild(
          void Function(BranchRateOverrideDecisionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchRateOverrideDecisionRequestBuilder toBuilder() =>
      BranchRateOverrideDecisionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchRateOverrideDecisionRequest &&
        decision == other.decision &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, decision.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchRateOverrideDecisionRequest')
          ..add('decision', decision)
          ..add('note', note))
        .toString();
  }
}

class BranchRateOverrideDecisionRequestBuilder
    implements
        Builder<BranchRateOverrideDecisionRequest,
            BranchRateOverrideDecisionRequestBuilder> {
  _$BranchRateOverrideDecisionRequest? _$v;

  String? _decision;
  String? get decision => _$this._decision;
  set decision(String? decision) => _$this._decision = decision;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  BranchRateOverrideDecisionRequestBuilder() {
    BranchRateOverrideDecisionRequest._defaults(this);
  }

  BranchRateOverrideDecisionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _decision = $v.decision;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchRateOverrideDecisionRequest other) {
    _$v = other as _$BranchRateOverrideDecisionRequest;
  }

  @override
  void update(
      void Function(BranchRateOverrideDecisionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchRateOverrideDecisionRequest build() => _build();

  _$BranchRateOverrideDecisionRequest _build() {
    final _$result = _$v ??
        _$BranchRateOverrideDecisionRequest._(
          decision: decision,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
