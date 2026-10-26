// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_admin_membership_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateAdminMembershipRequest extends UpdateAdminMembershipRequest {
  @override
  final String? branchId;
  @override
  final DateTime? startsAtUtc;
  @override
  final DateTime? expiresAtUtc;

  factory _$UpdateAdminMembershipRequest(
          [void Function(UpdateAdminMembershipRequestBuilder)? updates]) =>
      (UpdateAdminMembershipRequestBuilder()..update(updates))._build();

  _$UpdateAdminMembershipRequest._(
      {this.branchId, this.startsAtUtc, this.expiresAtUtc})
      : super._();
  @override
  UpdateAdminMembershipRequest rebuild(
          void Function(UpdateAdminMembershipRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateAdminMembershipRequestBuilder toBuilder() =>
      UpdateAdminMembershipRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateAdminMembershipRequest &&
        branchId == other.branchId &&
        startsAtUtc == other.startsAtUtc &&
        expiresAtUtc == other.expiresAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, startsAtUtc.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateAdminMembershipRequest')
          ..add('branchId', branchId)
          ..add('startsAtUtc', startsAtUtc)
          ..add('expiresAtUtc', expiresAtUtc))
        .toString();
  }
}

class UpdateAdminMembershipRequestBuilder
    implements
        Builder<UpdateAdminMembershipRequest,
            UpdateAdminMembershipRequestBuilder> {
  _$UpdateAdminMembershipRequest? _$v;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  DateTime? _startsAtUtc;
  DateTime? get startsAtUtc => _$this._startsAtUtc;
  set startsAtUtc(DateTime? startsAtUtc) => _$this._startsAtUtc = startsAtUtc;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  UpdateAdminMembershipRequestBuilder() {
    UpdateAdminMembershipRequest._defaults(this);
  }

  UpdateAdminMembershipRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _branchId = $v.branchId;
      _startsAtUtc = $v.startsAtUtc;
      _expiresAtUtc = $v.expiresAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateAdminMembershipRequest other) {
    _$v = other as _$UpdateAdminMembershipRequest;
  }

  @override
  void update(void Function(UpdateAdminMembershipRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateAdminMembershipRequest build() => _build();

  _$UpdateAdminMembershipRequest _build() {
    final _$result = _$v ??
        _$UpdateAdminMembershipRequest._(
          branchId: branchId,
          startsAtUtc: startsAtUtc,
          expiresAtUtc: expiresAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
