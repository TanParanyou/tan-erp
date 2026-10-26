// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'actor_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ActorSummaryResponse extends ActorSummaryResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$ActorSummaryResponse(
          [void Function(ActorSummaryResponseBuilder)? updates]) =>
      (ActorSummaryResponseBuilder()..update(updates))._build();

  _$ActorSummaryResponse._({this.id, this.displayName}) : super._();
  @override
  ActorSummaryResponse rebuild(
          void Function(ActorSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ActorSummaryResponseBuilder toBuilder() =>
      ActorSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ActorSummaryResponse &&
        id == other.id &&
        displayName == other.displayName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ActorSummaryResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class ActorSummaryResponseBuilder
    implements Builder<ActorSummaryResponse, ActorSummaryResponseBuilder> {
  _$ActorSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  ActorSummaryResponseBuilder() {
    ActorSummaryResponse._defaults(this);
  }

  ActorSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ActorSummaryResponse other) {
    _$v = other as _$ActorSummaryResponse;
  }

  @override
  void update(void Function(ActorSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ActorSummaryResponse build() => _build();

  _$ActorSummaryResponse _build() {
    final _$result = _$v ??
        _$ActorSummaryResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
