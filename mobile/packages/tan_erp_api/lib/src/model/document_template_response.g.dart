// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'document_template_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DocumentTemplateResponse extends DocumentTemplateResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? branchId;
  @override
  final String? documentType;
  @override
  final String? name;
  @override
  final bool? isDefault;
  @override
  final String? layoutConfigJson;
  @override
  final String? logoFileId;
  @override
  final bool? isActive;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;
  @override
  final String? rowVersion;

  factory _$DocumentTemplateResponse(
          [void Function(DocumentTemplateResponseBuilder)? updates]) =>
      (DocumentTemplateResponseBuilder()..update(updates))._build();

  _$DocumentTemplateResponse._(
      {this.id,
      this.organizationId,
      this.branchId,
      this.documentType,
      this.name,
      this.isDefault,
      this.layoutConfigJson,
      this.logoFileId,
      this.isActive,
      this.createdAtUtc,
      this.updatedAtUtc,
      this.rowVersion})
      : super._();
  @override
  DocumentTemplateResponse rebuild(
          void Function(DocumentTemplateResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DocumentTemplateResponseBuilder toBuilder() =>
      DocumentTemplateResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DocumentTemplateResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        branchId == other.branchId &&
        documentType == other.documentType &&
        name == other.name &&
        isDefault == other.isDefault &&
        layoutConfigJson == other.layoutConfigJson &&
        logoFileId == other.logoFileId &&
        isActive == other.isActive &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, documentType.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, isDefault.hashCode);
    _$hash = $jc(_$hash, layoutConfigJson.hashCode);
    _$hash = $jc(_$hash, logoFileId.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DocumentTemplateResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('branchId', branchId)
          ..add('documentType', documentType)
          ..add('name', name)
          ..add('isDefault', isDefault)
          ..add('layoutConfigJson', layoutConfigJson)
          ..add('logoFileId', logoFileId)
          ..add('isActive', isActive)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class DocumentTemplateResponseBuilder
    implements
        Builder<DocumentTemplateResponse, DocumentTemplateResponseBuilder> {
  _$DocumentTemplateResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _documentType;
  String? get documentType => _$this._documentType;
  set documentType(String? documentType) => _$this._documentType = documentType;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  bool? _isDefault;
  bool? get isDefault => _$this._isDefault;
  set isDefault(bool? isDefault) => _$this._isDefault = isDefault;

  String? _layoutConfigJson;
  String? get layoutConfigJson => _$this._layoutConfigJson;
  set layoutConfigJson(String? layoutConfigJson) =>
      _$this._layoutConfigJson = layoutConfigJson;

  String? _logoFileId;
  String? get logoFileId => _$this._logoFileId;
  set logoFileId(String? logoFileId) => _$this._logoFileId = logoFileId;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DocumentTemplateResponseBuilder() {
    DocumentTemplateResponse._defaults(this);
  }

  DocumentTemplateResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _branchId = $v.branchId;
      _documentType = $v.documentType;
      _name = $v.name;
      _isDefault = $v.isDefault;
      _layoutConfigJson = $v.layoutConfigJson;
      _logoFileId = $v.logoFileId;
      _isActive = $v.isActive;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DocumentTemplateResponse other) {
    _$v = other as _$DocumentTemplateResponse;
  }

  @override
  void update(void Function(DocumentTemplateResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DocumentTemplateResponse build() => _build();

  _$DocumentTemplateResponse _build() {
    final _$result = _$v ??
        _$DocumentTemplateResponse._(
          id: id,
          organizationId: organizationId,
          branchId: branchId,
          documentType: documentType,
          name: name,
          isDefault: isDefault,
          layoutConfigJson: layoutConfigJson,
          logoFileId: logoFileId,
          isActive: isActive,
          createdAtUtc: createdAtUtc,
          updatedAtUtc: updatedAtUtc,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
