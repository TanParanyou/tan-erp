// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'active_session_item_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ActiveSessionItemDto extends ActiveSessionItemDto {
  @override
  final String? id;
  @override
  final String? deviceLabel;
  @override
  final String? clientPlatform;
  @override
  final String? ipAddress;
  @override
  final String? userAgent;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? lastUsedAtUtc;
  @override
  final DateTime? expiresAtUtc;
  @override
  final bool? isCurrent;

  factory _$ActiveSessionItemDto(
          [void Function(ActiveSessionItemDtoBuilder)? updates]) =>
      (ActiveSessionItemDtoBuilder()..update(updates))._build();

  _$ActiveSessionItemDto._(
      {this.id,
      this.deviceLabel,
      this.clientPlatform,
      this.ipAddress,
      this.userAgent,
      this.createdAtUtc,
      this.lastUsedAtUtc,
      this.expiresAtUtc,
      this.isCurrent})
      : super._();
  @override
  ActiveSessionItemDto rebuild(
          void Function(ActiveSessionItemDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ActiveSessionItemDtoBuilder toBuilder() =>
      ActiveSessionItemDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ActiveSessionItemDto &&
        id == other.id &&
        deviceLabel == other.deviceLabel &&
        clientPlatform == other.clientPlatform &&
        ipAddress == other.ipAddress &&
        userAgent == other.userAgent &&
        createdAtUtc == other.createdAtUtc &&
        lastUsedAtUtc == other.lastUsedAtUtc &&
        expiresAtUtc == other.expiresAtUtc &&
        isCurrent == other.isCurrent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, deviceLabel.hashCode);
    _$hash = $jc(_$hash, clientPlatform.hashCode);
    _$hash = $jc(_$hash, ipAddress.hashCode);
    _$hash = $jc(_$hash, userAgent.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, lastUsedAtUtc.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jc(_$hash, isCurrent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ActiveSessionItemDto')
          ..add('id', id)
          ..add('deviceLabel', deviceLabel)
          ..add('clientPlatform', clientPlatform)
          ..add('ipAddress', ipAddress)
          ..add('userAgent', userAgent)
          ..add('createdAtUtc', createdAtUtc)
          ..add('lastUsedAtUtc', lastUsedAtUtc)
          ..add('expiresAtUtc', expiresAtUtc)
          ..add('isCurrent', isCurrent))
        .toString();
  }
}

class ActiveSessionItemDtoBuilder
    implements Builder<ActiveSessionItemDto, ActiveSessionItemDtoBuilder> {
  _$ActiveSessionItemDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _deviceLabel;
  String? get deviceLabel => _$this._deviceLabel;
  set deviceLabel(String? deviceLabel) => _$this._deviceLabel = deviceLabel;

  String? _clientPlatform;
  String? get clientPlatform => _$this._clientPlatform;
  set clientPlatform(String? clientPlatform) =>
      _$this._clientPlatform = clientPlatform;

  String? _ipAddress;
  String? get ipAddress => _$this._ipAddress;
  set ipAddress(String? ipAddress) => _$this._ipAddress = ipAddress;

  String? _userAgent;
  String? get userAgent => _$this._userAgent;
  set userAgent(String? userAgent) => _$this._userAgent = userAgent;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _lastUsedAtUtc;
  DateTime? get lastUsedAtUtc => _$this._lastUsedAtUtc;
  set lastUsedAtUtc(DateTime? lastUsedAtUtc) =>
      _$this._lastUsedAtUtc = lastUsedAtUtc;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  bool? _isCurrent;
  bool? get isCurrent => _$this._isCurrent;
  set isCurrent(bool? isCurrent) => _$this._isCurrent = isCurrent;

  ActiveSessionItemDtoBuilder() {
    ActiveSessionItemDto._defaults(this);
  }

  ActiveSessionItemDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _deviceLabel = $v.deviceLabel;
      _clientPlatform = $v.clientPlatform;
      _ipAddress = $v.ipAddress;
      _userAgent = $v.userAgent;
      _createdAtUtc = $v.createdAtUtc;
      _lastUsedAtUtc = $v.lastUsedAtUtc;
      _expiresAtUtc = $v.expiresAtUtc;
      _isCurrent = $v.isCurrent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ActiveSessionItemDto other) {
    _$v = other as _$ActiveSessionItemDto;
  }

  @override
  void update(void Function(ActiveSessionItemDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ActiveSessionItemDto build() => _build();

  _$ActiveSessionItemDto _build() {
    final _$result = _$v ??
        _$ActiveSessionItemDto._(
          id: id,
          deviceLabel: deviceLabel,
          clientPlatform: clientPlatform,
          ipAddress: ipAddress,
          userAgent: userAgent,
          createdAtUtc: createdAtUtc,
          lastUsedAtUtc: lastUsedAtUtc,
          expiresAtUtc: expiresAtUtc,
          isCurrent: isCurrent,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
