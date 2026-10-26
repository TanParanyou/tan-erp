// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_queue_customer_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewQueueCustomerResponse
    extends EstimateReviewQueueCustomerResponse {
  @override
  final String? id;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;

  factory _$EstimateReviewQueueCustomerResponse(
          [void Function(EstimateReviewQueueCustomerResponseBuilder)?
              updates]) =>
      (EstimateReviewQueueCustomerResponseBuilder()..update(updates))._build();

  _$EstimateReviewQueueCustomerResponse._(
      {this.id, this.displayNameTh, this.displayNameEn})
      : super._();
  @override
  EstimateReviewQueueCustomerResponse rebuild(
          void Function(EstimateReviewQueueCustomerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewQueueCustomerResponseBuilder toBuilder() =>
      EstimateReviewQueueCustomerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewQueueCustomerResponse &&
        id == other.id &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewQueueCustomerResponse')
          ..add('id', id)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn))
        .toString();
  }
}

class EstimateReviewQueueCustomerResponseBuilder
    implements
        Builder<EstimateReviewQueueCustomerResponse,
            EstimateReviewQueueCustomerResponseBuilder> {
  _$EstimateReviewQueueCustomerResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayNameTh;
  String? get displayNameTh => _$this._displayNameTh;
  set displayNameTh(String? displayNameTh) =>
      _$this._displayNameTh = displayNameTh;

  String? _displayNameEn;
  String? get displayNameEn => _$this._displayNameEn;
  set displayNameEn(String? displayNameEn) =>
      _$this._displayNameEn = displayNameEn;

  EstimateReviewQueueCustomerResponseBuilder() {
    EstimateReviewQueueCustomerResponse._defaults(this);
  }

  EstimateReviewQueueCustomerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewQueueCustomerResponse other) {
    _$v = other as _$EstimateReviewQueueCustomerResponse;
  }

  @override
  void update(
      void Function(EstimateReviewQueueCustomerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewQueueCustomerResponse build() => _build();

  _$EstimateReviewQueueCustomerResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewQueueCustomerResponse._(
          id: id,
          displayNameTh: displayNameTh,
          displayNameEn: displayNameEn,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
