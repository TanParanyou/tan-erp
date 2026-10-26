// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cost_source_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CostSourceResponse extends CostSourceResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final String? sourceType;
  @override
  final int? priority;
  @override
  final bool? isActive;
  @override
  final String? rowVersion;

  factory _$CostSourceResponse(
          [void Function(CostSourceResponseBuilder)? updates]) =>
      (CostSourceResponseBuilder()..update(updates))._build();

  _$CostSourceResponse._(
      {this.id,
      this.code,
      this.name,
      this.sourceType,
      this.priority,
      this.isActive,
      this.rowVersion})
      : super._();
  @override
  CostSourceResponse rebuild(
          void Function(CostSourceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CostSourceResponseBuilder toBuilder() =>
      CostSourceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CostSourceResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        sourceType == other.sourceType &&
        priority == other.priority &&
        isActive == other.isActive &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, sourceType.hashCode);
    _$hash = $jc(_$hash, priority.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CostSourceResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('sourceType', sourceType)
          ..add('priority', priority)
          ..add('isActive', isActive)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class CostSourceResponseBuilder
    implements Builder<CostSourceResponse, CostSourceResponseBuilder> {
  _$CostSourceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  String? _sourceType;
  String? get sourceType => _$this._sourceType;
  set sourceType(String? sourceType) => _$this._sourceType = sourceType;

  int? _priority;
  int? get priority => _$this._priority;
  set priority(int? priority) => _$this._priority = priority;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  CostSourceResponseBuilder() {
    CostSourceResponse._defaults(this);
  }

  CostSourceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _sourceType = $v.sourceType;
      _priority = $v.priority;
      _isActive = $v.isActive;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CostSourceResponse other) {
    _$v = other as _$CostSourceResponse;
  }

  @override
  void update(void Function(CostSourceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CostSourceResponse build() => _build();

  _$CostSourceResponse _build() {
    _$CostSourceResponse _$result;
    try {
      _$result = _$v ??
          _$CostSourceResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            sourceType: sourceType,
            priority: priority,
            isActive: isActive,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CostSourceResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
