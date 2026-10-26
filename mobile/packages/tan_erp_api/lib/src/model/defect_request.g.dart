// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'defect_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DefectRequest extends DefectRequest {
  @override
  final String? description;
  @override
  final String? severity;

  factory _$DefectRequest([void Function(DefectRequestBuilder)? updates]) =>
      (DefectRequestBuilder()..update(updates))._build();

  _$DefectRequest._({this.description, this.severity}) : super._();
  @override
  DefectRequest rebuild(void Function(DefectRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DefectRequestBuilder toBuilder() => DefectRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DefectRequest &&
        description == other.description &&
        severity == other.severity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, severity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DefectRequest')
          ..add('description', description)
          ..add('severity', severity))
        .toString();
  }
}

class DefectRequestBuilder
    implements Builder<DefectRequest, DefectRequestBuilder> {
  _$DefectRequest? _$v;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  String? _severity;
  String? get severity => _$this._severity;
  set severity(String? severity) => _$this._severity = severity;

  DefectRequestBuilder() {
    DefectRequest._defaults(this);
  }

  DefectRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _description = $v.description;
      _severity = $v.severity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DefectRequest other) {
    _$v = other as _$DefectRequest;
  }

  @override
  void update(void Function(DefectRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DefectRequest build() => _build();

  _$DefectRequest _build() {
    final _$result = _$v ??
        _$DefectRequest._(
          description: description,
          severity: severity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
