// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quick_estimate_template_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuickEstimateTemplateRefResponse
    extends QuickEstimateTemplateRefResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final int? version;
  @override
  final String? name;
  @override
  final String? status;

  factory _$QuickEstimateTemplateRefResponse(
          [void Function(QuickEstimateTemplateRefResponseBuilder)? updates]) =>
      (QuickEstimateTemplateRefResponseBuilder()..update(updates))._build();

  _$QuickEstimateTemplateRefResponse._(
      {this.id, this.code, this.version, this.name, this.status})
      : super._();
  @override
  QuickEstimateTemplateRefResponse rebuild(
          void Function(QuickEstimateTemplateRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuickEstimateTemplateRefResponseBuilder toBuilder() =>
      QuickEstimateTemplateRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuickEstimateTemplateRefResponse &&
        id == other.id &&
        code == other.code &&
        version == other.version &&
        name == other.name &&
        status == other.status;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuickEstimateTemplateRefResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('version', version)
          ..add('name', name)
          ..add('status', status))
        .toString();
  }
}

class QuickEstimateTemplateRefResponseBuilder
    implements
        Builder<QuickEstimateTemplateRefResponse,
            QuickEstimateTemplateRefResponseBuilder> {
  _$QuickEstimateTemplateRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  QuickEstimateTemplateRefResponseBuilder() {
    QuickEstimateTemplateRefResponse._defaults(this);
  }

  QuickEstimateTemplateRefResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _version = $v.version;
      _name = $v.name;
      _status = $v.status;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuickEstimateTemplateRefResponse other) {
    _$v = other as _$QuickEstimateTemplateRefResponse;
  }

  @override
  void update(void Function(QuickEstimateTemplateRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuickEstimateTemplateRefResponse build() => _build();

  _$QuickEstimateTemplateRefResponse _build() {
    final _$result = _$v ??
        _$QuickEstimateTemplateRefResponse._(
          id: id,
          code: code,
          version: version,
          name: name,
          status: status,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
