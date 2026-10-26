// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'preview_item_import_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PreviewItemImportRequest extends PreviewItemImportRequest {
  @override
  final String? content;
  @override
  final String? kind;
  @override
  final String? mode;

  factory _$PreviewItemImportRequest(
          [void Function(PreviewItemImportRequestBuilder)? updates]) =>
      (PreviewItemImportRequestBuilder()..update(updates))._build();

  _$PreviewItemImportRequest._({this.content, this.kind, this.mode})
      : super._();
  @override
  PreviewItemImportRequest rebuild(
          void Function(PreviewItemImportRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PreviewItemImportRequestBuilder toBuilder() =>
      PreviewItemImportRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PreviewItemImportRequest &&
        content == other.content &&
        kind == other.kind &&
        mode == other.mode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, content.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, mode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PreviewItemImportRequest')
          ..add('content', content)
          ..add('kind', kind)
          ..add('mode', mode))
        .toString();
  }
}

class PreviewItemImportRequestBuilder
    implements
        Builder<PreviewItemImportRequest, PreviewItemImportRequestBuilder> {
  _$PreviewItemImportRequest? _$v;

  String? _content;
  String? get content => _$this._content;
  set content(String? content) => _$this._content = content;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _mode;
  String? get mode => _$this._mode;
  set mode(String? mode) => _$this._mode = mode;

  PreviewItemImportRequestBuilder() {
    PreviewItemImportRequest._defaults(this);
  }

  PreviewItemImportRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _content = $v.content;
      _kind = $v.kind;
      _mode = $v.mode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PreviewItemImportRequest other) {
    _$v = other as _$PreviewItemImportRequest;
  }

  @override
  void update(void Function(PreviewItemImportRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PreviewItemImportRequest build() => _build();

  _$PreviewItemImportRequest _build() {
    final _$result = _$v ??
        _$PreviewItemImportRequest._(
          content: content,
          kind: kind,
          mode: mode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
