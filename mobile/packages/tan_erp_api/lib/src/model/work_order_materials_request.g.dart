// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_materials_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderMaterialsRequest extends WorkOrderMaterialsRequest {
  @override
  final BuiltList<WorkOrderMaterialLineRequest>? lines;

  factory _$WorkOrderMaterialsRequest(
          [void Function(WorkOrderMaterialsRequestBuilder)? updates]) =>
      (WorkOrderMaterialsRequestBuilder()..update(updates))._build();

  _$WorkOrderMaterialsRequest._({this.lines}) : super._();
  @override
  WorkOrderMaterialsRequest rebuild(
          void Function(WorkOrderMaterialsRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderMaterialsRequestBuilder toBuilder() =>
      WorkOrderMaterialsRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderMaterialsRequest && lines == other.lines;
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
    return (newBuiltValueToStringHelper(r'WorkOrderMaterialsRequest')
          ..add('lines', lines))
        .toString();
  }
}

class WorkOrderMaterialsRequestBuilder
    implements
        Builder<WorkOrderMaterialsRequest, WorkOrderMaterialsRequestBuilder> {
  _$WorkOrderMaterialsRequest? _$v;

  ListBuilder<WorkOrderMaterialLineRequest>? _lines;
  ListBuilder<WorkOrderMaterialLineRequest> get lines =>
      _$this._lines ??= ListBuilder<WorkOrderMaterialLineRequest>();
  set lines(ListBuilder<WorkOrderMaterialLineRequest>? lines) =>
      _$this._lines = lines;

  WorkOrderMaterialsRequestBuilder() {
    WorkOrderMaterialsRequest._defaults(this);
  }

  WorkOrderMaterialsRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderMaterialsRequest other) {
    _$v = other as _$WorkOrderMaterialsRequest;
  }

  @override
  void update(void Function(WorkOrderMaterialsRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderMaterialsRequest build() => _build();

  _$WorkOrderMaterialsRequest _build() {
    _$WorkOrderMaterialsRequest _$result;
    try {
      _$result = _$v ??
          _$WorkOrderMaterialsRequest._(
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkOrderMaterialsRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
