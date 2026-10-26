// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchDto extends BranchDto {
  @override
  final String? id;
  @override
  final String? name;

  factory _$BranchDto([void Function(BranchDtoBuilder)? updates]) =>
      (BranchDtoBuilder()..update(updates))._build();

  _$BranchDto._({this.id, this.name}) : super._();
  @override
  BranchDto rebuild(void Function(BranchDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchDtoBuilder toBuilder() => BranchDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchDto && id == other.id && name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchDto')
          ..add('id', id)
          ..add('name', name))
        .toString();
  }
}

class BranchDtoBuilder implements Builder<BranchDto, BranchDtoBuilder> {
  _$BranchDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  BranchDtoBuilder() {
    BranchDto._defaults(this);
  }

  BranchDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchDto other) {
    _$v = other as _$BranchDto;
  }

  @override
  void update(void Function(BranchDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchDto build() => _build();

  _$BranchDto _build() {
    final _$result = _$v ??
        _$BranchDto._(
          id: id,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
