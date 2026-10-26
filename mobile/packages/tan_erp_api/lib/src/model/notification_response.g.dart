// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$NotificationResponse extends NotificationResponse {
  @override
  final String? id;
  @override
  final String? type;
  @override
  final BuiltMap<String, String>? payload;
  @override
  final String? deepLink;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? readAtUtc;

  factory _$NotificationResponse(
          [void Function(NotificationResponseBuilder)? updates]) =>
      (NotificationResponseBuilder()..update(updates))._build();

  _$NotificationResponse._(
      {this.id,
      this.type,
      this.payload,
      this.deepLink,
      this.createdAtUtc,
      this.readAtUtc})
      : super._();
  @override
  NotificationResponse rebuild(
          void Function(NotificationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  NotificationResponseBuilder toBuilder() =>
      NotificationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is NotificationResponse &&
        id == other.id &&
        type == other.type &&
        payload == other.payload &&
        deepLink == other.deepLink &&
        createdAtUtc == other.createdAtUtc &&
        readAtUtc == other.readAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, type.hashCode);
    _$hash = $jc(_$hash, payload.hashCode);
    _$hash = $jc(_$hash, deepLink.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, readAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'NotificationResponse')
          ..add('id', id)
          ..add('type', type)
          ..add('payload', payload)
          ..add('deepLink', deepLink)
          ..add('createdAtUtc', createdAtUtc)
          ..add('readAtUtc', readAtUtc))
        .toString();
  }
}

class NotificationResponseBuilder
    implements Builder<NotificationResponse, NotificationResponseBuilder> {
  _$NotificationResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _type;
  String? get type => _$this._type;
  set type(String? type) => _$this._type = type;

  MapBuilder<String, String>? _payload;
  MapBuilder<String, String> get payload =>
      _$this._payload ??= MapBuilder<String, String>();
  set payload(MapBuilder<String, String>? payload) => _$this._payload = payload;

  String? _deepLink;
  String? get deepLink => _$this._deepLink;
  set deepLink(String? deepLink) => _$this._deepLink = deepLink;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _readAtUtc;
  DateTime? get readAtUtc => _$this._readAtUtc;
  set readAtUtc(DateTime? readAtUtc) => _$this._readAtUtc = readAtUtc;

  NotificationResponseBuilder() {
    NotificationResponse._defaults(this);
  }

  NotificationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _type = $v.type;
      _payload = $v.payload?.toBuilder();
      _deepLink = $v.deepLink;
      _createdAtUtc = $v.createdAtUtc;
      _readAtUtc = $v.readAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(NotificationResponse other) {
    _$v = other as _$NotificationResponse;
  }

  @override
  void update(void Function(NotificationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  NotificationResponse build() => _build();

  _$NotificationResponse _build() {
    _$NotificationResponse _$result;
    try {
      _$result = _$v ??
          _$NotificationResponse._(
            id: id,
            type: type,
            payload: _payload?.build(),
            deepLink: deepLink,
            createdAtUtc: createdAtUtc,
            readAtUtc: readAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'payload';
        _payload?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'NotificationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
