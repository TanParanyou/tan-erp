// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_item_import_job_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateItemImportJobRequest extends CreateItemImportJobRequest {
  @override
  final String? kind;
  @override
  final String? mode;
  @override
  final String? content;
  @override
  final String? expectedContentSha256;

  factory _$CreateItemImportJobRequest(
          [void Function(CreateItemImportJobRequestBuilder)? updates]) =>
      (CreateItemImportJobRequestBuilder()..update(updates))._build();

  _$CreateItemImportJobRequest._(
      {this.kind, this.mode, this.content, this.expectedContentSha256})
      : super._();
  @override
  CreateItemImportJobRequest rebuild(
          void Function(CreateItemImportJobRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateItemImportJobRequestBuilder toBuilder() =>
      CreateItemImportJobRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateItemImportJobRequest &&
        kind == other.kind &&
        mode == other.mode &&
        content == other.content &&
        expectedContentSha256 == other.expectedContentSha256;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, mode.hashCode);
    _$hash = $jc(_$hash, content.hashCode);
    _$hash = $jc(_$hash, expectedContentSha256.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateItemImportJobRequest')
          ..add('kind', kind)
          ..add('mode', mode)
          ..add('content', content)
          ..add('expectedContentSha256', expectedContentSha256))
        .toString();
  }
}

class CreateItemImportJobRequestBuilder
    implements
        Builder<CreateItemImportJobRequest, CreateItemImportJobRequestBuilder> {
  _$CreateItemImportJobRequest? _$v;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _mode;
  String? get mode => _$this._mode;
  set mode(String? mode) => _$this._mode = mode;

  String? _content;
  String? get content => _$this._content;
  set content(String? content) => _$this._content = content;

  String? _expectedContentSha256;
  String? get expectedContentSha256 => _$this._expectedContentSha256;
  set expectedContentSha256(String? expectedContentSha256) =>
      _$this._expectedContentSha256 = expectedContentSha256;

  CreateItemImportJobRequestBuilder() {
    CreateItemImportJobRequest._defaults(this);
  }

  CreateItemImportJobRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _kind = $v.kind;
      _mode = $v.mode;
      _content = $v.content;
      _expectedContentSha256 = $v.expectedContentSha256;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateItemImportJobRequest other) {
    _$v = other as _$CreateItemImportJobRequest;
  }

  @override
  void update(void Function(CreateItemImportJobRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateItemImportJobRequest build() => _build();

  _$CreateItemImportJobRequest _build() {
    final _$result = _$v ??
        _$CreateItemImportJobRequest._(
          kind: kind,
          mode: mode,
          content: content,
          expectedContentSha256: expectedContentSha256,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
