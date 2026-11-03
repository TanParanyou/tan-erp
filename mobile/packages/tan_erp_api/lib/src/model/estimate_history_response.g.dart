// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_history_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateHistoryResponse extends EstimateHistoryResponse {
  @override
  final BuiltList<EstimateRevisionSummaryResponse> revisions;
  @override
  final BuiltList<EstimateHistoryEventResponse> events;

  factory _$EstimateHistoryResponse(
          [void Function(EstimateHistoryResponseBuilder)? updates]) =>
      (EstimateHistoryResponseBuilder()..update(updates))._build();

  _$EstimateHistoryResponse._({required this.revisions, required this.events})
      : super._();
  @override
  EstimateHistoryResponse rebuild(
          void Function(EstimateHistoryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateHistoryResponseBuilder toBuilder() =>
      EstimateHistoryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateHistoryResponse &&
        revisions == other.revisions &&
        events == other.events;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, revisions.hashCode);
    _$hash = $jc(_$hash, events.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateHistoryResponse')
          ..add('revisions', revisions)
          ..add('events', events))
        .toString();
  }
}

class EstimateHistoryResponseBuilder
    implements
        Builder<EstimateHistoryResponse, EstimateHistoryResponseBuilder> {
  _$EstimateHistoryResponse? _$v;

  ListBuilder<EstimateRevisionSummaryResponse>? _revisions;
  ListBuilder<EstimateRevisionSummaryResponse> get revisions =>
      _$this._revisions ??= ListBuilder<EstimateRevisionSummaryResponse>();
  set revisions(ListBuilder<EstimateRevisionSummaryResponse>? revisions) =>
      _$this._revisions = revisions;

  ListBuilder<EstimateHistoryEventResponse>? _events;
  ListBuilder<EstimateHistoryEventResponse> get events =>
      _$this._events ??= ListBuilder<EstimateHistoryEventResponse>();
  set events(ListBuilder<EstimateHistoryEventResponse>? events) =>
      _$this._events = events;

  EstimateHistoryResponseBuilder() {
    EstimateHistoryResponse._defaults(this);
  }

  EstimateHistoryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _revisions = $v.revisions.toBuilder();
      _events = $v.events.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateHistoryResponse other) {
    _$v = other as _$EstimateHistoryResponse;
  }

  @override
  void update(void Function(EstimateHistoryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateHistoryResponse build() => _build();

  _$EstimateHistoryResponse _build() {
    _$EstimateHistoryResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateHistoryResponse._(
            revisions: revisions.build(),
            events: events.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'revisions';
        revisions.build();
        _$failedField = 'events';
        events.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateHistoryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
