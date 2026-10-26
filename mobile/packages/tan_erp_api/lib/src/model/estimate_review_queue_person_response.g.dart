// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_queue_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewQueuePersonResponse
    extends EstimateReviewQueuePersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$EstimateReviewQueuePersonResponse(
          [void Function(EstimateReviewQueuePersonResponseBuilder)? updates]) =>
      (EstimateReviewQueuePersonResponseBuilder()..update(updates))._build();

  _$EstimateReviewQueuePersonResponse._({this.id, this.displayName})
      : super._();
  @override
  EstimateReviewQueuePersonResponse rebuild(
          void Function(EstimateReviewQueuePersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewQueuePersonResponseBuilder toBuilder() =>
      EstimateReviewQueuePersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewQueuePersonResponse &&
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
    return (newBuiltValueToStringHelper(r'EstimateReviewQueuePersonResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class EstimateReviewQueuePersonResponseBuilder
    implements
        Builder<EstimateReviewQueuePersonResponse,
            EstimateReviewQueuePersonResponseBuilder> {
  _$EstimateReviewQueuePersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  EstimateReviewQueuePersonResponseBuilder() {
    EstimateReviewQueuePersonResponse._defaults(this);
  }

  EstimateReviewQueuePersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewQueuePersonResponse other) {
    _$v = other as _$EstimateReviewQueuePersonResponse;
  }

  @override
  void update(
      void Function(EstimateReviewQueuePersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewQueuePersonResponse build() => _build();

  _$EstimateReviewQueuePersonResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewQueuePersonResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
