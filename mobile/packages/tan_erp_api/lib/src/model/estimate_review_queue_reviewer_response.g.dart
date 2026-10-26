// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_queue_reviewer_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewQueueReviewerResponse
    extends EstimateReviewQueueReviewerResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? membershipId;

  factory _$EstimateReviewQueueReviewerResponse(
          [void Function(EstimateReviewQueueReviewerResponseBuilder)?
              updates]) =>
      (EstimateReviewQueueReviewerResponseBuilder()..update(updates))._build();

  _$EstimateReviewQueueReviewerResponse._(
      {this.id, this.displayName, this.membershipId})
      : super._();
  @override
  EstimateReviewQueueReviewerResponse rebuild(
          void Function(EstimateReviewQueueReviewerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewQueueReviewerResponseBuilder toBuilder() =>
      EstimateReviewQueueReviewerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewQueueReviewerResponse &&
        id == other.id &&
        displayName == other.displayName &&
        membershipId == other.membershipId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, membershipId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewQueueReviewerResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('membershipId', membershipId))
        .toString();
  }
}

class EstimateReviewQueueReviewerResponseBuilder
    implements
        Builder<EstimateReviewQueueReviewerResponse,
            EstimateReviewQueueReviewerResponseBuilder> {
  _$EstimateReviewQueueReviewerResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _membershipId;
  String? get membershipId => _$this._membershipId;
  set membershipId(String? membershipId) => _$this._membershipId = membershipId;

  EstimateReviewQueueReviewerResponseBuilder() {
    EstimateReviewQueueReviewerResponse._defaults(this);
  }

  EstimateReviewQueueReviewerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _membershipId = $v.membershipId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewQueueReviewerResponse other) {
    _$v = other as _$EstimateReviewQueueReviewerResponse;
  }

  @override
  void update(
      void Function(EstimateReviewQueueReviewerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewQueueReviewerResponse build() => _build();

  _$EstimateReviewQueueReviewerResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewQueueReviewerResponse._(
          id: id,
          displayName: displayName,
          membershipId: membershipId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
