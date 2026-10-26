// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_supplier_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderSupplierResponse extends PurchaseOrderSupplierResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? nameTh;
  @override
  final String? nameEn;

  factory _$PurchaseOrderSupplierResponse(
          [void Function(PurchaseOrderSupplierResponseBuilder)? updates]) =>
      (PurchaseOrderSupplierResponseBuilder()..update(updates))._build();

  _$PurchaseOrderSupplierResponse._(
      {this.id, this.code, this.nameTh, this.nameEn})
      : super._();
  @override
  PurchaseOrderSupplierResponse rebuild(
          void Function(PurchaseOrderSupplierResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderSupplierResponseBuilder toBuilder() =>
      PurchaseOrderSupplierResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderSupplierResponse &&
        id == other.id &&
        code == other.code &&
        nameTh == other.nameTh &&
        nameEn == other.nameEn;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderSupplierResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('nameEn', nameEn))
        .toString();
  }
}

class PurchaseOrderSupplierResponseBuilder
    implements
        Builder<PurchaseOrderSupplierResponse,
            PurchaseOrderSupplierResponseBuilder> {
  _$PurchaseOrderSupplierResponse? _$v;

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

  PurchaseOrderSupplierResponseBuilder() {
    PurchaseOrderSupplierResponse._defaults(this);
  }

  PurchaseOrderSupplierResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _nameTh = $v.nameTh;
      _nameEn = $v.nameEn;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderSupplierResponse other) {
    _$v = other as _$PurchaseOrderSupplierResponse;
  }

  @override
  void update(void Function(PurchaseOrderSupplierResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderSupplierResponse build() => _build();

  _$PurchaseOrderSupplierResponse _build() {
    final _$result = _$v ??
        _$PurchaseOrderSupplierResponse._(
          id: id,
          code: code,
          nameTh: nameTh,
          nameEn: nameEn,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
