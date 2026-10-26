// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_upload_session_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateUploadSessionResponse extends CreateUploadSessionResponse {
  @override
  final String? sessionId;
  @override
  final DateTime? expiresAtUtc;
  @override
  final BuiltList<UploadSlotResponse>? slots;

  factory _$CreateUploadSessionResponse(
          [void Function(CreateUploadSessionResponseBuilder)? updates]) =>
      (CreateUploadSessionResponseBuilder()..update(updates))._build();

  _$CreateUploadSessionResponse._(
      {this.sessionId, this.expiresAtUtc, this.slots})
      : super._();
  @override
  CreateUploadSessionResponse rebuild(
          void Function(CreateUploadSessionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateUploadSessionResponseBuilder toBuilder() =>
      CreateUploadSessionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateUploadSessionResponse &&
        sessionId == other.sessionId &&
        expiresAtUtc == other.expiresAtUtc &&
        slots == other.slots;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sessionId.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jc(_$hash, slots.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateUploadSessionResponse')
          ..add('sessionId', sessionId)
          ..add('expiresAtUtc', expiresAtUtc)
          ..add('slots', slots))
        .toString();
  }
}

class CreateUploadSessionResponseBuilder
    implements
        Builder<CreateUploadSessionResponse,
            CreateUploadSessionResponseBuilder> {
  _$CreateUploadSessionResponse? _$v;

  String? _sessionId;
  String? get sessionId => _$this._sessionId;
  set sessionId(String? sessionId) => _$this._sessionId = sessionId;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  ListBuilder<UploadSlotResponse>? _slots;
  ListBuilder<UploadSlotResponse> get slots =>
      _$this._slots ??= ListBuilder<UploadSlotResponse>();
  set slots(ListBuilder<UploadSlotResponse>? slots) => _$this._slots = slots;

  CreateUploadSessionResponseBuilder() {
    CreateUploadSessionResponse._defaults(this);
  }

  CreateUploadSessionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sessionId = $v.sessionId;
      _expiresAtUtc = $v.expiresAtUtc;
      _slots = $v.slots?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateUploadSessionResponse other) {
    _$v = other as _$CreateUploadSessionResponse;
  }

  @override
  void update(void Function(CreateUploadSessionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateUploadSessionResponse build() => _build();

  _$CreateUploadSessionResponse _build() {
    _$CreateUploadSessionResponse _$result;
    try {
      _$result = _$v ??
          _$CreateUploadSessionResponse._(
            sessionId: sessionId,
            expiresAtUtc: expiresAtUtc,
            slots: _slots?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'slots';
        _slots?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateUploadSessionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
