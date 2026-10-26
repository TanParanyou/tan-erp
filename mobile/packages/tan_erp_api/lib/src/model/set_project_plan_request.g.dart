// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'set_project_plan_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SetProjectPlanRequest extends SetProjectPlanRequest {
  @override
  final Date? plannedStartDate;
  @override
  final Date? plannedEndDate;

  factory _$SetProjectPlanRequest(
          [void Function(SetProjectPlanRequestBuilder)? updates]) =>
      (SetProjectPlanRequestBuilder()..update(updates))._build();

  _$SetProjectPlanRequest._({this.plannedStartDate, this.plannedEndDate})
      : super._();
  @override
  SetProjectPlanRequest rebuild(
          void Function(SetProjectPlanRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SetProjectPlanRequestBuilder toBuilder() =>
      SetProjectPlanRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SetProjectPlanRequest &&
        plannedStartDate == other.plannedStartDate &&
        plannedEndDate == other.plannedEndDate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, plannedStartDate.hashCode);
    _$hash = $jc(_$hash, plannedEndDate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SetProjectPlanRequest')
          ..add('plannedStartDate', plannedStartDate)
          ..add('plannedEndDate', plannedEndDate))
        .toString();
  }
}

class SetProjectPlanRequestBuilder
    implements Builder<SetProjectPlanRequest, SetProjectPlanRequestBuilder> {
  _$SetProjectPlanRequest? _$v;

  Date? _plannedStartDate;
  Date? get plannedStartDate => _$this._plannedStartDate;
  set plannedStartDate(Date? plannedStartDate) =>
      _$this._plannedStartDate = plannedStartDate;

  Date? _plannedEndDate;
  Date? get plannedEndDate => _$this._plannedEndDate;
  set plannedEndDate(Date? plannedEndDate) =>
      _$this._plannedEndDate = plannedEndDate;

  SetProjectPlanRequestBuilder() {
    SetProjectPlanRequest._defaults(this);
  }

  SetProjectPlanRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _plannedStartDate = $v.plannedStartDate;
      _plannedEndDate = $v.plannedEndDate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SetProjectPlanRequest other) {
    _$v = other as _$SetProjectPlanRequest;
  }

  @override
  void update(void Function(SetProjectPlanRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SetProjectPlanRequest build() => _build();

  _$SetProjectPlanRequest _build() {
    final _$result = _$v ??
        _$SetProjectPlanRequest._(
          plannedStartDate: plannedStartDate,
          plannedEndDate: plannedEndDate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
