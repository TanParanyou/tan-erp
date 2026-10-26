// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'procurement_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProcurementPersonResponse extends ProcurementPersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;

  factory _$ProcurementPersonResponse(
          [void Function(ProcurementPersonResponseBuilder)? updates]) =>
      (ProcurementPersonResponseBuilder()..update(updates))._build();

  _$ProcurementPersonResponse._({this.id, this.displayName, this.email})
      : super._();
  @override
  ProcurementPersonResponse rebuild(
          void Function(ProcurementPersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProcurementPersonResponseBuilder toBuilder() =>
      ProcurementPersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProcurementPersonResponse &&
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
    return (newBuiltValueToStringHelper(r'ProcurementPersonResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email))
        .toString();
  }
}

class ProcurementPersonResponseBuilder
    implements
        Builder<ProcurementPersonResponse, ProcurementPersonResponseBuilder> {
  _$ProcurementPersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  ProcurementPersonResponseBuilder() {
    ProcurementPersonResponse._defaults(this);
  }

  ProcurementPersonResponseBuilder get _$this {
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
  void replace(ProcurementPersonResponse other) {
    _$v = other as _$ProcurementPersonResponse;
  }

  @override
  void update(void Function(ProcurementPersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProcurementPersonResponse build() => _build();

  _$ProcurementPersonResponse _build() {
    final _$result = _$v ??
        _$ProcurementPersonResponse._(
          id: id,
          displayName: displayName,
          email: email,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
