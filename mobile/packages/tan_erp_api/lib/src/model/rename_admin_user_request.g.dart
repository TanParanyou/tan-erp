// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'rename_admin_user_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RenameAdminUserRequest extends RenameAdminUserRequest {
  @override
  final String? displayName;

  factory _$RenameAdminUserRequest(
          [void Function(RenameAdminUserRequestBuilder)? updates]) =>
      (RenameAdminUserRequestBuilder()..update(updates))._build();

  _$RenameAdminUserRequest._({this.displayName}) : super._();
  @override
  RenameAdminUserRequest rebuild(
          void Function(RenameAdminUserRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RenameAdminUserRequestBuilder toBuilder() =>
      RenameAdminUserRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RenameAdminUserRequest && displayName == other.displayName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RenameAdminUserRequest')
          ..add('displayName', displayName))
        .toString();
  }
}

class RenameAdminUserRequestBuilder
    implements Builder<RenameAdminUserRequest, RenameAdminUserRequestBuilder> {
  _$RenameAdminUserRequest? _$v;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  RenameAdminUserRequestBuilder() {
    RenameAdminUserRequest._defaults(this);
  }

  RenameAdminUserRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RenameAdminUserRequest other) {
    _$v = other as _$RenameAdminUserRequest;
  }

  @override
  void update(void Function(RenameAdminUserRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RenameAdminUserRequest build() => _build();

  _$RenameAdminUserRequest _build() {
    final _$result = _$v ??
        _$RenameAdminUserRequest._(
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
