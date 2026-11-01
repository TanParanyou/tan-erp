// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_work_center_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateWorkCenterRequest extends CreateWorkCenterRequest {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? dailyCapacityHours;
  @override
  final double? hourlyRate;
  @override
  final String? branchId;

  factory _$CreateWorkCenterRequest(
          [void Function(CreateWorkCenterRequestBuilder)? updates]) =>
      (CreateWorkCenterRequestBuilder()..update(updates))._build();

  _$CreateWorkCenterRequest._(
      {this.code,
      this.name,
      this.dailyCapacityHours,
      this.hourlyRate,
      this.branchId})
      : super._();
  @override
  CreateWorkCenterRequest rebuild(
          void Function(CreateWorkCenterRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateWorkCenterRequestBuilder toBuilder() =>
      CreateWorkCenterRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateWorkCenterRequest &&
        code == other.code &&
        name == other.name &&
        dailyCapacityHours == other.dailyCapacityHours &&
        hourlyRate == other.hourlyRate &&
        branchId == other.branchId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, dailyCapacityHours.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateWorkCenterRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('dailyCapacityHours', dailyCapacityHours)
          ..add('hourlyRate', hourlyRate)
          ..add('branchId', branchId))
        .toString();
  }
}

class CreateWorkCenterRequestBuilder
    implements
        Builder<CreateWorkCenterRequest, CreateWorkCenterRequestBuilder> {
  _$CreateWorkCenterRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  double? _dailyCapacityHours;
  double? get dailyCapacityHours => _$this._dailyCapacityHours;
  set dailyCapacityHours(double? dailyCapacityHours) =>
      _$this._dailyCapacityHours = dailyCapacityHours;

  double? _hourlyRate;
  double? get hourlyRate => _$this._hourlyRate;
  set hourlyRate(double? hourlyRate) => _$this._hourlyRate = hourlyRate;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  CreateWorkCenterRequestBuilder() {
    CreateWorkCenterRequest._defaults(this);
  }

  CreateWorkCenterRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _dailyCapacityHours = $v.dailyCapacityHours;
      _hourlyRate = $v.hourlyRate;
      _branchId = $v.branchId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateWorkCenterRequest other) {
    _$v = other as _$CreateWorkCenterRequest;
  }

  @override
  void update(void Function(CreateWorkCenterRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateWorkCenterRequest build() => _build();

  _$CreateWorkCenterRequest _build() {
    final _$result = _$v ??
        _$CreateWorkCenterRequest._(
          code: code,
          name: name,
          dailyCapacityHours: dailyCapacityHours,
          hourlyRate: hourlyRate,
          branchId: branchId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
