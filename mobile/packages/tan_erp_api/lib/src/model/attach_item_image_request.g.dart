// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attach_item_image_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachItemImageRequest extends AttachItemImageRequest {
  @override
  final String fileId;
  @override
  final String? role;
  @override
  final bool? isPrimary;
  @override
  final LocalizedTextInput altText;
  @override
  final LocalizedTextInput? caption;

  factory _$AttachItemImageRequest(
          [void Function(AttachItemImageRequestBuilder)? updates]) =>
      (AttachItemImageRequestBuilder()..update(updates))._build();

  _$AttachItemImageRequest._(
      {required this.fileId,
      this.role,
      this.isPrimary,
      required this.altText,
      this.caption})
      : super._();
  @override
  AttachItemImageRequest rebuild(
          void Function(AttachItemImageRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachItemImageRequestBuilder toBuilder() =>
      AttachItemImageRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachItemImageRequest &&
        fileId == other.fileId &&
        role == other.role &&
        isPrimary == other.isPrimary &&
        altText == other.altText &&
        caption == other.caption;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jc(_$hash, altText.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AttachItemImageRequest')
          ..add('fileId', fileId)
          ..add('role', role)
          ..add('isPrimary', isPrimary)
          ..add('altText', altText)
          ..add('caption', caption))
        .toString();
  }
}

class AttachItemImageRequestBuilder
    implements Builder<AttachItemImageRequest, AttachItemImageRequestBuilder> {
  _$AttachItemImageRequest? _$v;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _role;
  String? get role => _$this._role;
  set role(String? role) => _$this._role = role;

  bool? _isPrimary;
  bool? get isPrimary => _$this._isPrimary;
  set isPrimary(bool? isPrimary) => _$this._isPrimary = isPrimary;

  LocalizedTextInputBuilder? _altText;
  LocalizedTextInputBuilder get altText =>
      _$this._altText ??= LocalizedTextInputBuilder();
  set altText(LocalizedTextInputBuilder? altText) => _$this._altText = altText;

  LocalizedTextInputBuilder? _caption;
  LocalizedTextInputBuilder get caption =>
      _$this._caption ??= LocalizedTextInputBuilder();
  set caption(LocalizedTextInputBuilder? caption) => _$this._caption = caption;

  AttachItemImageRequestBuilder() {
    AttachItemImageRequest._defaults(this);
  }

  AttachItemImageRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fileId = $v.fileId;
      _role = $v.role;
      _isPrimary = $v.isPrimary;
      _altText = $v.altText.toBuilder();
      _caption = $v.caption?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachItemImageRequest other) {
    _$v = other as _$AttachItemImageRequest;
  }

  @override
  void update(void Function(AttachItemImageRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachItemImageRequest build() => _build();

  _$AttachItemImageRequest _build() {
    _$AttachItemImageRequest _$result;
    try {
      _$result = _$v ??
          _$AttachItemImageRequest._(
            fileId: BuiltValueNullFieldError.checkNotNull(
                fileId, r'AttachItemImageRequest', 'fileId'),
            role: role,
            isPrimary: isPrimary,
            altText: altText.build(),
            caption: _caption?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'altText';
        altText.build();
        _$failedField = 'caption';
        _caption?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AttachItemImageRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
