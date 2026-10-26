// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_branch_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateBranchRequest extends CreateBranchRequest {
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

  factory _$CreateBranchRequest(
          [void Function(CreateBranchRequestBuilder)? updates]) =>
      (CreateBranchRequestBuilder()..update(updates))._build();

  _$CreateBranchRequest._(
      {this.code,
      this.name,
      this.nameEn,
      this.taxBranchCode,
      this.addressTh,
      this.addressEn,
      this.phone})
      : super._();
  @override
  CreateBranchRequest rebuild(
          void Function(CreateBranchRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateBranchRequestBuilder toBuilder() =>
      CreateBranchRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateBranchRequest &&
        code == other.code &&
        name == other.name &&
        nameEn == other.nameEn &&
        taxBranchCode == other.taxBranchCode &&
        addressTh == other.addressTh &&
        addressEn == other.addressEn &&
        phone == other.phone;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, taxBranchCode.hashCode);
    _$hash = $jc(_$hash, addressTh.hashCode);
    _$hash = $jc(_$hash, addressEn.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateBranchRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('nameEn', nameEn)
          ..add('taxBranchCode', taxBranchCode)
          ..add('addressTh', addressTh)
          ..add('addressEn', addressEn)
          ..add('phone', phone))
        .toString();
  }
}

class CreateBranchRequestBuilder
    implements Builder<CreateBranchRequest, CreateBranchRequestBuilder> {
  _$CreateBranchRequest? _$v;

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

  CreateBranchRequestBuilder() {
    CreateBranchRequest._defaults(this);
  }

  CreateBranchRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _nameEn = $v.nameEn;
      _taxBranchCode = $v.taxBranchCode;
      _addressTh = $v.addressTh;
      _addressEn = $v.addressEn;
      _phone = $v.phone;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateBranchRequest other) {
    _$v = other as _$CreateBranchRequest;
  }

  @override
  void update(void Function(CreateBranchRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateBranchRequest build() => _build();

  _$CreateBranchRequest _build() {
    final _$result = _$v ??
        _$CreateBranchRequest._(
          code: code,
          name: name,
          nameEn: nameEn,
          taxBranchCode: taxBranchCode,
          addressTh: addressTh,
          addressEn: addressEn,
          phone: phone,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
