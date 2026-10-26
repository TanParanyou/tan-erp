// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'preview_document_sequence_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PreviewDocumentSequenceResponse
    extends PreviewDocumentSequenceResponse {
  @override
  final String? preview;

  factory _$PreviewDocumentSequenceResponse(
          [void Function(PreviewDocumentSequenceResponseBuilder)? updates]) =>
      (PreviewDocumentSequenceResponseBuilder()..update(updates))._build();

  _$PreviewDocumentSequenceResponse._({this.preview}) : super._();
  @override
  PreviewDocumentSequenceResponse rebuild(
          void Function(PreviewDocumentSequenceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PreviewDocumentSequenceResponseBuilder toBuilder() =>
      PreviewDocumentSequenceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PreviewDocumentSequenceResponse && preview == other.preview;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, preview.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PreviewDocumentSequenceResponse')
          ..add('preview', preview))
        .toString();
  }
}

class PreviewDocumentSequenceResponseBuilder
    implements
        Builder<PreviewDocumentSequenceResponse,
            PreviewDocumentSequenceResponseBuilder> {
  _$PreviewDocumentSequenceResponse? _$v;

  String? _preview;
  String? get preview => _$this._preview;
  set preview(String? preview) => _$this._preview = preview;

  PreviewDocumentSequenceResponseBuilder() {
    PreviewDocumentSequenceResponse._defaults(this);
  }

  PreviewDocumentSequenceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _preview = $v.preview;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PreviewDocumentSequenceResponse other) {
    _$v = other as _$PreviewDocumentSequenceResponse;
  }

  @override
  void update(void Function(PreviewDocumentSequenceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PreviewDocumentSequenceResponse build() => _build();

  _$PreviewDocumentSequenceResponse _build() {
    final _$result = _$v ??
        _$PreviewDocumentSequenceResponse._(
          preview: preview,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
