// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'commit_item_import_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CommitItemImportRequest extends CommitItemImportRequest {
  @override
  final String? content;
  @override
  final String? expectedContentSha256;
  @override
  final String? kind;
  @override
  final String? mode;

  factory _$CommitItemImportRequest(
          [void Function(CommitItemImportRequestBuilder)? updates]) =>
      (CommitItemImportRequestBuilder()..update(updates))._build();

  _$CommitItemImportRequest._(
      {this.content, this.expectedContentSha256, this.kind, this.mode})
      : super._();
  @override
  CommitItemImportRequest rebuild(
          void Function(CommitItemImportRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CommitItemImportRequestBuilder toBuilder() =>
      CommitItemImportRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CommitItemImportRequest &&
        content == other.content &&
        expectedContentSha256 == other.expectedContentSha256 &&
        kind == other.kind &&
        mode == other.mode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, content.hashCode);
    _$hash = $jc(_$hash, expectedContentSha256.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, mode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CommitItemImportRequest')
          ..add('content', content)
          ..add('expectedContentSha256', expectedContentSha256)
          ..add('kind', kind)
          ..add('mode', mode))
        .toString();
  }
}

class CommitItemImportRequestBuilder
    implements
        Builder<CommitItemImportRequest, CommitItemImportRequestBuilder> {
  _$CommitItemImportRequest? _$v;

  String? _content;
  String? get content => _$this._content;
  set content(String? content) => _$this._content = content;

  String? _expectedContentSha256;
  String? get expectedContentSha256 => _$this._expectedContentSha256;
  set expectedContentSha256(String? expectedContentSha256) =>
      _$this._expectedContentSha256 = expectedContentSha256;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _mode;
  String? get mode => _$this._mode;
  set mode(String? mode) => _$this._mode = mode;

  CommitItemImportRequestBuilder() {
    CommitItemImportRequest._defaults(this);
  }

  CommitItemImportRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _content = $v.content;
      _expectedContentSha256 = $v.expectedContentSha256;
      _kind = $v.kind;
      _mode = $v.mode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CommitItemImportRequest other) {
    _$v = other as _$CommitItemImportRequest;
  }

  @override
  void update(void Function(CommitItemImportRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CommitItemImportRequest build() => _build();

  _$CommitItemImportRequest _build() {
    final _$result = _$v ??
        _$CommitItemImportRequest._(
          content: content,
          expectedContentSha256: expectedContentSha256,
          kind: kind,
          mode: mode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
