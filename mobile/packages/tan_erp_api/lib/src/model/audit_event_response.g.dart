// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'audit_event_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AuditEventResponse extends AuditEventResponse {
  @override
  final String? id;
  @override
  final DateTime? occurredAtUtc;
  @override
  final String? action;
  @override
  final String? resourceType;
  @override
  final String? resourceId;
  @override
  final AdminRefResponse? actor;
  @override
  final String? branchId;
  @override
  final String? traceId;
  @override
  final String? requestId;
  @override
  final String? reason;
  @override
  final String? changesJson;

  factory _$AuditEventResponse(
          [void Function(AuditEventResponseBuilder)? updates]) =>
      (AuditEventResponseBuilder()..update(updates))._build();

  _$AuditEventResponse._(
      {this.id,
      this.occurredAtUtc,
      this.action,
      this.resourceType,
      this.resourceId,
      this.actor,
      this.branchId,
      this.traceId,
      this.requestId,
      this.reason,
      this.changesJson})
      : super._();
  @override
  AuditEventResponse rebuild(
          void Function(AuditEventResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AuditEventResponseBuilder toBuilder() =>
      AuditEventResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AuditEventResponse &&
        id == other.id &&
        occurredAtUtc == other.occurredAtUtc &&
        action == other.action &&
        resourceType == other.resourceType &&
        resourceId == other.resourceId &&
        actor == other.actor &&
        branchId == other.branchId &&
        traceId == other.traceId &&
        requestId == other.requestId &&
        reason == other.reason &&
        changesJson == other.changesJson;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jc(_$hash, action.hashCode);
    _$hash = $jc(_$hash, resourceType.hashCode);
    _$hash = $jc(_$hash, resourceId.hashCode);
    _$hash = $jc(_$hash, actor.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, traceId.hashCode);
    _$hash = $jc(_$hash, requestId.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, changesJson.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AuditEventResponse')
          ..add('id', id)
          ..add('occurredAtUtc', occurredAtUtc)
          ..add('action', action)
          ..add('resourceType', resourceType)
          ..add('resourceId', resourceId)
          ..add('actor', actor)
          ..add('branchId', branchId)
          ..add('traceId', traceId)
          ..add('requestId', requestId)
          ..add('reason', reason)
          ..add('changesJson', changesJson))
        .toString();
  }
}

class AuditEventResponseBuilder
    implements Builder<AuditEventResponse, AuditEventResponseBuilder> {
  _$AuditEventResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  String? _action;
  String? get action => _$this._action;
  set action(String? action) => _$this._action = action;

  String? _resourceType;
  String? get resourceType => _$this._resourceType;
  set resourceType(String? resourceType) => _$this._resourceType = resourceType;

  String? _resourceId;
  String? get resourceId => _$this._resourceId;
  set resourceId(String? resourceId) => _$this._resourceId = resourceId;

  AdminRefResponseBuilder? _actor;
  AdminRefResponseBuilder get actor =>
      _$this._actor ??= AdminRefResponseBuilder();
  set actor(AdminRefResponseBuilder? actor) => _$this._actor = actor;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _traceId;
  String? get traceId => _$this._traceId;
  set traceId(String? traceId) => _$this._traceId = traceId;

  String? _requestId;
  String? get requestId => _$this._requestId;
  set requestId(String? requestId) => _$this._requestId = requestId;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  String? _changesJson;
  String? get changesJson => _$this._changesJson;
  set changesJson(String? changesJson) => _$this._changesJson = changesJson;

  AuditEventResponseBuilder() {
    AuditEventResponse._defaults(this);
  }

  AuditEventResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _occurredAtUtc = $v.occurredAtUtc;
      _action = $v.action;
      _resourceType = $v.resourceType;
      _resourceId = $v.resourceId;
      _actor = $v.actor?.toBuilder();
      _branchId = $v.branchId;
      _traceId = $v.traceId;
      _requestId = $v.requestId;
      _reason = $v.reason;
      _changesJson = $v.changesJson;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AuditEventResponse other) {
    _$v = other as _$AuditEventResponse;
  }

  @override
  void update(void Function(AuditEventResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AuditEventResponse build() => _build();

  _$AuditEventResponse _build() {
    _$AuditEventResponse _$result;
    try {
      _$result = _$v ??
          _$AuditEventResponse._(
            id: id,
            occurredAtUtc: occurredAtUtc,
            action: action,
            resourceType: resourceType,
            resourceId: resourceId,
            actor: _actor?.build(),
            branchId: branchId,
            traceId: traceId,
            requestId: requestId,
            reason: reason,
            changesJson: changesJson,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'actor';
        _actor?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AuditEventResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
