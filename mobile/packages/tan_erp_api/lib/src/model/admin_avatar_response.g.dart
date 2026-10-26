// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_avatar_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminAvatarResponse extends AdminAvatarResponse {
  @override
  final String? id;
  @override
  final String? servingUrl;

  factory _$AdminAvatarResponse(
          [void Function(AdminAvatarResponseBuilder)? updates]) =>
      (AdminAvatarResponseBuilder()..update(updates))._build();

  _$AdminAvatarResponse._({this.id, this.servingUrl}) : super._();
  @override
  AdminAvatarResponse rebuild(
          void Function(AdminAvatarResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminAvatarResponseBuilder toBuilder() =>
      AdminAvatarResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminAvatarResponse &&
        id == other.id &&
        servingUrl == other.servingUrl;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, servingUrl.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminAvatarResponse')
          ..add('id', id)
          ..add('servingUrl', servingUrl))
        .toString();
  }
}

class AdminAvatarResponseBuilder
    implements Builder<AdminAvatarResponse, AdminAvatarResponseBuilder> {
  _$AdminAvatarResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _servingUrl;
  String? get servingUrl => _$this._servingUrl;
  set servingUrl(String? servingUrl) => _$this._servingUrl = servingUrl;

  AdminAvatarResponseBuilder() {
    AdminAvatarResponse._defaults(this);
  }

  AdminAvatarResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _servingUrl = $v.servingUrl;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminAvatarResponse other) {
    _$v = other as _$AdminAvatarResponse;
  }

  @override
  void update(void Function(AdminAvatarResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminAvatarResponse build() => _build();

  _$AdminAvatarResponse _build() {
    final _$result = _$v ??
        _$AdminAvatarResponse._(
          id: id,
          servingUrl: servingUrl,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
