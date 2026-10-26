// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_upload_session_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateUploadSessionRequest extends CreateUploadSessionRequest {
  @override
  final String? parentType;
  @override
  final String? parentId;
  @override
  final String? creationIntentId;
  @override
  final BuiltList<FileSlotRequest>? files;

  factory _$CreateUploadSessionRequest(
          [void Function(CreateUploadSessionRequestBuilder)? updates]) =>
      (CreateUploadSessionRequestBuilder()..update(updates))._build();

  _$CreateUploadSessionRequest._(
      {this.parentType, this.parentId, this.creationIntentId, this.files})
      : super._();
  @override
  CreateUploadSessionRequest rebuild(
          void Function(CreateUploadSessionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateUploadSessionRequestBuilder toBuilder() =>
      CreateUploadSessionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateUploadSessionRequest &&
        parentType == other.parentType &&
        parentId == other.parentId &&
        creationIntentId == other.creationIntentId &&
        files == other.files;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, parentType.hashCode);
    _$hash = $jc(_$hash, parentId.hashCode);
    _$hash = $jc(_$hash, creationIntentId.hashCode);
    _$hash = $jc(_$hash, files.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateUploadSessionRequest')
          ..add('parentType', parentType)
          ..add('parentId', parentId)
          ..add('creationIntentId', creationIntentId)
          ..add('files', files))
        .toString();
  }
}

class CreateUploadSessionRequestBuilder
    implements
        Builder<CreateUploadSessionRequest, CreateUploadSessionRequestBuilder> {
  _$CreateUploadSessionRequest? _$v;

  String? _parentType;
  String? get parentType => _$this._parentType;
  set parentType(String? parentType) => _$this._parentType = parentType;

  String? _parentId;
  String? get parentId => _$this._parentId;
  set parentId(String? parentId) => _$this._parentId = parentId;

  String? _creationIntentId;
  String? get creationIntentId => _$this._creationIntentId;
  set creationIntentId(String? creationIntentId) =>
      _$this._creationIntentId = creationIntentId;

  ListBuilder<FileSlotRequest>? _files;
  ListBuilder<FileSlotRequest> get files =>
      _$this._files ??= ListBuilder<FileSlotRequest>();
  set files(ListBuilder<FileSlotRequest>? files) => _$this._files = files;

  CreateUploadSessionRequestBuilder() {
    CreateUploadSessionRequest._defaults(this);
  }

  CreateUploadSessionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _parentType = $v.parentType;
      _parentId = $v.parentId;
      _creationIntentId = $v.creationIntentId;
      _files = $v.files?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateUploadSessionRequest other) {
    _$v = other as _$CreateUploadSessionRequest;
  }

  @override
  void update(void Function(CreateUploadSessionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateUploadSessionRequest build() => _build();

  _$CreateUploadSessionRequest _build() {
    _$CreateUploadSessionRequest _$result;
    try {
      _$result = _$v ??
          _$CreateUploadSessionRequest._(
            parentType: parentType,
            parentId: parentId,
            creationIntentId: creationIntentId,
            files: _files?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'files';
        _files?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateUploadSessionRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
