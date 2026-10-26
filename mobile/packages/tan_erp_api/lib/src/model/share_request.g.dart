// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'share_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ShareRequest extends ShareRequest {
  @override
  final int? sourceVersion;
  @override
  final String? channel;
  @override
  final String? recipient;
  @override
  final String? locale;

  factory _$ShareRequest([void Function(ShareRequestBuilder)? updates]) =>
      (ShareRequestBuilder()..update(updates))._build();

  _$ShareRequest._(
      {this.sourceVersion, this.channel, this.recipient, this.locale})
      : super._();
  @override
  ShareRequest rebuild(void Function(ShareRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ShareRequestBuilder toBuilder() => ShareRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ShareRequest &&
        sourceVersion == other.sourceVersion &&
        channel == other.channel &&
        recipient == other.recipient &&
        locale == other.locale;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sourceVersion.hashCode);
    _$hash = $jc(_$hash, channel.hashCode);
    _$hash = $jc(_$hash, recipient.hashCode);
    _$hash = $jc(_$hash, locale.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ShareRequest')
          ..add('sourceVersion', sourceVersion)
          ..add('channel', channel)
          ..add('recipient', recipient)
          ..add('locale', locale))
        .toString();
  }
}

class ShareRequestBuilder
    implements Builder<ShareRequest, ShareRequestBuilder> {
  _$ShareRequest? _$v;

  int? _sourceVersion;
  int? get sourceVersion => _$this._sourceVersion;
  set sourceVersion(int? sourceVersion) =>
      _$this._sourceVersion = sourceVersion;

  String? _channel;
  String? get channel => _$this._channel;
  set channel(String? channel) => _$this._channel = channel;

  String? _recipient;
  String? get recipient => _$this._recipient;
  set recipient(String? recipient) => _$this._recipient = recipient;

  String? _locale;
  String? get locale => _$this._locale;
  set locale(String? locale) => _$this._locale = locale;

  ShareRequestBuilder() {
    ShareRequest._defaults(this);
  }

  ShareRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sourceVersion = $v.sourceVersion;
      _channel = $v.channel;
      _recipient = $v.recipient;
      _locale = $v.locale;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ShareRequest other) {
    _$v = other as _$ShareRequest;
  }

  @override
  void update(void Function(ShareRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ShareRequest build() => _build();

  _$ShareRequest _build() {
    final _$result = _$v ??
        _$ShareRequest._(
          sourceVersion: sourceVersion,
          channel: channel,
          recipient: recipient,
          locale: locale,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
