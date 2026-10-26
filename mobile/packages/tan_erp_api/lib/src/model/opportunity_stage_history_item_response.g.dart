// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'opportunity_stage_history_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OpportunityStageHistoryItemResponse
    extends OpportunityStageHistoryItemResponse {
  @override
  final String? id;
  @override
  final String? opportunityId;
  @override
  final String? fromStage;
  @override
  final String? toStage;
  @override
  final String? reasonCode;
  @override
  final String? note;
  @override
  final String? actorUserId;
  @override
  final DateTime? occurredAtUtc;
  @override
  final String? policyVersion;
  @override
  final ActorSummaryResponse? actor;

  factory _$OpportunityStageHistoryItemResponse(
          [void Function(OpportunityStageHistoryItemResponseBuilder)?
              updates]) =>
      (OpportunityStageHistoryItemResponseBuilder()..update(updates))._build();

  _$OpportunityStageHistoryItemResponse._(
      {this.id,
      this.opportunityId,
      this.fromStage,
      this.toStage,
      this.reasonCode,
      this.note,
      this.actorUserId,
      this.occurredAtUtc,
      this.policyVersion,
      this.actor})
      : super._();
  @override
  OpportunityStageHistoryItemResponse rebuild(
          void Function(OpportunityStageHistoryItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OpportunityStageHistoryItemResponseBuilder toBuilder() =>
      OpportunityStageHistoryItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OpportunityStageHistoryItemResponse &&
        id == other.id &&
        opportunityId == other.opportunityId &&
        fromStage == other.fromStage &&
        toStage == other.toStage &&
        reasonCode == other.reasonCode &&
        note == other.note &&
        actorUserId == other.actorUserId &&
        occurredAtUtc == other.occurredAtUtc &&
        policyVersion == other.policyVersion &&
        actor == other.actor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, fromStage.hashCode);
    _$hash = $jc(_$hash, toStage.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, actorUserId.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jc(_$hash, policyVersion.hashCode);
    _$hash = $jc(_$hash, actor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OpportunityStageHistoryItemResponse')
          ..add('id', id)
          ..add('opportunityId', opportunityId)
          ..add('fromStage', fromStage)
          ..add('toStage', toStage)
          ..add('reasonCode', reasonCode)
          ..add('note', note)
          ..add('actorUserId', actorUserId)
          ..add('occurredAtUtc', occurredAtUtc)
          ..add('policyVersion', policyVersion)
          ..add('actor', actor))
        .toString();
  }
}

class OpportunityStageHistoryItemResponseBuilder
    implements
        Builder<OpportunityStageHistoryItemResponse,
            OpportunityStageHistoryItemResponseBuilder> {
  _$OpportunityStageHistoryItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  String? _fromStage;
  String? get fromStage => _$this._fromStage;
  set fromStage(String? fromStage) => _$this._fromStage = fromStage;

  String? _toStage;
  String? get toStage => _$this._toStage;
  set toStage(String? toStage) => _$this._toStage = toStage;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  String? _actorUserId;
  String? get actorUserId => _$this._actorUserId;
  set actorUserId(String? actorUserId) => _$this._actorUserId = actorUserId;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  String? _policyVersion;
  String? get policyVersion => _$this._policyVersion;
  set policyVersion(String? policyVersion) =>
      _$this._policyVersion = policyVersion;

  ActorSummaryResponseBuilder? _actor;
  ActorSummaryResponseBuilder get actor =>
      _$this._actor ??= ActorSummaryResponseBuilder();
  set actor(ActorSummaryResponseBuilder? actor) => _$this._actor = actor;

  OpportunityStageHistoryItemResponseBuilder() {
    OpportunityStageHistoryItemResponse._defaults(this);
  }

  OpportunityStageHistoryItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _opportunityId = $v.opportunityId;
      _fromStage = $v.fromStage;
      _toStage = $v.toStage;
      _reasonCode = $v.reasonCode;
      _note = $v.note;
      _actorUserId = $v.actorUserId;
      _occurredAtUtc = $v.occurredAtUtc;
      _policyVersion = $v.policyVersion;
      _actor = $v.actor?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OpportunityStageHistoryItemResponse other) {
    _$v = other as _$OpportunityStageHistoryItemResponse;
  }

  @override
  void update(
      void Function(OpportunityStageHistoryItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OpportunityStageHistoryItemResponse build() => _build();

  _$OpportunityStageHistoryItemResponse _build() {
    _$OpportunityStageHistoryItemResponse _$result;
    try {
      _$result = _$v ??
          _$OpportunityStageHistoryItemResponse._(
            id: id,
            opportunityId: opportunityId,
            fromStage: fromStage,
            toStage: toStage,
            reasonCode: reasonCode,
            note: note,
            actorUserId: actorUserId,
            occurredAtUtc: occurredAtUtc,
            policyVersion: policyVersion,
            actor: _actor?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'actor';
        _actor?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(r'OpportunityStageHistoryItemResponse',
            _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
