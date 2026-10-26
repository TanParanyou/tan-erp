// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_work_item_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateWorkItemItemResponse extends EstimateWorkItemItemResponse {
  @override
  final String id;
  @override
  final String code;
  @override
  final String nameTh;
  @override
  final String? nameEn;

  factory _$EstimateWorkItemItemResponse(
          [void Function(EstimateWorkItemItemResponseBuilder)? updates]) =>
      (EstimateWorkItemItemResponseBuilder()..update(updates))._build();

  _$EstimateWorkItemItemResponse._(
      {required this.id, required this.code, required this.nameTh, this.nameEn})
      : super._();
  @override
  EstimateWorkItemItemResponse rebuild(
          void Function(EstimateWorkItemItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateWorkItemItemResponseBuilder toBuilder() =>
      EstimateWorkItemItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateWorkItemItemResponse &&
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
    return (newBuiltValueToStringHelper(r'EstimateWorkItemItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('nameEn', nameEn))
        .toString();
  }
}

class EstimateWorkItemItemResponseBuilder
    implements
        Builder<EstimateWorkItemItemResponse,
            EstimateWorkItemItemResponseBuilder> {
  _$EstimateWorkItemItemResponse? _$v;

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

  EstimateWorkItemItemResponseBuilder() {
    EstimateWorkItemItemResponse._defaults(this);
  }

  EstimateWorkItemItemResponseBuilder get _$this {
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
  void replace(EstimateWorkItemItemResponse other) {
    _$v = other as _$EstimateWorkItemItemResponse;
  }

  @override
  void update(void Function(EstimateWorkItemItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateWorkItemItemResponse build() => _build();

  _$EstimateWorkItemItemResponse _build() {
    final _$result = _$v ??
        _$EstimateWorkItemItemResponse._(
          id: BuiltValueNullFieldError.checkNotNull(
              id, r'EstimateWorkItemItemResponse', 'id'),
          code: BuiltValueNullFieldError.checkNotNull(
              code, r'EstimateWorkItemItemResponse', 'code'),
          nameTh: BuiltValueNullFieldError.checkNotNull(
              nameTh, r'EstimateWorkItemItemResponse', 'nameTh'),
          nameEn: nameEn,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
