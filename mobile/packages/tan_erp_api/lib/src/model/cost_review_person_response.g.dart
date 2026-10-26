// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cost_review_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CostReviewPersonResponse extends CostReviewPersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$CostReviewPersonResponse(
          [void Function(CostReviewPersonResponseBuilder)? updates]) =>
      (CostReviewPersonResponseBuilder()..update(updates))._build();

  _$CostReviewPersonResponse._({this.id, this.displayName}) : super._();
  @override
  CostReviewPersonResponse rebuild(
          void Function(CostReviewPersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CostReviewPersonResponseBuilder toBuilder() =>
      CostReviewPersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CostReviewPersonResponse &&
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
    return (newBuiltValueToStringHelper(r'CostReviewPersonResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class CostReviewPersonResponseBuilder
    implements
        Builder<CostReviewPersonResponse, CostReviewPersonResponseBuilder> {
  _$CostReviewPersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  CostReviewPersonResponseBuilder() {
    CostReviewPersonResponse._defaults(this);
  }

  CostReviewPersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CostReviewPersonResponse other) {
    _$v = other as _$CostReviewPersonResponse;
  }

  @override
  void update(void Function(CostReviewPersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CostReviewPersonResponse build() => _build();

  _$CostReviewPersonResponse _build() {
    final _$result = _$v ??
        _$CostReviewPersonResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
