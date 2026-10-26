// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_converted_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpConvertedResponse extends MrpConvertedResponse {
  @override
  final String? type;
  @override
  final String? id;
  @override
  final String? number;

  factory _$MrpConvertedResponse(
          [void Function(MrpConvertedResponseBuilder)? updates]) =>
      (MrpConvertedResponseBuilder()..update(updates))._build();

  _$MrpConvertedResponse._({this.type, this.id, this.number}) : super._();
  @override
  MrpConvertedResponse rebuild(
          void Function(MrpConvertedResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpConvertedResponseBuilder toBuilder() =>
      MrpConvertedResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpConvertedResponse &&
        type == other.type &&
        id == other.id &&
        number == other.number;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, type.hashCode);
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpConvertedResponse')
          ..add('type', type)
          ..add('id', id)
          ..add('number', number))
        .toString();
  }
}

class MrpConvertedResponseBuilder
    implements Builder<MrpConvertedResponse, MrpConvertedResponseBuilder> {
  _$MrpConvertedResponse? _$v;

  String? _type;
  String? get type => _$this._type;
  set type(String? type) => _$this._type = type;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  MrpConvertedResponseBuilder() {
    MrpConvertedResponse._defaults(this);
  }

  MrpConvertedResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _type = $v.type;
      _id = $v.id;
      _number = $v.number;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpConvertedResponse other) {
    _$v = other as _$MrpConvertedResponse;
  }

  @override
  void update(void Function(MrpConvertedResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpConvertedResponse build() => _build();

  _$MrpConvertedResponse _build() {
    final _$result = _$v ??
        _$MrpConvertedResponse._(
          type: type,
          id: id,
          number: number,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
