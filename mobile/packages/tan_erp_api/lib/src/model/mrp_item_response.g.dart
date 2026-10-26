// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpItemResponse extends MrpItemResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? nameTh;
  @override
  final String? unitCode;

  factory _$MrpItemResponse([void Function(MrpItemResponseBuilder)? updates]) =>
      (MrpItemResponseBuilder()..update(updates))._build();

  _$MrpItemResponse._({this.id, this.code, this.nameTh, this.unitCode})
      : super._();
  @override
  MrpItemResponse rebuild(void Function(MrpItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpItemResponseBuilder toBuilder() => MrpItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpItemResponse &&
        id == other.id &&
        code == other.code &&
        nameTh == other.nameTh &&
        unitCode == other.unitCode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('unitCode', unitCode))
        .toString();
  }
}

class MrpItemResponseBuilder
    implements Builder<MrpItemResponse, MrpItemResponseBuilder> {
  _$MrpItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _nameTh;
  String? get nameTh => _$this._nameTh;
  set nameTh(String? nameTh) => _$this._nameTh = nameTh;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  MrpItemResponseBuilder() {
    MrpItemResponse._defaults(this);
  }

  MrpItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _nameTh = $v.nameTh;
      _unitCode = $v.unitCode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpItemResponse other) {
    _$v = other as _$MrpItemResponse;
  }

  @override
  void update(void Function(MrpItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpItemResponse build() => _build();

  _$MrpItemResponse _build() {
    final _$result = _$v ??
        _$MrpItemResponse._(
          id: id,
          code: code,
          nameTh: nameTh,
          unitCode: unitCode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
