// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'organization_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OrganizationDto extends OrganizationDto {
  @override
  final String? id;
  @override
  final String? name;
  @override
  final OrganizationBrandingDto? branding;

  factory _$OrganizationDto([void Function(OrganizationDtoBuilder)? updates]) =>
      (OrganizationDtoBuilder()..update(updates))._build();

  _$OrganizationDto._({this.id, this.name, this.branding}) : super._();
  @override
  OrganizationDto rebuild(void Function(OrganizationDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OrganizationDtoBuilder toBuilder() => OrganizationDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OrganizationDto &&
        id == other.id &&
        name == other.name &&
        branding == other.branding;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, branding.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OrganizationDto')
          ..add('id', id)
          ..add('name', name)
          ..add('branding', branding))
        .toString();
  }
}

class OrganizationDtoBuilder
    implements Builder<OrganizationDto, OrganizationDtoBuilder> {
  _$OrganizationDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  OrganizationBrandingDtoBuilder? _branding;
  OrganizationBrandingDtoBuilder get branding =>
      _$this._branding ??= OrganizationBrandingDtoBuilder();
  set branding(OrganizationBrandingDtoBuilder? branding) =>
      _$this._branding = branding;

  OrganizationDtoBuilder() {
    OrganizationDto._defaults(this);
  }

  OrganizationDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _branding = $v.branding?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OrganizationDto other) {
    _$v = other as _$OrganizationDto;
  }

  @override
  void update(void Function(OrganizationDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OrganizationDto build() => _build();

  _$OrganizationDto _build() {
    _$OrganizationDto _$result;
    try {
      _$result = _$v ??
          _$OrganizationDto._(
            id: id,
            name: name,
            branding: _branding?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'branding';
        _branding?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'OrganizationDto', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
