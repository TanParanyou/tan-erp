// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_queue_branch_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewQueueBranchResponse
    extends EstimateReviewQueueBranchResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$EstimateReviewQueueBranchResponse(
          [void Function(EstimateReviewQueueBranchResponseBuilder)? updates]) =>
      (EstimateReviewQueueBranchResponseBuilder()..update(updates))._build();

  _$EstimateReviewQueueBranchResponse._({this.id, this.code, this.name})
      : super._();
  @override
  EstimateReviewQueueBranchResponse rebuild(
          void Function(EstimateReviewQueueBranchResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewQueueBranchResponseBuilder toBuilder() =>
      EstimateReviewQueueBranchResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewQueueBranchResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewQueueBranchResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class EstimateReviewQueueBranchResponseBuilder
    implements
        Builder<EstimateReviewQueueBranchResponse,
            EstimateReviewQueueBranchResponseBuilder> {
  _$EstimateReviewQueueBranchResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  EstimateReviewQueueBranchResponseBuilder() {
    EstimateReviewQueueBranchResponse._defaults(this);
  }

  EstimateReviewQueueBranchResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewQueueBranchResponse other) {
    _$v = other as _$EstimateReviewQueueBranchResponse;
  }

  @override
  void update(
      void Function(EstimateReviewQueueBranchResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewQueueBranchResponse build() => _build();

  _$EstimateReviewQueueBranchResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewQueueBranchResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
