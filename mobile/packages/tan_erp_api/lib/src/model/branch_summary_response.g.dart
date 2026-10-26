// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchSummaryResponse extends BranchSummaryResponse {
  @override
  final String? id;
  @override
  final String? name;

  factory _$BranchSummaryResponse(
          [void Function(BranchSummaryResponseBuilder)? updates]) =>
      (BranchSummaryResponseBuilder()..update(updates))._build();

  _$BranchSummaryResponse._({this.id, this.name}) : super._();
  @override
  BranchSummaryResponse rebuild(
          void Function(BranchSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchSummaryResponseBuilder toBuilder() =>
      BranchSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchSummaryResponse &&
        id == other.id &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchSummaryResponse')
          ..add('id', id)
          ..add('name', name))
        .toString();
  }
}

class BranchSummaryResponseBuilder
    implements Builder<BranchSummaryResponse, BranchSummaryResponseBuilder> {
  _$BranchSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  BranchSummaryResponseBuilder() {
    BranchSummaryResponse._defaults(this);
  }

  BranchSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchSummaryResponse other) {
    _$v = other as _$BranchSummaryResponse;
  }

  @override
  void update(void Function(BranchSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchSummaryResponse build() => _build();

  _$BranchSummaryResponse _build() {
    final _$result = _$v ??
        _$BranchSummaryResponse._(
          id: id,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
