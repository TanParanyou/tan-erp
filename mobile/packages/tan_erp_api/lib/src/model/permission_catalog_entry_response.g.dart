// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'permission_catalog_entry_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PermissionCatalogEntryResponse extends PermissionCatalogEntryResponse {
  @override
  final String? key;
  @override
  final String? description;
  @override
  final String? module;
  @override
  final bool? requiresMakerChecker;
  @override
  final bool? isDocumentApproval;
  @override
  final bool? isApprovalPermission;

  factory _$PermissionCatalogEntryResponse(
          [void Function(PermissionCatalogEntryResponseBuilder)? updates]) =>
      (PermissionCatalogEntryResponseBuilder()..update(updates))._build();

  _$PermissionCatalogEntryResponse._(
      {this.key,
      this.description,
      this.module,
      this.requiresMakerChecker,
      this.isDocumentApproval,
      this.isApprovalPermission})
      : super._();
  @override
  PermissionCatalogEntryResponse rebuild(
          void Function(PermissionCatalogEntryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PermissionCatalogEntryResponseBuilder toBuilder() =>
      PermissionCatalogEntryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PermissionCatalogEntryResponse &&
        key == other.key &&
        description == other.description &&
        module == other.module &&
        requiresMakerChecker == other.requiresMakerChecker &&
        isDocumentApproval == other.isDocumentApproval &&
        isApprovalPermission == other.isApprovalPermission;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, key.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, module.hashCode);
    _$hash = $jc(_$hash, requiresMakerChecker.hashCode);
    _$hash = $jc(_$hash, isDocumentApproval.hashCode);
    _$hash = $jc(_$hash, isApprovalPermission.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PermissionCatalogEntryResponse')
          ..add('key', key)
          ..add('description', description)
          ..add('module', module)
          ..add('requiresMakerChecker', requiresMakerChecker)
          ..add('isDocumentApproval', isDocumentApproval)
          ..add('isApprovalPermission', isApprovalPermission))
        .toString();
  }
}

class PermissionCatalogEntryResponseBuilder
    implements
        Builder<PermissionCatalogEntryResponse,
            PermissionCatalogEntryResponseBuilder> {
  _$PermissionCatalogEntryResponse? _$v;

  String? _key;
  String? get key => _$this._key;
  set key(String? key) => _$this._key = key;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  String? _module;
  String? get module => _$this._module;
  set module(String? module) => _$this._module = module;

  bool? _requiresMakerChecker;
  bool? get requiresMakerChecker => _$this._requiresMakerChecker;
  set requiresMakerChecker(bool? requiresMakerChecker) =>
      _$this._requiresMakerChecker = requiresMakerChecker;

  bool? _isDocumentApproval;
  bool? get isDocumentApproval => _$this._isDocumentApproval;
  set isDocumentApproval(bool? isDocumentApproval) =>
      _$this._isDocumentApproval = isDocumentApproval;

  bool? _isApprovalPermission;
  bool? get isApprovalPermission => _$this._isApprovalPermission;
  set isApprovalPermission(bool? isApprovalPermission) =>
      _$this._isApprovalPermission = isApprovalPermission;

  PermissionCatalogEntryResponseBuilder() {
    PermissionCatalogEntryResponse._defaults(this);
  }

  PermissionCatalogEntryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _key = $v.key;
      _description = $v.description;
      _module = $v.module;
      _requiresMakerChecker = $v.requiresMakerChecker;
      _isDocumentApproval = $v.isDocumentApproval;
      _isApprovalPermission = $v.isApprovalPermission;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PermissionCatalogEntryResponse other) {
    _$v = other as _$PermissionCatalogEntryResponse;
  }

  @override
  void update(void Function(PermissionCatalogEntryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PermissionCatalogEntryResponse build() => _build();

  _$PermissionCatalogEntryResponse _build() {
    final _$result = _$v ??
        _$PermissionCatalogEntryResponse._(
          key: key,
          description: description,
          module: module,
          requiresMakerChecker: requiresMakerChecker,
          isDocumentApproval: isDocumentApproval,
          isApprovalPermission: isApprovalPermission,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
