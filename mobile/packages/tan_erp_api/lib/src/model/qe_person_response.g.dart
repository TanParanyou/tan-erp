// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'qe_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QePersonResponse extends QePersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$QePersonResponse(
          [void Function(QePersonResponseBuilder)? updates]) =>
      (QePersonResponseBuilder()..update(updates))._build();

  _$QePersonResponse._({this.id, this.displayName}) : super._();
  @override
  QePersonResponse rebuild(void Function(QePersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QePersonResponseBuilder toBuilder() =>
      QePersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QePersonResponse &&
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
    return (newBuiltValueToStringHelper(r'QePersonResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class QePersonResponseBuilder
    implements Builder<QePersonResponse, QePersonResponseBuilder> {
  _$QePersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  QePersonResponseBuilder() {
    QePersonResponse._defaults(this);
  }

  QePersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QePersonResponse other) {
    _$v = other as _$QePersonResponse;
  }

  @override
  void update(void Function(QePersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QePersonResponse build() => _build();

  _$QePersonResponse _build() {
    final _$result = _$v ??
        _$QePersonResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
