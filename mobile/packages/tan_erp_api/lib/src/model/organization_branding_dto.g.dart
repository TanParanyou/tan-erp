// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'organization_branding_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OrganizationBrandingDto extends OrganizationBrandingDto {
  @override
  final String? primaryColor;
  @override
  final String? logoUrl;
  @override
  final String? logoFileId;

  factory _$OrganizationBrandingDto(
          [void Function(OrganizationBrandingDtoBuilder)? updates]) =>
      (OrganizationBrandingDtoBuilder()..update(updates))._build();

  _$OrganizationBrandingDto._(
      {this.primaryColor, this.logoUrl, this.logoFileId})
      : super._();
  @override
  OrganizationBrandingDto rebuild(
          void Function(OrganizationBrandingDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OrganizationBrandingDtoBuilder toBuilder() =>
      OrganizationBrandingDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OrganizationBrandingDto &&
        primaryColor == other.primaryColor &&
        logoUrl == other.logoUrl &&
        logoFileId == other.logoFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, primaryColor.hashCode);
    _$hash = $jc(_$hash, logoUrl.hashCode);
    _$hash = $jc(_$hash, logoFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OrganizationBrandingDto')
          ..add('primaryColor', primaryColor)
          ..add('logoUrl', logoUrl)
          ..add('logoFileId', logoFileId))
        .toString();
  }
}

class OrganizationBrandingDtoBuilder
    implements
        Builder<OrganizationBrandingDto, OrganizationBrandingDtoBuilder> {
  _$OrganizationBrandingDto? _$v;

  String? _primaryColor;
  String? get primaryColor => _$this._primaryColor;
  set primaryColor(String? primaryColor) => _$this._primaryColor = primaryColor;

  String? _logoUrl;
  String? get logoUrl => _$this._logoUrl;
  set logoUrl(String? logoUrl) => _$this._logoUrl = logoUrl;

  String? _logoFileId;
  String? get logoFileId => _$this._logoFileId;
  set logoFileId(String? logoFileId) => _$this._logoFileId = logoFileId;

  OrganizationBrandingDtoBuilder() {
    OrganizationBrandingDto._defaults(this);
  }

  OrganizationBrandingDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _primaryColor = $v.primaryColor;
      _logoUrl = $v.logoUrl;
      _logoFileId = $v.logoFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OrganizationBrandingDto other) {
    _$v = other as _$OrganizationBrandingDto;
  }

  @override
  void update(void Function(OrganizationBrandingDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OrganizationBrandingDto build() => _build();

  _$OrganizationBrandingDto _build() {
    final _$result = _$v ??
        _$OrganizationBrandingDto._(
          primaryColor: primaryColor,
          logoUrl: logoUrl,
          logoFileId: logoFileId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
