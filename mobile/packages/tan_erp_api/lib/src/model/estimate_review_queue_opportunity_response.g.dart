// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_queue_opportunity_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewQueueOpportunityResponse
    extends EstimateReviewQueueOpportunityResponse {
  @override
  final String? id;
  @override
  final String? title;

  factory _$EstimateReviewQueueOpportunityResponse(
          [void Function(EstimateReviewQueueOpportunityResponseBuilder)?
              updates]) =>
      (EstimateReviewQueueOpportunityResponseBuilder()..update(updates))
          ._build();

  _$EstimateReviewQueueOpportunityResponse._({this.id, this.title}) : super._();
  @override
  EstimateReviewQueueOpportunityResponse rebuild(
          void Function(EstimateReviewQueueOpportunityResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewQueueOpportunityResponseBuilder toBuilder() =>
      EstimateReviewQueueOpportunityResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewQueueOpportunityResponse &&
        id == other.id &&
        title == other.title;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(
            r'EstimateReviewQueueOpportunityResponse')
          ..add('id', id)
          ..add('title', title))
        .toString();
  }
}

class EstimateReviewQueueOpportunityResponseBuilder
    implements
        Builder<EstimateReviewQueueOpportunityResponse,
            EstimateReviewQueueOpportunityResponseBuilder> {
  _$EstimateReviewQueueOpportunityResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  EstimateReviewQueueOpportunityResponseBuilder() {
    EstimateReviewQueueOpportunityResponse._defaults(this);
  }

  EstimateReviewQueueOpportunityResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _title = $v.title;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewQueueOpportunityResponse other) {
    _$v = other as _$EstimateReviewQueueOpportunityResponse;
  }

  @override
  void update(
      void Function(EstimateReviewQueueOpportunityResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewQueueOpportunityResponse build() => _build();

  _$EstimateReviewQueueOpportunityResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewQueueOpportunityResponse._(
          id: id,
          title: title,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
