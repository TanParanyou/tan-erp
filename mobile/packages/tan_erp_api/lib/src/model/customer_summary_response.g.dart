// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerSummaryResponse extends CustomerSummaryResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;
  @override
  final String? status;

  factory _$CustomerSummaryResponse(
          [void Function(CustomerSummaryResponseBuilder)? updates]) =>
      (CustomerSummaryResponseBuilder()..update(updates))._build();

  _$CustomerSummaryResponse._(
      {this.id, this.code, this.displayNameTh, this.displayNameEn, this.status})
      : super._();
  @override
  CustomerSummaryResponse rebuild(
          void Function(CustomerSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerSummaryResponseBuilder toBuilder() =>
      CustomerSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerSummaryResponse &&
        id == other.id &&
        code == other.code &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn &&
        status == other.status;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CustomerSummaryResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn)
          ..add('status', status))
        .toString();
  }
}

class CustomerSummaryResponseBuilder
    implements
        Builder<CustomerSummaryResponse, CustomerSummaryResponseBuilder> {
  _$CustomerSummaryResponse? _$v;

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

  String? _displayNameEn;
  String? get displayNameEn => _$this._displayNameEn;
  set displayNameEn(String? displayNameEn) =>
      _$this._displayNameEn = displayNameEn;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  CustomerSummaryResponseBuilder() {
    CustomerSummaryResponse._defaults(this);
  }

  CustomerSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _status = $v.status;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerSummaryResponse other) {
    _$v = other as _$CustomerSummaryResponse;
  }

  @override
  void update(void Function(CustomerSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerSummaryResponse build() => _build();

  _$CustomerSummaryResponse _build() {
    final _$result = _$v ??
        _$CustomerSummaryResponse._(
          id: id,
          code: code,
          displayNameTh: displayNameTh,
          displayNameEn: displayNameEn,
          status: status,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
