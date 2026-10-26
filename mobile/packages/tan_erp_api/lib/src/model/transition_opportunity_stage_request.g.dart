// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'transition_opportunity_stage_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TransitionOpportunityStageRequest
    extends TransitionOpportunityStageRequest {
  @override
  final String? targetStage;
  @override
  final String? expectedVersion;
  @override
  final String? reasonCode;
  @override
  final String? note;

  factory _$TransitionOpportunityStageRequest(
          [void Function(TransitionOpportunityStageRequestBuilder)? updates]) =>
      (TransitionOpportunityStageRequestBuilder()..update(updates))._build();

  _$TransitionOpportunityStageRequest._(
      {this.targetStage, this.expectedVersion, this.reasonCode, this.note})
      : super._();
  @override
  TransitionOpportunityStageRequest rebuild(
          void Function(TransitionOpportunityStageRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TransitionOpportunityStageRequestBuilder toBuilder() =>
      TransitionOpportunityStageRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TransitionOpportunityStageRequest &&
        targetStage == other.targetStage &&
        expectedVersion == other.expectedVersion &&
        reasonCode == other.reasonCode &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, targetStage.hashCode);
    _$hash = $jc(_$hash, expectedVersion.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TransitionOpportunityStageRequest')
          ..add('targetStage', targetStage)
          ..add('expectedVersion', expectedVersion)
          ..add('reasonCode', reasonCode)
          ..add('note', note))
        .toString();
  }
}

class TransitionOpportunityStageRequestBuilder
    implements
        Builder<TransitionOpportunityStageRequest,
            TransitionOpportunityStageRequestBuilder> {
  _$TransitionOpportunityStageRequest? _$v;

  String? _targetStage;
  String? get targetStage => _$this._targetStage;
  set targetStage(String? targetStage) => _$this._targetStage = targetStage;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  TransitionOpportunityStageRequestBuilder() {
    TransitionOpportunityStageRequest._defaults(this);
  }

  TransitionOpportunityStageRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _targetStage = $v.targetStage;
      _expectedVersion = $v.expectedVersion;
      _reasonCode = $v.reasonCode;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TransitionOpportunityStageRequest other) {
    _$v = other as _$TransitionOpportunityStageRequest;
  }

  @override
  void update(
      void Function(TransitionOpportunityStageRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TransitionOpportunityStageRequest build() => _build();

  _$TransitionOpportunityStageRequest _build() {
    final _$result = _$v ??
        _$TransitionOpportunityStageRequest._(
          targetStage: targetStage,
          expectedVersion: expectedVersion,
          reasonCode: reasonCode,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
