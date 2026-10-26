// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_image_user_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkImageUserSummaryResponse extends WorkImageUserSummaryResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$WorkImageUserSummaryResponse(
          [void Function(WorkImageUserSummaryResponseBuilder)? updates]) =>
      (WorkImageUserSummaryResponseBuilder()..update(updates))._build();

  _$WorkImageUserSummaryResponse._({this.id, this.displayName}) : super._();
  @override
  WorkImageUserSummaryResponse rebuild(
          void Function(WorkImageUserSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkImageUserSummaryResponseBuilder toBuilder() =>
      WorkImageUserSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkImageUserSummaryResponse &&
        id == other.id &&
        displayName == other.displayName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkImageUserSummaryResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class WorkImageUserSummaryResponseBuilder
    implements
        Builder<WorkImageUserSummaryResponse,
            WorkImageUserSummaryResponseBuilder> {
  _$WorkImageUserSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  WorkImageUserSummaryResponseBuilder() {
    WorkImageUserSummaryResponse._defaults(this);
  }

  WorkImageUserSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkImageUserSummaryResponse other) {
    _$v = other as _$WorkImageUserSummaryResponse;
  }

  @override
  void update(void Function(WorkImageUserSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkImageUserSummaryResponse build() => _build();

  _$WorkImageUserSummaryResponse _build() {
    final _$result = _$v ??
        _$WorkImageUserSummaryResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
