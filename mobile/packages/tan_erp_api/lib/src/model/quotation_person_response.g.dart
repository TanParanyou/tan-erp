// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationPersonResponse extends QuotationPersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$QuotationPersonResponse(
          [void Function(QuotationPersonResponseBuilder)? updates]) =>
      (QuotationPersonResponseBuilder()..update(updates))._build();

  _$QuotationPersonResponse._({this.id, this.displayName}) : super._();
  @override
  QuotationPersonResponse rebuild(
          void Function(QuotationPersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationPersonResponseBuilder toBuilder() =>
      QuotationPersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationPersonResponse &&
        id == other.id &&
        displayName == other.displayName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationPersonResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class QuotationPersonResponseBuilder
    implements
        Builder<QuotationPersonResponse, QuotationPersonResponseBuilder> {
  _$QuotationPersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  QuotationPersonResponseBuilder() {
    QuotationPersonResponse._defaults(this);
  }

  QuotationPersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationPersonResponse other) {
    _$v = other as _$QuotationPersonResponse;
  }

  @override
  void update(void Function(QuotationPersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationPersonResponse build() => _build();

  _$QuotationPersonResponse _build() {
    final _$result = _$v ??
        _$QuotationPersonResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
