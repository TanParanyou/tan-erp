// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_return_reviewer_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReturnReviewerResponse extends EstimateReturnReviewerResponse {
  @override
  final String id;
  @override
  final String displayName;

  factory _$EstimateReturnReviewerResponse(
          [void Function(EstimateReturnReviewerResponseBuilder)? updates]) =>
      (EstimateReturnReviewerResponseBuilder()..update(updates))._build();

  _$EstimateReturnReviewerResponse._(
      {required this.id, required this.displayName})
      : super._();
  @override
  EstimateReturnReviewerResponse rebuild(
          void Function(EstimateReturnReviewerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReturnReviewerResponseBuilder toBuilder() =>
      EstimateReturnReviewerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReturnReviewerResponse &&
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
    return (newBuiltValueToStringHelper(r'EstimateReturnReviewerResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class EstimateReturnReviewerResponseBuilder
    implements
        Builder<EstimateReturnReviewerResponse,
            EstimateReturnReviewerResponseBuilder> {
  _$EstimateReturnReviewerResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  EstimateReturnReviewerResponseBuilder() {
    EstimateReturnReviewerResponse._defaults(this);
  }

  EstimateReturnReviewerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReturnReviewerResponse other) {
    _$v = other as _$EstimateReturnReviewerResponse;
  }

  @override
  void update(void Function(EstimateReturnReviewerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReturnReviewerResponse build() => _build();

  _$EstimateReturnReviewerResponse _build() {
    final _$result = _$v ??
        _$EstimateReturnReviewerResponse._(
          id: BuiltValueNullFieldError.checkNotNull(
              id, r'EstimateReturnReviewerResponse', 'id'),
          displayName: BuiltValueNullFieldError.checkNotNull(
              displayName, r'EstimateReturnReviewerResponse', 'displayName'),
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
