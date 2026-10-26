// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'api_problem_details.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ApiProblemDetails extends ApiProblemDetails {
  @override
  final String? type;
  @override
  final String? title;
  @override
  final int? status;
  @override
  final String? detail;
  @override
  final String? instance;
  @override
  final String? code;
  @override
  final String? traceId;
  @override
  final BuiltMap<String, BuiltList<String>>? errors;

  factory _$ApiProblemDetails(
          [void Function(ApiProblemDetailsBuilder)? updates]) =>
      (ApiProblemDetailsBuilder()..update(updates))._build();

  _$ApiProblemDetails._(
      {this.type,
      this.title,
      this.status,
      this.detail,
      this.instance,
      this.code,
      this.traceId,
      this.errors})
      : super._();
  @override
  ApiProblemDetails rebuild(void Function(ApiProblemDetailsBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ApiProblemDetailsBuilder toBuilder() =>
      ApiProblemDetailsBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ApiProblemDetails &&
        type == other.type &&
        title == other.title &&
        status == other.status &&
        detail == other.detail &&
        instance == other.instance &&
        code == other.code &&
        traceId == other.traceId &&
        errors == other.errors;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, type.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, detail.hashCode);
    _$hash = $jc(_$hash, instance.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, traceId.hashCode);
    _$hash = $jc(_$hash, errors.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ApiProblemDetails')
          ..add('type', type)
          ..add('title', title)
          ..add('status', status)
          ..add('detail', detail)
          ..add('instance', instance)
          ..add('code', code)
          ..add('traceId', traceId)
          ..add('errors', errors))
        .toString();
  }
}

class ApiProblemDetailsBuilder
    implements Builder<ApiProblemDetails, ApiProblemDetailsBuilder> {
  _$ApiProblemDetails? _$v;

  String? _type;
  String? get type => _$this._type;
  set type(String? type) => _$this._type = type;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  int? _status;
  int? get status => _$this._status;
  set status(int? status) => _$this._status = status;

  String? _detail;
  String? get detail => _$this._detail;
  set detail(String? detail) => _$this._detail = detail;

  String? _instance;
  String? get instance => _$this._instance;
  set instance(String? instance) => _$this._instance = instance;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _traceId;
  String? get traceId => _$this._traceId;
  set traceId(String? traceId) => _$this._traceId = traceId;

  MapBuilder<String, BuiltList<String>>? _errors;
  MapBuilder<String, BuiltList<String>> get errors =>
      _$this._errors ??= MapBuilder<String, BuiltList<String>>();
  set errors(MapBuilder<String, BuiltList<String>>? errors) =>
      _$this._errors = errors;

  ApiProblemDetailsBuilder() {
    ApiProblemDetails._defaults(this);
  }

  ApiProblemDetailsBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _type = $v.type;
      _title = $v.title;
      _status = $v.status;
      _detail = $v.detail;
      _instance = $v.instance;
      _code = $v.code;
      _traceId = $v.traceId;
      _errors = $v.errors?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ApiProblemDetails other) {
    _$v = other as _$ApiProblemDetails;
  }

  @override
  void update(void Function(ApiProblemDetailsBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ApiProblemDetails build() => _build();

  _$ApiProblemDetails _build() {
    _$ApiProblemDetails _$result;
    try {
      _$result = _$v ??
          _$ApiProblemDetails._(
            type: type,
            title: title,
            status: status,
            detail: detail,
            instance: instance,
            code: code,
            traceId: traceId,
            errors: _errors?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'errors';
        _errors?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ApiProblemDetails', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
