// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomRequest extends BomRequest {
  @override
  final String? itemId;
  @override
  final double? outputQuantity;
  @override
  final String? note;
  @override
  final BuiltList<BomLineRequest>? lines;

  factory _$BomRequest([void Function(BomRequestBuilder)? updates]) =>
      (BomRequestBuilder()..update(updates))._build();

  _$BomRequest._({this.itemId, this.outputQuantity, this.note, this.lines})
      : super._();
  @override
  BomRequest rebuild(void Function(BomRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomRequestBuilder toBuilder() => BomRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomRequest &&
        itemId == other.itemId &&
        outputQuantity == other.outputQuantity &&
        note == other.note &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, outputQuantity.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomRequest')
          ..add('itemId', itemId)
          ..add('outputQuantity', outputQuantity)
          ..add('note', note)
          ..add('lines', lines))
        .toString();
  }
}

class BomRequestBuilder implements Builder<BomRequest, BomRequestBuilder> {
  _$BomRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _outputQuantity;
  double? get outputQuantity => _$this._outputQuantity;
  set outputQuantity(double? outputQuantity) =>
      _$this._outputQuantity = outputQuantity;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ListBuilder<BomLineRequest>? _lines;
  ListBuilder<BomLineRequest> get lines =>
      _$this._lines ??= ListBuilder<BomLineRequest>();
  set lines(ListBuilder<BomLineRequest>? lines) => _$this._lines = lines;

  BomRequestBuilder() {
    BomRequest._defaults(this);
  }

  BomRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _outputQuantity = $v.outputQuantity;
      _note = $v.note;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomRequest other) {
    _$v = other as _$BomRequest;
  }

  @override
  void update(void Function(BomRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomRequest build() => _build();

  _$BomRequest _build() {
    _$BomRequest _$result;
    try {
      _$result = _$v ??
          _$BomRequest._(
            itemId: itemId,
            outputQuantity: outputQuantity,
            note: note,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BomRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
