// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_blocker_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchBlockerResponse extends BranchBlockerResponse {
  @override
  final String? type;
  @override
  final int? count;

  factory _$BranchBlockerResponse(
          [void Function(BranchBlockerResponseBuilder)? updates]) =>
      (BranchBlockerResponseBuilder()..update(updates))._build();

  _$BranchBlockerResponse._({this.type, this.count}) : super._();
  @override
  BranchBlockerResponse rebuild(
          void Function(BranchBlockerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchBlockerResponseBuilder toBuilder() =>
      BranchBlockerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchBlockerResponse &&
        type == other.type &&
        count == other.count;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, type.hashCode);
    _$hash = $jc(_$hash, count.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchBlockerResponse')
          ..add('type', type)
          ..add('count', count))
        .toString();
  }
}

class BranchBlockerResponseBuilder
    implements Builder<BranchBlockerResponse, BranchBlockerResponseBuilder> {
  _$BranchBlockerResponse? _$v;

  String? _type;
  String? get type => _$this._type;
  set type(String? type) => _$this._type = type;

  int? _count;
  int? get count => _$this._count;
  set count(int? count) => _$this._count = count;

  BranchBlockerResponseBuilder() {
    BranchBlockerResponse._defaults(this);
  }

  BranchBlockerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _type = $v.type;
      _count = $v.count;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchBlockerResponse other) {
    _$v = other as _$BranchBlockerResponse;
  }

  @override
  void update(void Function(BranchBlockerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchBlockerResponse build() => _build();

  _$BranchBlockerResponse _build() {
    final _$result = _$v ??
        _$BranchBlockerResponse._(
          type: type,
          count: count,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
