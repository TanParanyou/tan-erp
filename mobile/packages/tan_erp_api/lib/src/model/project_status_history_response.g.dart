// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_status_history_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectStatusHistoryResponse extends ProjectStatusHistoryResponse {
  @override
  final String? id;
  @override
  final String? fromStatus;
  @override
  final String? toStatus;
  @override
  final String? reason;
  @override
  final ProjectPersonResponse? actor;
  @override
  final DateTime? occurredAtUtc;

  factory _$ProjectStatusHistoryResponse(
          [void Function(ProjectStatusHistoryResponseBuilder)? updates]) =>
      (ProjectStatusHistoryResponseBuilder()..update(updates))._build();

  _$ProjectStatusHistoryResponse._(
      {this.id,
      this.fromStatus,
      this.toStatus,
      this.reason,
      this.actor,
      this.occurredAtUtc})
      : super._();
  @override
  ProjectStatusHistoryResponse rebuild(
          void Function(ProjectStatusHistoryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectStatusHistoryResponseBuilder toBuilder() =>
      ProjectStatusHistoryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectStatusHistoryResponse &&
        id == other.id &&
        fromStatus == other.fromStatus &&
        toStatus == other.toStatus &&
        reason == other.reason &&
        actor == other.actor &&
        occurredAtUtc == other.occurredAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, fromStatus.hashCode);
    _$hash = $jc(_$hash, toStatus.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, actor.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectStatusHistoryResponse')
          ..add('id', id)
          ..add('fromStatus', fromStatus)
          ..add('toStatus', toStatus)
          ..add('reason', reason)
          ..add('actor', actor)
          ..add('occurredAtUtc', occurredAtUtc))
        .toString();
  }
}

class ProjectStatusHistoryResponseBuilder
    implements
        Builder<ProjectStatusHistoryResponse,
            ProjectStatusHistoryResponseBuilder> {
  _$ProjectStatusHistoryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _fromStatus;
  String? get fromStatus => _$this._fromStatus;
  set fromStatus(String? fromStatus) => _$this._fromStatus = fromStatus;

  String? _toStatus;
  String? get toStatus => _$this._toStatus;
  set toStatus(String? toStatus) => _$this._toStatus = toStatus;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  ProjectPersonResponseBuilder? _actor;
  ProjectPersonResponseBuilder get actor =>
      _$this._actor ??= ProjectPersonResponseBuilder();
  set actor(ProjectPersonResponseBuilder? actor) => _$this._actor = actor;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  ProjectStatusHistoryResponseBuilder() {
    ProjectStatusHistoryResponse._defaults(this);
  }

  ProjectStatusHistoryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _fromStatus = $v.fromStatus;
      _toStatus = $v.toStatus;
      _reason = $v.reason;
      _actor = $v.actor?.toBuilder();
      _occurredAtUtc = $v.occurredAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectStatusHistoryResponse other) {
    _$v = other as _$ProjectStatusHistoryResponse;
  }

  @override
  void update(void Function(ProjectStatusHistoryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectStatusHistoryResponse build() => _build();

  _$ProjectStatusHistoryResponse _build() {
    _$ProjectStatusHistoryResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectStatusHistoryResponse._(
            id: id,
            fromStatus: fromStatus,
            toStatus: toStatus,
            reason: reason,
            actor: _actor?.build(),
            occurredAtUtc: occurredAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'actor';
        _actor?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectStatusHistoryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
