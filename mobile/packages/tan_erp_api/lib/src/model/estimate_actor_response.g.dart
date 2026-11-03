// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_actor_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateActorResponse extends EstimateActorResponse {
  @override
  final String id;
  @override
  final String displayName;

  factory _$EstimateActorResponse(
          [void Function(EstimateActorResponseBuilder)? updates]) =>
      (EstimateActorResponseBuilder()..update(updates))._build();

  _$EstimateActorResponse._({required this.id, required this.displayName})
      : super._();
  @override
  EstimateActorResponse rebuild(
          void Function(EstimateActorResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateActorResponseBuilder toBuilder() =>
      EstimateActorResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateActorResponse &&
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
    return (newBuiltValueToStringHelper(r'EstimateActorResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class EstimateActorResponseBuilder
    implements Builder<EstimateActorResponse, EstimateActorResponseBuilder> {
  _$EstimateActorResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  EstimateActorResponseBuilder() {
    EstimateActorResponse._defaults(this);
  }

  EstimateActorResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateActorResponse other) {
    _$v = other as _$EstimateActorResponse;
  }

  @override
  void update(void Function(EstimateActorResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateActorResponse build() => _build();

  _$EstimateActorResponse _build() {
    final _$result = _$v ??
        _$EstimateActorResponse._(
          id: BuiltValueNullFieldError.checkNotNull(
              id, r'EstimateActorResponse', 'id'),
          displayName: BuiltValueNullFieldError.checkNotNull(
              displayName, r'EstimateActorResponse', 'displayName'),
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
