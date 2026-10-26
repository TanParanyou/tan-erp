// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_customer_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectCustomerResponse extends ProjectCustomerResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;

  factory _$ProjectCustomerResponse(
          [void Function(ProjectCustomerResponseBuilder)? updates]) =>
      (ProjectCustomerResponseBuilder()..update(updates))._build();

  _$ProjectCustomerResponse._(
      {this.id, this.code, this.displayNameTh, this.displayNameEn})
      : super._();
  @override
  ProjectCustomerResponse rebuild(
          void Function(ProjectCustomerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectCustomerResponseBuilder toBuilder() =>
      ProjectCustomerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectCustomerResponse &&
        id == other.id &&
        code == other.code &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectCustomerResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn))
        .toString();
  }
}

class ProjectCustomerResponseBuilder
    implements
        Builder<ProjectCustomerResponse, ProjectCustomerResponseBuilder> {
  _$ProjectCustomerResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _displayNameTh;
  String? get displayNameTh => _$this._displayNameTh;
  set displayNameTh(String? displayNameTh) =>
      _$this._displayNameTh = displayNameTh;

  String? _displayNameEn;
  String? get displayNameEn => _$this._displayNameEn;
  set displayNameEn(String? displayNameEn) =>
      _$this._displayNameEn = displayNameEn;

  ProjectCustomerResponseBuilder() {
    ProjectCustomerResponse._defaults(this);
  }

  ProjectCustomerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectCustomerResponse other) {
    _$v = other as _$ProjectCustomerResponse;
  }

  @override
  void update(void Function(ProjectCustomerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectCustomerResponse build() => _build();

  _$ProjectCustomerResponse _build() {
    final _$result = _$v ??
        _$ProjectCustomerResponse._(
          id: id,
          code: code,
          displayNameTh: displayNameTh,
          displayNameEn: displayNameEn,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
