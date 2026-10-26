// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'created_acceptance_link_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreatedAcceptanceLinkResponse extends CreatedAcceptanceLinkResponse {
  @override
  final AcceptanceLinkResponse? link;
  @override
  final String? token;
  @override
  final String? publicPath;

  factory _$CreatedAcceptanceLinkResponse(
          [void Function(CreatedAcceptanceLinkResponseBuilder)? updates]) =>
      (CreatedAcceptanceLinkResponseBuilder()..update(updates))._build();

  _$CreatedAcceptanceLinkResponse._({this.link, this.token, this.publicPath})
      : super._();
  @override
  CreatedAcceptanceLinkResponse rebuild(
          void Function(CreatedAcceptanceLinkResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreatedAcceptanceLinkResponseBuilder toBuilder() =>
      CreatedAcceptanceLinkResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreatedAcceptanceLinkResponse &&
        link == other.link &&
        token == other.token &&
        publicPath == other.publicPath;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, link.hashCode);
    _$hash = $jc(_$hash, token.hashCode);
    _$hash = $jc(_$hash, publicPath.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreatedAcceptanceLinkResponse')
          ..add('link', link)
          ..add('token', token)
          ..add('publicPath', publicPath))
        .toString();
  }
}

class CreatedAcceptanceLinkResponseBuilder
    implements
        Builder<CreatedAcceptanceLinkResponse,
            CreatedAcceptanceLinkResponseBuilder> {
  _$CreatedAcceptanceLinkResponse? _$v;

  AcceptanceLinkResponseBuilder? _link;
  AcceptanceLinkResponseBuilder get link =>
      _$this._link ??= AcceptanceLinkResponseBuilder();
  set link(AcceptanceLinkResponseBuilder? link) => _$this._link = link;

  String? _token;
  String? get token => _$this._token;
  set token(String? token) => _$this._token = token;

  String? _publicPath;
  String? get publicPath => _$this._publicPath;
  set publicPath(String? publicPath) => _$this._publicPath = publicPath;

  CreatedAcceptanceLinkResponseBuilder() {
    CreatedAcceptanceLinkResponse._defaults(this);
  }

  CreatedAcceptanceLinkResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _link = $v.link?.toBuilder();
      _token = $v.token;
      _publicPath = $v.publicPath;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreatedAcceptanceLinkResponse other) {
    _$v = other as _$CreatedAcceptanceLinkResponse;
  }

  @override
  void update(void Function(CreatedAcceptanceLinkResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreatedAcceptanceLinkResponse build() => _build();

  _$CreatedAcceptanceLinkResponse _build() {
    _$CreatedAcceptanceLinkResponse _$result;
    try {
      _$result = _$v ??
          _$CreatedAcceptanceLinkResponse._(
            link: _link?.build(),
            token: token,
            publicPath: publicPath,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'link';
        _link?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreatedAcceptanceLinkResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
