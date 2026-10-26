// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpPersonResponse extends MrpPersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;

  factory _$MrpPersonResponse(
          [void Function(MrpPersonResponseBuilder)? updates]) =>
      (MrpPersonResponseBuilder()..update(updates))._build();

  _$MrpPersonResponse._({this.id, this.displayName, this.email}) : super._();
  @override
  MrpPersonResponse rebuild(void Function(MrpPersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpPersonResponseBuilder toBuilder() =>
      MrpPersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpPersonResponse &&
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
    return (newBuiltValueToStringHelper(r'MrpPersonResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email))
        .toString();
  }
}

class MrpPersonResponseBuilder
    implements Builder<MrpPersonResponse, MrpPersonResponseBuilder> {
  _$MrpPersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  MrpPersonResponseBuilder() {
    MrpPersonResponse._defaults(this);
  }

  MrpPersonResponseBuilder get _$this {
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
  void replace(MrpPersonResponse other) {
    _$v = other as _$MrpPersonResponse;
  }

  @override
  void update(void Function(MrpPersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpPersonResponse build() => _build();

  _$MrpPersonResponse _build() {
    final _$result = _$v ??
        _$MrpPersonResponse._(
          id: id,
          displayName: displayName,
          email: email,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
