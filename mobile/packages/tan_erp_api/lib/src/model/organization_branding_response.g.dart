// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'organization_branding_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OrganizationBrandingResponse extends OrganizationBrandingResponse {
  @override
  final String? primaryColor;
  @override
  final String? logoFileId;
  @override
  final String? logoUrl;
  @override
  final String? rowVersion;

  factory _$OrganizationBrandingResponse(
          [void Function(OrganizationBrandingResponseBuilder)? updates]) =>
      (OrganizationBrandingResponseBuilder()..update(updates))._build();

  _$OrganizationBrandingResponse._(
      {this.primaryColor, this.logoFileId, this.logoUrl, this.rowVersion})
      : super._();
  @override
  OrganizationBrandingResponse rebuild(
          void Function(OrganizationBrandingResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OrganizationBrandingResponseBuilder toBuilder() =>
      OrganizationBrandingResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OrganizationBrandingResponse &&
        primaryColor == other.primaryColor &&
        logoFileId == other.logoFileId &&
        logoUrl == other.logoUrl &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, primaryColor.hashCode);
    _$hash = $jc(_$hash, logoFileId.hashCode);
    _$hash = $jc(_$hash, logoUrl.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OrganizationBrandingResponse')
          ..add('primaryColor', primaryColor)
          ..add('logoFileId', logoFileId)
          ..add('logoUrl', logoUrl)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class OrganizationBrandingResponseBuilder
    implements
        Builder<OrganizationBrandingResponse,
            OrganizationBrandingResponseBuilder> {
  _$OrganizationBrandingResponse? _$v;

  String? _primaryColor;
  String? get primaryColor => _$this._primaryColor;
  set primaryColor(String? primaryColor) => _$this._primaryColor = primaryColor;

  String? _logoFileId;
  String? get logoFileId => _$this._logoFileId;
  set logoFileId(String? logoFileId) => _$this._logoFileId = logoFileId;

  String? _logoUrl;
  String? get logoUrl => _$this._logoUrl;
  set logoUrl(String? logoUrl) => _$this._logoUrl = logoUrl;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  OrganizationBrandingResponseBuilder() {
    OrganizationBrandingResponse._defaults(this);
  }

  OrganizationBrandingResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _primaryColor = $v.primaryColor;
      _logoFileId = $v.logoFileId;
      _logoUrl = $v.logoUrl;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OrganizationBrandingResponse other) {
    _$v = other as _$OrganizationBrandingResponse;
  }

  @override
  void update(void Function(OrganizationBrandingResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OrganizationBrandingResponse build() => _build();

  _$OrganizationBrandingResponse _build() {
    final _$result = _$v ??
        _$OrganizationBrandingResponse._(
          primaryColor: primaryColor,
          logoFileId: logoFileId,
          logoUrl: logoUrl,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
