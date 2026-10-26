// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerListItemResponse extends CustomerListItemResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? customerType;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;
  @override
  final String? preferredLocale;
  @override
  final String? status;
  @override
  final CustomerContactResponse? primaryContact;
  @override
  final String? leadSource;

  factory _$CustomerListItemResponse(
          [void Function(CustomerListItemResponseBuilder)? updates]) =>
      (CustomerListItemResponseBuilder()..update(updates))._build();

  _$CustomerListItemResponse._(
      {this.id,
      this.code,
      this.customerType,
      this.displayNameTh,
      this.displayNameEn,
      this.preferredLocale,
      this.status,
      this.primaryContact,
      this.leadSource})
      : super._();
  @override
  CustomerListItemResponse rebuild(
          void Function(CustomerListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerListItemResponseBuilder toBuilder() =>
      CustomerListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerListItemResponse &&
        id == other.id &&
        code == other.code &&
        customerType == other.customerType &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn &&
        preferredLocale == other.preferredLocale &&
        status == other.status &&
        primaryContact == other.primaryContact &&
        leadSource == other.leadSource;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, customerType.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jc(_$hash, preferredLocale.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, primaryContact.hashCode);
    _$hash = $jc(_$hash, leadSource.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CustomerListItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('customerType', customerType)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn)
          ..add('preferredLocale', preferredLocale)
          ..add('status', status)
          ..add('primaryContact', primaryContact)
          ..add('leadSource', leadSource))
        .toString();
  }
}

class CustomerListItemResponseBuilder
    implements
        Builder<CustomerListItemResponse, CustomerListItemResponseBuilder> {
  _$CustomerListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _customerType;
  String? get customerType => _$this._customerType;
  set customerType(String? customerType) => _$this._customerType = customerType;

  String? _displayNameTh;
  String? get displayNameTh => _$this._displayNameTh;
  set displayNameTh(String? displayNameTh) =>
      _$this._displayNameTh = displayNameTh;

  String? _displayNameEn;
  String? get displayNameEn => _$this._displayNameEn;
  set displayNameEn(String? displayNameEn) =>
      _$this._displayNameEn = displayNameEn;

  String? _preferredLocale;
  String? get preferredLocale => _$this._preferredLocale;
  set preferredLocale(String? preferredLocale) =>
      _$this._preferredLocale = preferredLocale;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  CustomerContactResponseBuilder? _primaryContact;
  CustomerContactResponseBuilder get primaryContact =>
      _$this._primaryContact ??= CustomerContactResponseBuilder();
  set primaryContact(CustomerContactResponseBuilder? primaryContact) =>
      _$this._primaryContact = primaryContact;

  String? _leadSource;
  String? get leadSource => _$this._leadSource;
  set leadSource(String? leadSource) => _$this._leadSource = leadSource;

  CustomerListItemResponseBuilder() {
    CustomerListItemResponse._defaults(this);
  }

  CustomerListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _customerType = $v.customerType;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _preferredLocale = $v.preferredLocale;
      _status = $v.status;
      _primaryContact = $v.primaryContact?.toBuilder();
      _leadSource = $v.leadSource;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerListItemResponse other) {
    _$v = other as _$CustomerListItemResponse;
  }

  @override
  void update(void Function(CustomerListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerListItemResponse build() => _build();

  _$CustomerListItemResponse _build() {
    _$CustomerListItemResponse _$result;
    try {
      _$result = _$v ??
          _$CustomerListItemResponse._(
            id: id,
            code: code,
            customerType: customerType,
            displayNameTh: displayNameTh,
            displayNameEn: displayNameEn,
            preferredLocale: preferredLocale,
            status: status,
            primaryContact: _primaryContact?.build(),
            leadSource: leadSource,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'primaryContact';
        _primaryContact?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CustomerListItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
