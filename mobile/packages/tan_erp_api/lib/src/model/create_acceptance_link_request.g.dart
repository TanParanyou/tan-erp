// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_acceptance_link_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateAcceptanceLinkRequest extends CreateAcceptanceLinkRequest {
  @override
  final int? lifetimeDays;
  @override
  final String? signerHint;
  @override
  final String? recipientEmail;
  @override
  final String? recipientPhone;

  factory _$CreateAcceptanceLinkRequest(
          [void Function(CreateAcceptanceLinkRequestBuilder)? updates]) =>
      (CreateAcceptanceLinkRequestBuilder()..update(updates))._build();

  _$CreateAcceptanceLinkRequest._(
      {this.lifetimeDays,
      this.signerHint,
      this.recipientEmail,
      this.recipientPhone})
      : super._();
  @override
  CreateAcceptanceLinkRequest rebuild(
          void Function(CreateAcceptanceLinkRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateAcceptanceLinkRequestBuilder toBuilder() =>
      CreateAcceptanceLinkRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateAcceptanceLinkRequest &&
        lifetimeDays == other.lifetimeDays &&
        signerHint == other.signerHint &&
        recipientEmail == other.recipientEmail &&
        recipientPhone == other.recipientPhone;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, lifetimeDays.hashCode);
    _$hash = $jc(_$hash, signerHint.hashCode);
    _$hash = $jc(_$hash, recipientEmail.hashCode);
    _$hash = $jc(_$hash, recipientPhone.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateAcceptanceLinkRequest')
          ..add('lifetimeDays', lifetimeDays)
          ..add('signerHint', signerHint)
          ..add('recipientEmail', recipientEmail)
          ..add('recipientPhone', recipientPhone))
        .toString();
  }
}

class CreateAcceptanceLinkRequestBuilder
    implements
        Builder<CreateAcceptanceLinkRequest,
            CreateAcceptanceLinkRequestBuilder> {
  _$CreateAcceptanceLinkRequest? _$v;

  int? _lifetimeDays;
  int? get lifetimeDays => _$this._lifetimeDays;
  set lifetimeDays(int? lifetimeDays) => _$this._lifetimeDays = lifetimeDays;

  String? _signerHint;
  String? get signerHint => _$this._signerHint;
  set signerHint(String? signerHint) => _$this._signerHint = signerHint;

  String? _recipientEmail;
  String? get recipientEmail => _$this._recipientEmail;
  set recipientEmail(String? recipientEmail) =>
      _$this._recipientEmail = recipientEmail;

  String? _recipientPhone;
  String? get recipientPhone => _$this._recipientPhone;
  set recipientPhone(String? recipientPhone) =>
      _$this._recipientPhone = recipientPhone;

  CreateAcceptanceLinkRequestBuilder() {
    CreateAcceptanceLinkRequest._defaults(this);
  }

  CreateAcceptanceLinkRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _lifetimeDays = $v.lifetimeDays;
      _signerHint = $v.signerHint;
      _recipientEmail = $v.recipientEmail;
      _recipientPhone = $v.recipientPhone;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateAcceptanceLinkRequest other) {
    _$v = other as _$CreateAcceptanceLinkRequest;
  }

  @override
  void update(void Function(CreateAcceptanceLinkRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateAcceptanceLinkRequest build() => _build();

  _$CreateAcceptanceLinkRequest _build() {
    final _$result = _$v ??
        _$CreateAcceptanceLinkRequest._(
          lifetimeDays: lifetimeDays,
          signerHint: signerHint,
          recipientEmail: recipientEmail,
          recipientPhone: recipientPhone,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
