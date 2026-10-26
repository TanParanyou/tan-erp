// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'inventory_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InventoryPersonResponse extends InventoryPersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;

  factory _$InventoryPersonResponse(
          [void Function(InventoryPersonResponseBuilder)? updates]) =>
      (InventoryPersonResponseBuilder()..update(updates))._build();

  _$InventoryPersonResponse._({this.id, this.displayName, this.email})
      : super._();
  @override
  InventoryPersonResponse rebuild(
          void Function(InventoryPersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InventoryPersonResponseBuilder toBuilder() =>
      InventoryPersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InventoryPersonResponse &&
        id == other.id &&
        displayName == other.displayName &&
        email == other.email;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InventoryPersonResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email))
        .toString();
  }
}

class InventoryPersonResponseBuilder
    implements
        Builder<InventoryPersonResponse, InventoryPersonResponseBuilder> {
  _$InventoryPersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  InventoryPersonResponseBuilder() {
    InventoryPersonResponse._defaults(this);
  }

  InventoryPersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _email = $v.email;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InventoryPersonResponse other) {
    _$v = other as _$InventoryPersonResponse;
  }

  @override
  void update(void Function(InventoryPersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InventoryPersonResponse build() => _build();

  _$InventoryPersonResponse _build() {
    final _$result = _$v ??
        _$InventoryPersonResponse._(
          id: id,
          displayName: displayName,
          email: email,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
