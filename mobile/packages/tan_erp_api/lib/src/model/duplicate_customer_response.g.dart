// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'duplicate_customer_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DuplicateCustomerResponse extends DuplicateCustomerResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? displayNameTh;
  @override
  final String? maskedPhone;
  @override
  final String? maskedEmail;

  factory _$DuplicateCustomerResponse(
          [void Function(DuplicateCustomerResponseBuilder)? updates]) =>
      (DuplicateCustomerResponseBuilder()..update(updates))._build();

  _$DuplicateCustomerResponse._(
      {this.id,
      this.code,
      this.displayNameTh,
      this.maskedPhone,
      this.maskedEmail})
      : super._();
  @override
  DuplicateCustomerResponse rebuild(
          void Function(DuplicateCustomerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DuplicateCustomerResponseBuilder toBuilder() =>
      DuplicateCustomerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DuplicateCustomerResponse &&
        id == other.id &&
        code == other.code &&
        displayNameTh == other.displayNameTh &&
        maskedPhone == other.maskedPhone &&
        maskedEmail == other.maskedEmail;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, maskedPhone.hashCode);
    _$hash = $jc(_$hash, maskedEmail.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DuplicateCustomerResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('displayNameTh', displayNameTh)
          ..add('maskedPhone', maskedPhone)
          ..add('maskedEmail', maskedEmail))
        .toString();
  }
}

class DuplicateCustomerResponseBuilder
    implements
        Builder<DuplicateCustomerResponse, DuplicateCustomerResponseBuilder> {
  _$DuplicateCustomerResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _displayNameTh;
  String? get displayNameTh => _$this._displayNameTh;
  set displayNameTh(String? displayNameTh) =>
      _$this._displayNameTh = displayNameTh;

  String? _maskedPhone;
  String? get maskedPhone => _$this._maskedPhone;
  set maskedPhone(String? maskedPhone) => _$this._maskedPhone = maskedPhone;

  String? _maskedEmail;
  String? get maskedEmail => _$this._maskedEmail;
  set maskedEmail(String? maskedEmail) => _$this._maskedEmail = maskedEmail;

  DuplicateCustomerResponseBuilder() {
    DuplicateCustomerResponse._defaults(this);
  }

  DuplicateCustomerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _displayNameTh = $v.displayNameTh;
      _maskedPhone = $v.maskedPhone;
      _maskedEmail = $v.maskedEmail;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DuplicateCustomerResponse other) {
    _$v = other as _$DuplicateCustomerResponse;
  }

  @override
  void update(void Function(DuplicateCustomerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DuplicateCustomerResponse build() => _build();

  _$DuplicateCustomerResponse _build() {
    final _$result = _$v ??
        _$DuplicateCustomerResponse._(
          id: id,
          code: code,
          displayNameTh: displayNameTh,
          maskedPhone: maskedPhone,
          maskedEmail: maskedEmail,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
