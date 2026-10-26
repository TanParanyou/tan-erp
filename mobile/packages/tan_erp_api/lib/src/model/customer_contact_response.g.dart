// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_contact_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerContactResponse extends CustomerContactResponse {
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
  final bool? isMasked;
  @override
  final String? lineId;

  factory _$CustomerContactResponse(
          [void Function(CustomerContactResponseBuilder)? updates]) =>
      (CustomerContactResponseBuilder()..update(updates))._build();

  _$CustomerContactResponse._(
      {this.name,
      this.roleTitle,
      this.phone,
      this.email,
      this.preferredChannel,
      this.isMasked,
      this.lineId})
      : super._();
  @override
  CustomerContactResponse rebuild(
          void Function(CustomerContactResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerContactResponseBuilder toBuilder() =>
      CustomerContactResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerContactResponse &&
        name == other.name &&
        roleTitle == other.roleTitle &&
        phone == other.phone &&
        email == other.email &&
        preferredChannel == other.preferredChannel &&
        isMasked == other.isMasked &&
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
    _$hash = $jc(_$hash, isMasked.hashCode);
    _$hash = $jc(_$hash, lineId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CustomerContactResponse')
          ..add('name', name)
          ..add('roleTitle', roleTitle)
          ..add('phone', phone)
          ..add('email', email)
          ..add('preferredChannel', preferredChannel)
          ..add('isMasked', isMasked)
          ..add('lineId', lineId))
        .toString();
  }
}

class CustomerContactResponseBuilder
    implements
        Builder<CustomerContactResponse, CustomerContactResponseBuilder> {
  _$CustomerContactResponse? _$v;

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

  bool? _isMasked;
  bool? get isMasked => _$this._isMasked;
  set isMasked(bool? isMasked) => _$this._isMasked = isMasked;

  String? _lineId;
  String? get lineId => _$this._lineId;
  set lineId(String? lineId) => _$this._lineId = lineId;

  CustomerContactResponseBuilder() {
    CustomerContactResponse._defaults(this);
  }

  CustomerContactResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _roleTitle = $v.roleTitle;
      _phone = $v.phone;
      _email = $v.email;
      _preferredChannel = $v.preferredChannel;
      _isMasked = $v.isMasked;
      _lineId = $v.lineId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerContactResponse other) {
    _$v = other as _$CustomerContactResponse;
  }

  @override
  void update(void Function(CustomerContactResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerContactResponse build() => _build();

  _$CustomerContactResponse _build() {
    final _$result = _$v ??
        _$CustomerContactResponse._(
          name: name,
          roleTitle: roleTitle,
          phone: phone,
          email: email,
          preferredChannel: preferredChannel,
          isMasked: isMasked,
          lineId: lineId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
