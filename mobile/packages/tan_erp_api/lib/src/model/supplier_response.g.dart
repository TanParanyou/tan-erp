// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierResponse extends SupplierResponse {
  @override
  final String? id;
  @override
  final String? code;
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
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;

  factory _$SupplierResponse(
          [void Function(SupplierResponseBuilder)? updates]) =>
      (SupplierResponseBuilder()..update(updates))._build();

  _$SupplierResponse._(
      {this.id,
      this.code,
      this.nameTh,
      this.nameEn,
      this.taxId,
      this.contactName,
      this.phone,
      this.email,
      this.paymentTermDays,
      this.status,
      this.rowVersion,
      this.createdAtUtc})
      : super._();
  @override
  SupplierResponse rebuild(void Function(SupplierResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierResponseBuilder toBuilder() =>
      SupplierResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierResponse &&
        id == other.id &&
        code == other.code &&
        nameTh == other.nameTh &&
        nameEn == other.nameEn &&
        taxId == other.taxId &&
        contactName == other.contactName &&
        phone == other.phone &&
        email == other.email &&
        paymentTermDays == other.paymentTermDays &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, taxId.hashCode);
    _$hash = $jc(_$hash, contactName.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, paymentTermDays.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('nameEn', nameEn)
          ..add('taxId', taxId)
          ..add('contactName', contactName)
          ..add('phone', phone)
          ..add('email', email)
          ..add('paymentTermDays', paymentTermDays)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class SupplierResponseBuilder
    implements Builder<SupplierResponse, SupplierResponseBuilder> {
  _$SupplierResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

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

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  SupplierResponseBuilder() {
    SupplierResponse._defaults(this);
  }

  SupplierResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _nameTh = $v.nameTh;
      _nameEn = $v.nameEn;
      _taxId = $v.taxId;
      _contactName = $v.contactName;
      _phone = $v.phone;
      _email = $v.email;
      _paymentTermDays = $v.paymentTermDays;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierResponse other) {
    _$v = other as _$SupplierResponse;
  }

  @override
  void update(void Function(SupplierResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierResponse build() => _build();

  _$SupplierResponse _build() {
    final _$result = _$v ??
        _$SupplierResponse._(
          id: id,
          code: code,
          nameTh: nameTh,
          nameEn: nameEn,
          taxId: taxId,
          contactName: contactName,
          phone: phone,
          email: email,
          paymentTermDays: paymentTermDays,
          status: status,
          rowVersion: rowVersion,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
