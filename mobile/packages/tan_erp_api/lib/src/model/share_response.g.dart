// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'share_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ShareResponse extends ShareResponse {
  @override
  final String? id;
  @override
  final int? sourceVersion;
  @override
  final String? channel;
  @override
  final String? recipient;
  @override
  final String? locale;
  @override
  final QePersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? summaryJson;

  factory _$ShareResponse([void Function(ShareResponseBuilder)? updates]) =>
      (ShareResponseBuilder()..update(updates))._build();

  _$ShareResponse._(
      {this.id,
      this.sourceVersion,
      this.channel,
      this.recipient,
      this.locale,
      this.createdBy,
      this.createdAtUtc,
      this.summaryJson})
      : super._();
  @override
  ShareResponse rebuild(void Function(ShareResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ShareResponseBuilder toBuilder() => ShareResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ShareResponse &&
        id == other.id &&
        sourceVersion == other.sourceVersion &&
        channel == other.channel &&
        recipient == other.recipient &&
        locale == other.locale &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        summaryJson == other.summaryJson;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, sourceVersion.hashCode);
    _$hash = $jc(_$hash, channel.hashCode);
    _$hash = $jc(_$hash, recipient.hashCode);
    _$hash = $jc(_$hash, locale.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, summaryJson.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ShareResponse')
          ..add('id', id)
          ..add('sourceVersion', sourceVersion)
          ..add('channel', channel)
          ..add('recipient', recipient)
          ..add('locale', locale)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('summaryJson', summaryJson))
        .toString();
  }
}

class ShareResponseBuilder
    implements Builder<ShareResponse, ShareResponseBuilder> {
  _$ShareResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  QePersonResponseBuilder? _createdBy;
  QePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= QePersonResponseBuilder();
  set createdBy(QePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _summaryJson;
  String? get summaryJson => _$this._summaryJson;
  set summaryJson(String? summaryJson) => _$this._summaryJson = summaryJson;

  ShareResponseBuilder() {
    ShareResponse._defaults(this);
  }

  ShareResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _sourceVersion = $v.sourceVersion;
      _channel = $v.channel;
      _recipient = $v.recipient;
      _locale = $v.locale;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _summaryJson = $v.summaryJson;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ShareResponse other) {
    _$v = other as _$ShareResponse;
  }

  @override
  void update(void Function(ShareResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ShareResponse build() => _build();

  _$ShareResponse _build() {
    _$ShareResponse _$result;
    try {
      _$result = _$v ??
          _$ShareResponse._(
            id: id,
            sourceVersion: sourceVersion,
            channel: channel,
            recipient: recipient,
            locale: locale,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            summaryJson: summaryJson,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ShareResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
