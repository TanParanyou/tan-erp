// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_revision_lines_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateRevisionLinesResponse extends EstimateRevisionLinesResponse {
  @override
  final BuiltList<EstimateRevisionLineResponse> lines;

  factory _$EstimateRevisionLinesResponse(
          [void Function(EstimateRevisionLinesResponseBuilder)? updates]) =>
      (EstimateRevisionLinesResponseBuilder()..update(updates))._build();

  _$EstimateRevisionLinesResponse._({required this.lines}) : super._();
  @override
  EstimateRevisionLinesResponse rebuild(
          void Function(EstimateRevisionLinesResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateRevisionLinesResponseBuilder toBuilder() =>
      EstimateRevisionLinesResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateRevisionLinesResponse && lines == other.lines;
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
    return (newBuiltValueToStringHelper(r'EstimateRevisionLinesResponse')
          ..add('lines', lines))
        .toString();
  }
}

class EstimateRevisionLinesResponseBuilder
    implements
        Builder<EstimateRevisionLinesResponse,
            EstimateRevisionLinesResponseBuilder> {
  _$EstimateRevisionLinesResponse? _$v;

  ListBuilder<EstimateRevisionLineResponse>? _lines;
  ListBuilder<EstimateRevisionLineResponse> get lines =>
      _$this._lines ??= ListBuilder<EstimateRevisionLineResponse>();
  set lines(ListBuilder<EstimateRevisionLineResponse>? lines) =>
      _$this._lines = lines;

  EstimateRevisionLinesResponseBuilder() {
    EstimateRevisionLinesResponse._defaults(this);
  }

  EstimateRevisionLinesResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _lines = $v.lines.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateRevisionLinesResponse other) {
    _$v = other as _$EstimateRevisionLinesResponse;
  }

  @override
  void update(void Function(EstimateRevisionLinesResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateRevisionLinesResponse build() => _build();

  _$EstimateRevisionLinesResponse _build() {
    _$EstimateRevisionLinesResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateRevisionLinesResponse._(
            lines: lines.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        lines.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateRevisionLinesResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
