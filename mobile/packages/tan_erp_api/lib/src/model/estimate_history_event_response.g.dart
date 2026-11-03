// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_history_event_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateHistoryEventResponse extends EstimateHistoryEventResponse {
  @override
  final DateTime occurredAtUtc;
  @override
  final String action;
  @override
  final EstimateActorResponse actor;

  factory _$EstimateHistoryEventResponse(
          [void Function(EstimateHistoryEventResponseBuilder)? updates]) =>
      (EstimateHistoryEventResponseBuilder()..update(updates))._build();

  _$EstimateHistoryEventResponse._(
      {required this.occurredAtUtc, required this.action, required this.actor})
      : super._();
  @override
  EstimateHistoryEventResponse rebuild(
          void Function(EstimateHistoryEventResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateHistoryEventResponseBuilder toBuilder() =>
      EstimateHistoryEventResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateHistoryEventResponse &&
        occurredAtUtc == other.occurredAtUtc &&
        action == other.action &&
        actor == other.actor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jc(_$hash, action.hashCode);
    _$hash = $jc(_$hash, actor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateHistoryEventResponse')
          ..add('occurredAtUtc', occurredAtUtc)
          ..add('action', action)
          ..add('actor', actor))
        .toString();
  }
}

class EstimateHistoryEventResponseBuilder
    implements
        Builder<EstimateHistoryEventResponse,
            EstimateHistoryEventResponseBuilder> {
  _$EstimateHistoryEventResponse? _$v;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  String? _action;
  String? get action => _$this._action;
  set action(String? action) => _$this._action = action;

  EstimateActorResponseBuilder? _actor;
  EstimateActorResponseBuilder get actor =>
      _$this._actor ??= EstimateActorResponseBuilder();
  set actor(EstimateActorResponseBuilder? actor) => _$this._actor = actor;

  EstimateHistoryEventResponseBuilder() {
    EstimateHistoryEventResponse._defaults(this);
  }

  EstimateHistoryEventResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _occurredAtUtc = $v.occurredAtUtc;
      _action = $v.action;
      _actor = $v.actor.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateHistoryEventResponse other) {
    _$v = other as _$EstimateHistoryEventResponse;
  }

  @override
  void update(void Function(EstimateHistoryEventResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateHistoryEventResponse build() => _build();

  _$EstimateHistoryEventResponse _build() {
    _$EstimateHistoryEventResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateHistoryEventResponse._(
            occurredAtUtc: BuiltValueNullFieldError.checkNotNull(occurredAtUtc,
                r'EstimateHistoryEventResponse', 'occurredAtUtc'),
            action: BuiltValueNullFieldError.checkNotNull(
                action, r'EstimateHistoryEventResponse', 'action'),
            actor: actor.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'actor';
        actor.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateHistoryEventResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
