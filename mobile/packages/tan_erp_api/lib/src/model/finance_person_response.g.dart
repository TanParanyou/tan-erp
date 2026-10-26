// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'finance_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$FinancePersonResponse extends FinancePersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$FinancePersonResponse(
          [void Function(FinancePersonResponseBuilder)? updates]) =>
      (FinancePersonResponseBuilder()..update(updates))._build();

  _$FinancePersonResponse._({this.id, this.displayName}) : super._();
  @override
  FinancePersonResponse rebuild(
          void Function(FinancePersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  FinancePersonResponseBuilder toBuilder() =>
      FinancePersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is FinancePersonResponse &&
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
    return (newBuiltValueToStringHelper(r'FinancePersonResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class FinancePersonResponseBuilder
    implements Builder<FinancePersonResponse, FinancePersonResponseBuilder> {
  _$FinancePersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  FinancePersonResponseBuilder() {
    FinancePersonResponse._defaults(this);
  }

  FinancePersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(FinancePersonResponse other) {
    _$v = other as _$FinancePersonResponse;
  }

  @override
  void update(void Function(FinancePersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  FinancePersonResponse build() => _build();

  _$FinancePersonResponse _build() {
    final _$result = _$v ??
        _$FinancePersonResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
