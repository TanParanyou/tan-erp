// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'assigned_survey_sync_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AssignedSurveySyncListResponse extends AssignedSurveySyncListResponse {
  @override
  final BuiltList<AssignedSurveySyncItemResponse>? items;
  @override
  final DateTime? serverTimeUtc;

  factory _$AssignedSurveySyncListResponse(
          [void Function(AssignedSurveySyncListResponseBuilder)? updates]) =>
      (AssignedSurveySyncListResponseBuilder()..update(updates))._build();

  _$AssignedSurveySyncListResponse._({this.items, this.serverTimeUtc})
      : super._();
  @override
  AssignedSurveySyncListResponse rebuild(
          void Function(AssignedSurveySyncListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AssignedSurveySyncListResponseBuilder toBuilder() =>
      AssignedSurveySyncListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AssignedSurveySyncListResponse &&
        items == other.items &&
        serverTimeUtc == other.serverTimeUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, serverTimeUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AssignedSurveySyncListResponse')
          ..add('items', items)
          ..add('serverTimeUtc', serverTimeUtc))
        .toString();
  }
}

class AssignedSurveySyncListResponseBuilder
    implements
        Builder<AssignedSurveySyncListResponse,
            AssignedSurveySyncListResponseBuilder> {
  _$AssignedSurveySyncListResponse? _$v;

  ListBuilder<AssignedSurveySyncItemResponse>? _items;
  ListBuilder<AssignedSurveySyncItemResponse> get items =>
      _$this._items ??= ListBuilder<AssignedSurveySyncItemResponse>();
  set items(ListBuilder<AssignedSurveySyncItemResponse>? items) =>
      _$this._items = items;

  DateTime? _serverTimeUtc;
  DateTime? get serverTimeUtc => _$this._serverTimeUtc;
  set serverTimeUtc(DateTime? serverTimeUtc) =>
      _$this._serverTimeUtc = serverTimeUtc;

  AssignedSurveySyncListResponseBuilder() {
    AssignedSurveySyncListResponse._defaults(this);
  }

  AssignedSurveySyncListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _serverTimeUtc = $v.serverTimeUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AssignedSurveySyncListResponse other) {
    _$v = other as _$AssignedSurveySyncListResponse;
  }

  @override
  void update(void Function(AssignedSurveySyncListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AssignedSurveySyncListResponse build() => _build();

  _$AssignedSurveySyncListResponse _build() {
    _$AssignedSurveySyncListResponse _$result;
    try {
      _$result = _$v ??
          _$AssignedSurveySyncListResponse._(
            items: _items?.build(),
            serverTimeUtc: serverTimeUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AssignedSurveySyncListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
