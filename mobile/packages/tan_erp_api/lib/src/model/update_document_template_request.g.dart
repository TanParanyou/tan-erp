// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_document_template_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateDocumentTemplateRequest extends UpdateDocumentTemplateRequest {
  @override
  final String? name;
  @override
  final bool? isDefault;
  @override
  final String? layoutConfigJson;
  @override
  final String? logoFileId;

  factory _$UpdateDocumentTemplateRequest(
          [void Function(UpdateDocumentTemplateRequestBuilder)? updates]) =>
      (UpdateDocumentTemplateRequestBuilder()..update(updates))._build();

  _$UpdateDocumentTemplateRequest._(
      {this.name, this.isDefault, this.layoutConfigJson, this.logoFileId})
      : super._();
  @override
  UpdateDocumentTemplateRequest rebuild(
          void Function(UpdateDocumentTemplateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateDocumentTemplateRequestBuilder toBuilder() =>
      UpdateDocumentTemplateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateDocumentTemplateRequest &&
        name == other.name &&
        isDefault == other.isDefault &&
        layoutConfigJson == other.layoutConfigJson &&
        logoFileId == other.logoFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, isDefault.hashCode);
    _$hash = $jc(_$hash, layoutConfigJson.hashCode);
    _$hash = $jc(_$hash, logoFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateDocumentTemplateRequest')
          ..add('name', name)
          ..add('isDefault', isDefault)
          ..add('layoutConfigJson', layoutConfigJson)
          ..add('logoFileId', logoFileId))
        .toString();
  }
}

class UpdateDocumentTemplateRequestBuilder
    implements
        Builder<UpdateDocumentTemplateRequest,
            UpdateDocumentTemplateRequestBuilder> {
  _$UpdateDocumentTemplateRequest? _$v;

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

  UpdateDocumentTemplateRequestBuilder() {
    UpdateDocumentTemplateRequest._defaults(this);
  }

  UpdateDocumentTemplateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _isDefault = $v.isDefault;
      _layoutConfigJson = $v.layoutConfigJson;
      _logoFileId = $v.logoFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateDocumentTemplateRequest other) {
    _$v = other as _$UpdateDocumentTemplateRequest;
  }

  @override
  void update(void Function(UpdateDocumentTemplateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateDocumentTemplateRequest build() => _build();

  _$UpdateDocumentTemplateRequest _build() {
    final _$result = _$v ??
        _$UpdateDocumentTemplateRequest._(
          name: name,
          isDefault: isDefault,
          layoutConfigJson: layoutConfigJson,
          logoFileId: logoFileId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
