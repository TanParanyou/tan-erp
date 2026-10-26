// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServicePersonResponse extends ServicePersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$ServicePersonResponse(
          [void Function(ServicePersonResponseBuilder)? updates]) =>
      (ServicePersonResponseBuilder()..update(updates))._build();

  _$ServicePersonResponse._({this.id, this.displayName}) : super._();
  @override
  ServicePersonResponse rebuild(
          void Function(ServicePersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServicePersonResponseBuilder toBuilder() =>
      ServicePersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServicePersonResponse &&
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
    return (newBuiltValueToStringHelper(r'ServicePersonResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class ServicePersonResponseBuilder
    implements Builder<ServicePersonResponse, ServicePersonResponseBuilder> {
  _$ServicePersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  ServicePersonResponseBuilder() {
    ServicePersonResponse._defaults(this);
  }

  ServicePersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServicePersonResponse other) {
    _$v = other as _$ServicePersonResponse;
  }

  @override
  void update(void Function(ServicePersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServicePersonResponse build() => _build();

  _$ServicePersonResponse _build() {
    final _$result = _$v ??
        _$ServicePersonResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
