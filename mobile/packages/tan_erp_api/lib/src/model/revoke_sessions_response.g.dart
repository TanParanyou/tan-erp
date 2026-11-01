// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'revoke_sessions_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RevokeSessionsResponse extends RevokeSessionsResponse {
  @override
  final int? revokedCount;

  factory _$RevokeSessionsResponse(
          [void Function(RevokeSessionsResponseBuilder)? updates]) =>
      (RevokeSessionsResponseBuilder()..update(updates))._build();

  _$RevokeSessionsResponse._({this.revokedCount}) : super._();
  @override
  RevokeSessionsResponse rebuild(
          void Function(RevokeSessionsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RevokeSessionsResponseBuilder toBuilder() =>
      RevokeSessionsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RevokeSessionsResponse &&
        revokedCount == other.revokedCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, revokedCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RevokeSessionsResponse')
          ..add('revokedCount', revokedCount))
        .toString();
  }
}

class RevokeSessionsResponseBuilder
    implements Builder<RevokeSessionsResponse, RevokeSessionsResponseBuilder> {
  _$RevokeSessionsResponse? _$v;

  int? _revokedCount;
  int? get revokedCount => _$this._revokedCount;
  set revokedCount(int? revokedCount) => _$this._revokedCount = revokedCount;

  RevokeSessionsResponseBuilder() {
    RevokeSessionsResponse._defaults(this);
  }

  RevokeSessionsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _revokedCount = $v.revokedCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RevokeSessionsResponse other) {
    _$v = other as _$RevokeSessionsResponse;
  }

  @override
  void update(void Function(RevokeSessionsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RevokeSessionsResponse build() => _build();

  _$RevokeSessionsResponse _build() {
    final _$result = _$v ??
        _$RevokeSessionsResponse._(
          revokedCount: revokedCount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
