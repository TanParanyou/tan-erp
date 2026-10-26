// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_request_event_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceRequestEventResponse extends ServiceRequestEventResponse {
  @override
  final String? fromStatus;
  @override
  final String? toStatus;
  @override
  final String? note;
  @override
  final ServicePersonResponse? actor;
  @override
  final DateTime? occurredAtUtc;

  factory _$ServiceRequestEventResponse(
          [void Function(ServiceRequestEventResponseBuilder)? updates]) =>
      (ServiceRequestEventResponseBuilder()..update(updates))._build();

  _$ServiceRequestEventResponse._(
      {this.fromStatus,
      this.toStatus,
      this.note,
      this.actor,
      this.occurredAtUtc})
      : super._();
  @override
  ServiceRequestEventResponse rebuild(
          void Function(ServiceRequestEventResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceRequestEventResponseBuilder toBuilder() =>
      ServiceRequestEventResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceRequestEventResponse &&
        fromStatus == other.fromStatus &&
        toStatus == other.toStatus &&
        note == other.note &&
        actor == other.actor &&
        occurredAtUtc == other.occurredAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fromStatus.hashCode);
    _$hash = $jc(_$hash, toStatus.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, actor.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ServiceRequestEventResponse')
          ..add('fromStatus', fromStatus)
          ..add('toStatus', toStatus)
          ..add('note', note)
          ..add('actor', actor)
          ..add('occurredAtUtc', occurredAtUtc))
        .toString();
  }
}

class ServiceRequestEventResponseBuilder
    implements
        Builder<ServiceRequestEventResponse,
            ServiceRequestEventResponseBuilder> {
  _$ServiceRequestEventResponse? _$v;

  String? _fromStatus;
  String? get fromStatus => _$this._fromStatus;
  set fromStatus(String? fromStatus) => _$this._fromStatus = fromStatus;

  String? _toStatus;
  String? get toStatus => _$this._toStatus;
  set toStatus(String? toStatus) => _$this._toStatus = toStatus;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ServicePersonResponseBuilder? _actor;
  ServicePersonResponseBuilder get actor =>
      _$this._actor ??= ServicePersonResponseBuilder();
  set actor(ServicePersonResponseBuilder? actor) => _$this._actor = actor;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  ServiceRequestEventResponseBuilder() {
    ServiceRequestEventResponse._defaults(this);
  }

  ServiceRequestEventResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fromStatus = $v.fromStatus;
      _toStatus = $v.toStatus;
      _note = $v.note;
      _actor = $v.actor?.toBuilder();
      _occurredAtUtc = $v.occurredAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceRequestEventResponse other) {
    _$v = other as _$ServiceRequestEventResponse;
  }

  @override
  void update(void Function(ServiceRequestEventResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceRequestEventResponse build() => _build();

  _$ServiceRequestEventResponse _build() {
    _$ServiceRequestEventResponse _$result;
    try {
      _$result = _$v ??
          _$ServiceRequestEventResponse._(
            fromStatus: fromStatus,
            toStatus: toStatus,
            note: note,
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
            r'ServiceRequestEventResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
