// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_document_template_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateDocumentTemplateRequest extends CreateDocumentTemplateRequest {
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
  final String? branchId;

  factory _$CreateDocumentTemplateRequest(
          [void Function(CreateDocumentTemplateRequestBuilder)? updates]) =>
      (CreateDocumentTemplateRequestBuilder()..update(updates))._build();

  _$CreateDocumentTemplateRequest._(
      {this.documentType,
      this.name,
      this.isDefault,
      this.layoutConfigJson,
      this.logoFileId,
      this.branchId})
      : super._();
  @override
  CreateDocumentTemplateRequest rebuild(
          void Function(CreateDocumentTemplateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateDocumentTemplateRequestBuilder toBuilder() =>
      CreateDocumentTemplateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateDocumentTemplateRequest &&
        documentType == other.documentType &&
        name == other.name &&
        isDefault == other.isDefault &&
        layoutConfigJson == other.layoutConfigJson &&
        logoFileId == other.logoFileId &&
        branchId == other.branchId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, documentType.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, isDefault.hashCode);
    _$hash = $jc(_$hash, layoutConfigJson.hashCode);
    _$hash = $jc(_$hash, logoFileId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateDocumentTemplateRequest')
          ..add('documentType', documentType)
          ..add('name', name)
          ..add('isDefault', isDefault)
          ..add('layoutConfigJson', layoutConfigJson)
          ..add('logoFileId', logoFileId)
          ..add('branchId', branchId))
        .toString();
  }
}

class CreateDocumentTemplateRequestBuilder
    implements
        Builder<CreateDocumentTemplateRequest,
            CreateDocumentTemplateRequestBuilder> {
  _$CreateDocumentTemplateRequest? _$v;

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

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  CreateDocumentTemplateRequestBuilder() {
    CreateDocumentTemplateRequest._defaults(this);
  }

  CreateDocumentTemplateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _documentType = $v.documentType;
      _name = $v.name;
      _isDefault = $v.isDefault;
      _layoutConfigJson = $v.layoutConfigJson;
      _logoFileId = $v.logoFileId;
      _branchId = $v.branchId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateDocumentTemplateRequest other) {
    _$v = other as _$CreateDocumentTemplateRequest;
  }

  @override
  void update(void Function(CreateDocumentTemplateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateDocumentTemplateRequest build() => _build();

  _$CreateDocumentTemplateRequest _build() {
    final _$result = _$v ??
        _$CreateDocumentTemplateRequest._(
          documentType: documentType,
          name: name,
          isDefault: isDefault,
          layoutConfigJson: layoutConfigJson,
          logoFileId: logoFileId,
          branchId: branchId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
