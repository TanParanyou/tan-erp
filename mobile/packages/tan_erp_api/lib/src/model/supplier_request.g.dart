// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierRequest extends SupplierRequest {
  @override
  final String? nameTh;
  @override
  final String? nameEn;
  @override
  final String? taxId;
  @override
  final String? contactName;
  @override
  final String? phone;
  @override
  final String? email;
  @override
  final int? paymentTermDays;

  factory _$SupplierRequest([void Function(SupplierRequestBuilder)? updates]) =>
      (SupplierRequestBuilder()..update(updates))._build();

  _$SupplierRequest._(
      {this.nameTh,
      this.nameEn,
      this.taxId,
      this.contactName,
      this.phone,
      this.email,
      this.paymentTermDays})
      : super._();
  @override
  SupplierRequest rebuild(void Function(SupplierRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierRequestBuilder toBuilder() => SupplierRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierRequest &&
        nameTh == other.nameTh &&
        nameEn == other.nameEn &&
        taxId == other.taxId &&
        contactName == other.contactName &&
        phone == other.phone &&
        email == other.email &&
        paymentTermDays == other.paymentTermDays;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, taxId.hashCode);
    _$hash = $jc(_$hash, contactName.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, paymentTermDays.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierRequest')
          ..add('nameTh', nameTh)
          ..add('nameEn', nameEn)
          ..add('taxId', taxId)
          ..add('contactName', contactName)
          ..add('phone', phone)
          ..add('email', email)
          ..add('paymentTermDays', paymentTermDays))
        .toString();
  }
}

class SupplierRequestBuilder
    implements Builder<SupplierRequest, SupplierRequestBuilder> {
  _$SupplierRequest? _$v;

  String? _nameTh;
  String? get nameTh => _$this._nameTh;
  set nameTh(String? nameTh) => _$this._nameTh = nameTh;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  String? _taxId;
  String? get taxId => _$this._taxId;
  set taxId(String? taxId) => _$this._taxId = taxId;

  String? _contactName;
  String? get contactName => _$this._contactName;
  set contactName(String? contactName) => _$this._contactName = contactName;

  String? _phone;
  String? get phone => _$this._phone;
  set phone(String? phone) => _$this._phone = phone;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  int? _paymentTermDays;
  int? get paymentTermDays => _$this._paymentTermDays;
  set paymentTermDays(int? paymentTermDays) =>
      _$this._paymentTermDays = paymentTermDays;

  SupplierRequestBuilder() {
    SupplierRequest._defaults(this);
  }

  SupplierRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _nameTh = $v.nameTh;
      _nameEn = $v.nameEn;
      _taxId = $v.taxId;
      _contactName = $v.contactName;
      _phone = $v.phone;
      _email = $v.email;
      _paymentTermDays = $v.paymentTermDays;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierRequest other) {
    _$v = other as _$SupplierRequest;
  }

  @override
  void update(void Function(SupplierRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierRequest build() => _build();

  _$SupplierRequest _build() {
    final _$result = _$v ??
        _$SupplierRequest._(
          nameTh: nameTh,
          nameEn: nameEn,
          taxId: taxId,
          contactName: contactName,
          phone: phone,
          email: email,
          paymentTermDays: paymentTermDays,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
