// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomResponse extends BomResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final ProductionItemResponse? item;
  @override
  final DateTime? createdAtUtc;
  @override
  final BuiltList<BomRevisionResponse>? revisions;

  factory _$BomResponse([void Function(BomResponseBuilder)? updates]) =>
      (BomResponseBuilder()..update(updates))._build();

  _$BomResponse._(
      {this.id, this.code, this.item, this.createdAtUtc, this.revisions})
      : super._();
  @override
  BomResponse rebuild(void Function(BomResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomResponseBuilder toBuilder() => BomResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomResponse &&
        id == other.id &&
        code == other.code &&
        item == other.item &&
        createdAtUtc == other.createdAtUtc &&
        revisions == other.revisions;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, revisions.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('item', item)
          ..add('createdAtUtc', createdAtUtc)
          ..add('revisions', revisions))
        .toString();
  }
}

class BomResponseBuilder implements Builder<BomResponse, BomResponseBuilder> {
  _$BomResponse? _$v;

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

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ListBuilder<BomRevisionResponse>? _revisions;
  ListBuilder<BomRevisionResponse> get revisions =>
      _$this._revisions ??= ListBuilder<BomRevisionResponse>();
  set revisions(ListBuilder<BomRevisionResponse>? revisions) =>
      _$this._revisions = revisions;

  BomResponseBuilder() {
    BomResponse._defaults(this);
  }

  BomResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _item = $v.item?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _revisions = $v.revisions?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomResponse other) {
    _$v = other as _$BomResponse;
  }

  @override
  void update(void Function(BomResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomResponse build() => _build();

  _$BomResponse _build() {
    _$BomResponse _$result;
    try {
      _$result = _$v ??
          _$BomResponse._(
            id: id,
            code: code,
            item: _item?.build(),
            createdAtUtc: createdAtUtc,
            revisions: _revisions?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();

        _$failedField = 'revisions';
        _revisions?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BomResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
