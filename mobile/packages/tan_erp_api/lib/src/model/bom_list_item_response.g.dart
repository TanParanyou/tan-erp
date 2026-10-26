// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomListItemResponse extends BomListItemResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final ProductionItemResponse? item;
  @override
  final int? approvedRevisionNo;
  @override
  final int? latestRevisionNo;
  @override
  final String? latestStatus;
  @override
  final DateTime? createdAtUtc;

  factory _$BomListItemResponse(
          [void Function(BomListItemResponseBuilder)? updates]) =>
      (BomListItemResponseBuilder()..update(updates))._build();

  _$BomListItemResponse._(
      {this.id,
      this.code,
      this.item,
      this.approvedRevisionNo,
      this.latestRevisionNo,
      this.latestStatus,
      this.createdAtUtc})
      : super._();
  @override
  BomListItemResponse rebuild(
          void Function(BomListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomListItemResponseBuilder toBuilder() =>
      BomListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomListItemResponse &&
        id == other.id &&
        code == other.code &&
        item == other.item &&
        approvedRevisionNo == other.approvedRevisionNo &&
        latestRevisionNo == other.latestRevisionNo &&
        latestStatus == other.latestStatus &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, approvedRevisionNo.hashCode);
    _$hash = $jc(_$hash, latestRevisionNo.hashCode);
    _$hash = $jc(_$hash, latestStatus.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomListItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('item', item)
          ..add('approvedRevisionNo', approvedRevisionNo)
          ..add('latestRevisionNo', latestRevisionNo)
          ..add('latestStatus', latestStatus)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class BomListItemResponseBuilder
    implements Builder<BomListItemResponse, BomListItemResponseBuilder> {
  _$BomListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  ProductionItemResponseBuilder? _item;
  ProductionItemResponseBuilder get item =>
      _$this._item ??= ProductionItemResponseBuilder();
  set item(ProductionItemResponseBuilder? item) => _$this._item = item;

  int? _approvedRevisionNo;
  int? get approvedRevisionNo => _$this._approvedRevisionNo;
  set approvedRevisionNo(int? approvedRevisionNo) =>
      _$this._approvedRevisionNo = approvedRevisionNo;

  int? _latestRevisionNo;
  int? get latestRevisionNo => _$this._latestRevisionNo;
  set latestRevisionNo(int? latestRevisionNo) =>
      _$this._latestRevisionNo = latestRevisionNo;

  String? _latestStatus;
  String? get latestStatus => _$this._latestStatus;
  set latestStatus(String? latestStatus) => _$this._latestStatus = latestStatus;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  BomListItemResponseBuilder() {
    BomListItemResponse._defaults(this);
  }

  BomListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _item = $v.item?.toBuilder();
      _approvedRevisionNo = $v.approvedRevisionNo;
      _latestRevisionNo = $v.latestRevisionNo;
      _latestStatus = $v.latestStatus;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomListItemResponse other) {
    _$v = other as _$BomListItemResponse;
  }

  @override
  void update(void Function(BomListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomListItemResponse build() => _build();

  _$BomListItemResponse _build() {
    _$BomListItemResponse _$result;
    try {
      _$result = _$v ??
          _$BomListItemResponse._(
            id: id,
            code: code,
            item: _item?.build(),
            approvedRevisionNo: approvedRevisionNo,
            latestRevisionNo: latestRevisionNo,
            latestStatus: latestStatus,
            createdAtUtc: createdAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BomListItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
