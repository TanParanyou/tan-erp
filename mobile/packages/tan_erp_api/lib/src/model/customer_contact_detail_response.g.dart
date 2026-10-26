// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_contact_detail_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerContactDetailResponse extends CustomerContactDetailResponse {
  @override
  final String? id;
  @override
  final String? customerId;
  @override
  final String? name;
  @override
  final String? roleTitle;
  @override
  final String? phone;
  @override
  final String? email;
  @override
  final String? lineId;
  @override
  final String? preferredChannel;
  @override
  final bool? isPrimary;
  @override
  final String? status;
  @override
  final String? rowVersion;

  factory _$CustomerContactDetailResponse(
          [void Function(CustomerContactDetailResponseBuilder)? updates]) =>
      (CustomerContactDetailResponseBuilder()..update(updates))._build();

  _$CustomerContactDetailResponse._(
      {this.id,
      this.customerId,
      this.name,
      this.roleTitle,
      this.phone,
      this.email,
      this.lineId,
      this.preferredChannel,
      this.isPrimary,
      this.status,
      this.rowVersion})
      : super._();
  @override
  CustomerContactDetailResponse rebuild(
          void Function(CustomerContactDetailResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerContactDetailResponseBuilder toBuilder() =>
      CustomerContactDetailResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerContactDetailResponse &&
        id == other.id &&
        customerId == other.customerId &&
        name == other.name &&
        roleTitle == other.roleTitle &&
        phone == other.phone &&
        email == other.email &&
        lineId == other.lineId &&
        preferredChannel == other.preferredChannel &&
        isPrimary == other.isPrimary &&
        status == other.status &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, roleTitle.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, lineId.hashCode);
    _$hash = $jc(_$hash, preferredChannel.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CustomerContactDetailResponse')
          ..add('id', id)
          ..add('customerId', customerId)
          ..add('name', name)
          ..add('roleTitle', roleTitle)
          ..add('phone', phone)
          ..add('email', email)
          ..add('lineId', lineId)
          ..add('preferredChannel', preferredChannel)
          ..add('isPrimary', isPrimary)
          ..add('status', status)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class CustomerContactDetailResponseBuilder
    implements
        Builder<CustomerContactDetailResponse,
            CustomerContactDetailResponseBuilder> {
  _$CustomerContactDetailResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

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

  String? _lineId;
  String? get lineId => _$this._lineId;
  set lineId(String? lineId) => _$this._lineId = lineId;

  String? _preferredChannel;
  String? get preferredChannel => _$this._preferredChannel;
  set preferredChannel(String? preferredChannel) =>
      _$this._preferredChannel = preferredChannel;

  bool? _isPrimary;
  bool? get isPrimary => _$this._isPrimary;
  set isPrimary(bool? isPrimary) => _$this._isPrimary = isPrimary;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  CustomerContactDetailResponseBuilder() {
    CustomerContactDetailResponse._defaults(this);
  }

  CustomerContactDetailResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _customerId = $v.customerId;
      _name = $v.name;
      _roleTitle = $v.roleTitle;
      _phone = $v.phone;
      _email = $v.email;
      _lineId = $v.lineId;
      _preferredChannel = $v.preferredChannel;
      _isPrimary = $v.isPrimary;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerContactDetailResponse other) {
    _$v = other as _$CustomerContactDetailResponse;
  }

  @override
  void update(void Function(CustomerContactDetailResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerContactDetailResponse build() => _build();

  _$CustomerContactDetailResponse _build() {
    final _$result = _$v ??
        _$CustomerContactDetailResponse._(
          id: id,
          customerId: customerId,
          name: name,
          roleTitle: roleTitle,
          phone: phone,
          email: email,
          lineId: lineId,
          preferredChannel: preferredChannel,
          isPrimary: isPrimary,
          status: status,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
