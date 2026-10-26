// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchResponse extends BranchResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? nameEn;
  @override
  final String? taxBranchCode;
  @override
  final String? addressTh;
  @override
  final String? addressEn;
  @override
  final String? phone;
  @override
  final bool? isActive;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;

  factory _$BranchResponse([void Function(BranchResponseBuilder)? updates]) =>
      (BranchResponseBuilder()..update(updates))._build();

  _$BranchResponse._(
      {this.id,
      this.code,
      this.name,
      this.nameEn,
      this.taxBranchCode,
      this.addressTh,
      this.addressEn,
      this.phone,
      this.isActive,
      this.rowVersion,
      this.createdAtUtc})
      : super._();
  @override
  BranchResponse rebuild(void Function(BranchResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchResponseBuilder toBuilder() => BranchResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        nameEn == other.nameEn &&
        taxBranchCode == other.taxBranchCode &&
        addressTh == other.addressTh &&
        addressEn == other.addressEn &&
        phone == other.phone &&
        isActive == other.isActive &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, taxBranchCode.hashCode);
    _$hash = $jc(_$hash, addressTh.hashCode);
    _$hash = $jc(_$hash, addressEn.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('nameEn', nameEn)
          ..add('taxBranchCode', taxBranchCode)
          ..add('addressTh', addressTh)
          ..add('addressEn', addressEn)
          ..add('phone', phone)
          ..add('isActive', isActive)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class BranchResponseBuilder
    implements Builder<BranchResponse, BranchResponseBuilder> {
  _$BranchResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  String? _taxBranchCode;
  String? get taxBranchCode => _$this._taxBranchCode;
  set taxBranchCode(String? taxBranchCode) =>
      _$this._taxBranchCode = taxBranchCode;

  String? _addressTh;
  String? get addressTh => _$this._addressTh;
  set addressTh(String? addressTh) => _$this._addressTh = addressTh;

  String? _addressEn;
  String? get addressEn => _$this._addressEn;
  set addressEn(String? addressEn) => _$this._addressEn = addressEn;

  String? _phone;
  String? get phone => _$this._phone;
  set phone(String? phone) => _$this._phone = phone;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  BranchResponseBuilder() {
    BranchResponse._defaults(this);
  }

  BranchResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _nameEn = $v.nameEn;
      _taxBranchCode = $v.taxBranchCode;
      _addressTh = $v.addressTh;
      _addressEn = $v.addressEn;
      _phone = $v.phone;
      _isActive = $v.isActive;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchResponse other) {
    _$v = other as _$BranchResponse;
  }

  @override
  void update(void Function(BranchResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchResponse build() => _build();

  _$BranchResponse _build() {
    final _$result = _$v ??
        _$BranchResponse._(
          id: id,
          code: code,
          name: name,
          nameEn: nameEn,
          taxBranchCode: taxBranchCode,
          addressTh: addressTh,
          addressEn: addressEn,
          phone: phone,
          isActive: isActive,
          rowVersion: rowVersion,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
