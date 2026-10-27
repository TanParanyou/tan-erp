// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_organization_branding_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateOrganizationBrandingRequest
    extends UpdateOrganizationBrandingRequest {
  @override
  final String? primaryColor;
  @override
  final String? logoFileId;

  factory _$UpdateOrganizationBrandingRequest(
          [void Function(UpdateOrganizationBrandingRequestBuilder)? updates]) =>
      (UpdateOrganizationBrandingRequestBuilder()..update(updates))._build();

  _$UpdateOrganizationBrandingRequest._({this.primaryColor, this.logoFileId})
      : super._();
  @override
  UpdateOrganizationBrandingRequest rebuild(
          void Function(UpdateOrganizationBrandingRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateOrganizationBrandingRequestBuilder toBuilder() =>
      UpdateOrganizationBrandingRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateOrganizationBrandingRequest &&
        primaryColor == other.primaryColor &&
        logoFileId == other.logoFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, primaryColor.hashCode);
    _$hash = $jc(_$hash, logoFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateOrganizationBrandingRequest')
          ..add('primaryColor', primaryColor)
          ..add('logoFileId', logoFileId))
        .toString();
  }
}

class UpdateOrganizationBrandingRequestBuilder
    implements
        Builder<UpdateOrganizationBrandingRequest,
            UpdateOrganizationBrandingRequestBuilder> {
  _$UpdateOrganizationBrandingRequest? _$v;

  String? _primaryColor;
  String? get primaryColor => _$this._primaryColor;
  set primaryColor(String? primaryColor) => _$this._primaryColor = primaryColor;

  String? _logoFileId;
  String? get logoFileId => _$this._logoFileId;
  set logoFileId(String? logoFileId) => _$this._logoFileId = logoFileId;

  UpdateOrganizationBrandingRequestBuilder() {
    UpdateOrganizationBrandingRequest._defaults(this);
  }

  UpdateOrganizationBrandingRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _primaryColor = $v.primaryColor;
      _logoFileId = $v.logoFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateOrganizationBrandingRequest other) {
    _$v = other as _$UpdateOrganizationBrandingRequest;
  }

  @override
  void update(
      void Function(UpdateOrganizationBrandingRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateOrganizationBrandingRequest build() => _build();

  _$UpdateOrganizationBrandingRequest _build() {
    final _$result = _$v ??
        _$UpdateOrganizationBrandingRequest._(
          primaryColor: primaryColor,
          logoFileId: logoFileId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
