// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'replace_project_budget_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReplaceProjectBudgetRequest extends ReplaceProjectBudgetRequest {
  @override
  final BuiltList<ProjectBudgetLineRequest>? lines;

  factory _$ReplaceProjectBudgetRequest(
          [void Function(ReplaceProjectBudgetRequestBuilder)? updates]) =>
      (ReplaceProjectBudgetRequestBuilder()..update(updates))._build();

  _$ReplaceProjectBudgetRequest._({this.lines}) : super._();
  @override
  ReplaceProjectBudgetRequest rebuild(
          void Function(ReplaceProjectBudgetRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReplaceProjectBudgetRequestBuilder toBuilder() =>
      ReplaceProjectBudgetRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReplaceProjectBudgetRequest && lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReplaceProjectBudgetRequest')
          ..add('lines', lines))
        .toString();
  }
}

class ReplaceProjectBudgetRequestBuilder
    implements
        Builder<ReplaceProjectBudgetRequest,
            ReplaceProjectBudgetRequestBuilder> {
  _$ReplaceProjectBudgetRequest? _$v;

  ListBuilder<ProjectBudgetLineRequest>? _lines;
  ListBuilder<ProjectBudgetLineRequest> get lines =>
      _$this._lines ??= ListBuilder<ProjectBudgetLineRequest>();
  set lines(ListBuilder<ProjectBudgetLineRequest>? lines) =>
      _$this._lines = lines;

  ReplaceProjectBudgetRequestBuilder() {
    ReplaceProjectBudgetRequest._defaults(this);
  }

  ReplaceProjectBudgetRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReplaceProjectBudgetRequest other) {
    _$v = other as _$ReplaceProjectBudgetRequest;
  }

  @override
  void update(void Function(ReplaceProjectBudgetRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReplaceProjectBudgetRequest build() => _build();

  _$ReplaceProjectBudgetRequest _build() {
    _$ReplaceProjectBudgetRequest _$result;
    try {
      _$result = _$v ??
          _$ReplaceProjectBudgetRequest._(
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ReplaceProjectBudgetRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
