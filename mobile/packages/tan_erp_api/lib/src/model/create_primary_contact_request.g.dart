// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_primary_contact_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreatePrimaryContactRequest extends CreatePrimaryContactRequest {
  @override
  final String? name;
  @override
  final String? roleTitle;
  @override
  final String? phone;
  @override
  final String? email;
  @override
  final String? preferredChannel;
  @override
  final String? lineId;

  factory _$CreatePrimaryContactRequest(
          [void Function(CreatePrimaryContactRequestBuilder)? updates]) =>
      (CreatePrimaryContactRequestBuilder()..update(updates))._build();

  _$CreatePrimaryContactRequest._(
      {this.name,
      this.roleTitle,
      this.phone,
      this.email,
      this.preferredChannel,
      this.lineId})
      : super._();
  @override
  CreatePrimaryContactRequest rebuild(
          void Function(CreatePrimaryContactRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreatePrimaryContactRequestBuilder toBuilder() =>
      CreatePrimaryContactRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreatePrimaryContactRequest &&
        name == other.name &&
        roleTitle == other.roleTitle &&
        phone == other.phone &&
        email == other.email &&
        preferredChannel == other.preferredChannel &&
        lineId == other.lineId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, roleTitle.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, preferredChannel.hashCode);
    _$hash = $jc(_$hash, lineId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreatePrimaryContactRequest')
          ..add('name', name)
          ..add('roleTitle', roleTitle)
          ..add('phone', phone)
          ..add('email', email)
          ..add('preferredChannel', preferredChannel)
          ..add('lineId', lineId))
        .toString();
  }
}

class CreatePrimaryContactRequestBuilder
    implements
        Builder<CreatePrimaryContactRequest,
            CreatePrimaryContactRequestBuilder> {
  _$CreatePrimaryContactRequest? _$v;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _roleTitle;
  String? get roleTitle => _$this._roleTitle;
  set roleTitle(String? roleTitle) => _$this._roleTitle = roleTitle;

  String? _phone;
  String? get phone => _$this._phone;
  set phone(String? phone) => _$this._phone = phone;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  String? _preferredChannel;
  String? get preferredChannel => _$this._preferredChannel;
  set preferredChannel(String? preferredChannel) =>
      _$this._preferredChannel = preferredChannel;

  String? _lineId;
  String? get lineId => _$this._lineId;
  set lineId(String? lineId) => _$this._lineId = lineId;

  CreatePrimaryContactRequestBuilder() {
    CreatePrimaryContactRequest._defaults(this);
  }

  CreatePrimaryContactRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _roleTitle = $v.roleTitle;
      _phone = $v.phone;
      _email = $v.email;
      _preferredChannel = $v.preferredChannel;
      _lineId = $v.lineId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreatePrimaryContactRequest other) {
    _$v = other as _$CreatePrimaryContactRequest;
  }

  @override
  void update(void Function(CreatePrimaryContactRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreatePrimaryContactRequest build() => _build();

  _$CreatePrimaryContactRequest _build() {
    final _$result = _$v ??
        _$CreatePrimaryContactRequest._(
          name: name,
          roleTitle: roleTitle,
          phone: phone,
          email: email,
          preferredChannel: preferredChannel,
          lineId: lineId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
