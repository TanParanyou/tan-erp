// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pricing_template_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PricingTemplateListItemResponse
    extends PricingTemplateListItemResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final int? version;
  @override
  final String? workType;
  @override
  final String? name;
  @override
  final String? status;
  @override
  final DateTime? updatedAtUtc;

  factory _$PricingTemplateListItemResponse(
          [void Function(PricingTemplateListItemResponseBuilder)? updates]) =>
      (PricingTemplateListItemResponseBuilder()..update(updates))._build();

  _$PricingTemplateListItemResponse._(
      {this.id,
      this.code,
      this.version,
      this.workType,
      this.name,
      this.status,
      this.updatedAtUtc})
      : super._();
  @override
  PricingTemplateListItemResponse rebuild(
          void Function(PricingTemplateListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PricingTemplateListItemResponseBuilder toBuilder() =>
      PricingTemplateListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PricingTemplateListItemResponse &&
        id == other.id &&
        code == other.code &&
        version == other.version &&
        workType == other.workType &&
        name == other.name &&
        status == other.status &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, workType.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PricingTemplateListItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('version', version)
          ..add('workType', workType)
          ..add('name', name)
          ..add('status', status)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class PricingTemplateListItemResponseBuilder
    implements
        Builder<PricingTemplateListItemResponse,
            PricingTemplateListItemResponseBuilder> {
  _$PricingTemplateListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _workType;
  String? get workType => _$this._workType;
  set workType(String? workType) => _$this._workType = workType;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  PricingTemplateListItemResponseBuilder() {
    PricingTemplateListItemResponse._defaults(this);
  }

  PricingTemplateListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _version = $v.version;
      _workType = $v.workType;
      _name = $v.name;
      _status = $v.status;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PricingTemplateListItemResponse other) {
    _$v = other as _$PricingTemplateListItemResponse;
  }

  @override
  void update(void Function(PricingTemplateListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PricingTemplateListItemResponse build() => _build();

  _$PricingTemplateListItemResponse _build() {
    final _$result = _$v ??
        _$PricingTemplateListItemResponse._(
          id: id,
          code: code,
          version: version,
          workType: workType,
          name: name,
          status: status,
          updatedAtUtc: updatedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
