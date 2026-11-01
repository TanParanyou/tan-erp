// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'technician_schedules_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TechnicianSchedulesListResponse
    extends TechnicianSchedulesListResponse {
  @override
  final BuiltList<TechnicianAssignmentResponse>? items;

  factory _$TechnicianSchedulesListResponse(
          [void Function(TechnicianSchedulesListResponseBuilder)? updates]) =>
      (TechnicianSchedulesListResponseBuilder()..update(updates))._build();

  _$TechnicianSchedulesListResponse._({this.items}) : super._();
  @override
  TechnicianSchedulesListResponse rebuild(
          void Function(TechnicianSchedulesListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TechnicianSchedulesListResponseBuilder toBuilder() =>
      TechnicianSchedulesListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TechnicianSchedulesListResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TechnicianSchedulesListResponse')
          ..add('items', items))
        .toString();
  }
}

class TechnicianSchedulesListResponseBuilder
    implements
        Builder<TechnicianSchedulesListResponse,
            TechnicianSchedulesListResponseBuilder> {
  _$TechnicianSchedulesListResponse? _$v;

  ListBuilder<TechnicianAssignmentResponse>? _items;
  ListBuilder<TechnicianAssignmentResponse> get items =>
      _$this._items ??= ListBuilder<TechnicianAssignmentResponse>();
  set items(ListBuilder<TechnicianAssignmentResponse>? items) =>
      _$this._items = items;

  TechnicianSchedulesListResponseBuilder() {
    TechnicianSchedulesListResponse._defaults(this);
  }

  TechnicianSchedulesListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TechnicianSchedulesListResponse other) {
    _$v = other as _$TechnicianSchedulesListResponse;
  }

  @override
  void update(void Function(TechnicianSchedulesListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TechnicianSchedulesListResponse build() => _build();

  _$TechnicianSchedulesListResponse _build() {
    _$TechnicianSchedulesListResponse _$result;
    try {
      _$result = _$v ??
          _$TechnicianSchedulesListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'TechnicianSchedulesListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
