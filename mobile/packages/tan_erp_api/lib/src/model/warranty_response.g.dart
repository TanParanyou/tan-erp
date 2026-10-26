// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warranty_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarrantyResponse extends WarrantyResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final ServiceProjectRefResponse? project;
  @override
  final String? installationJobId;
  @override
  final String? installationNumber;
  @override
  final Date? startDate;
  @override
  final Date? endDate;
  @override
  final int? months;
  @override
  final bool? isActive;
  @override
  final int? serviceRequestCount;

  factory _$WarrantyResponse(
          [void Function(WarrantyResponseBuilder)? updates]) =>
      (WarrantyResponseBuilder()..update(updates))._build();

  _$WarrantyResponse._(
      {this.id,
      this.number,
      this.project,
      this.installationJobId,
      this.installationNumber,
      this.startDate,
      this.endDate,
      this.months,
      this.isActive,
      this.serviceRequestCount})
      : super._();
  @override
  WarrantyResponse rebuild(void Function(WarrantyResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarrantyResponseBuilder toBuilder() =>
      WarrantyResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarrantyResponse &&
        id == other.id &&
        number == other.number &&
        project == other.project &&
        installationJobId == other.installationJobId &&
        installationNumber == other.installationNumber &&
        startDate == other.startDate &&
        endDate == other.endDate &&
        months == other.months &&
        isActive == other.isActive &&
        serviceRequestCount == other.serviceRequestCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, installationJobId.hashCode);
    _$hash = $jc(_$hash, installationNumber.hashCode);
    _$hash = $jc(_$hash, startDate.hashCode);
    _$hash = $jc(_$hash, endDate.hashCode);
    _$hash = $jc(_$hash, months.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, serviceRequestCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WarrantyResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('project', project)
          ..add('installationJobId', installationJobId)
          ..add('installationNumber', installationNumber)
          ..add('startDate', startDate)
          ..add('endDate', endDate)
          ..add('months', months)
          ..add('isActive', isActive)
          ..add('serviceRequestCount', serviceRequestCount))
        .toString();
  }
}

class WarrantyResponseBuilder
    implements Builder<WarrantyResponse, WarrantyResponseBuilder> {
  _$WarrantyResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  ServiceProjectRefResponseBuilder? _project;
  ServiceProjectRefResponseBuilder get project =>
      _$this._project ??= ServiceProjectRefResponseBuilder();
  set project(ServiceProjectRefResponseBuilder? project) =>
      _$this._project = project;

  String? _installationJobId;
  String? get installationJobId => _$this._installationJobId;
  set installationJobId(String? installationJobId) =>
      _$this._installationJobId = installationJobId;

  String? _installationNumber;
  String? get installationNumber => _$this._installationNumber;
  set installationNumber(String? installationNumber) =>
      _$this._installationNumber = installationNumber;

  Date? _startDate;
  Date? get startDate => _$this._startDate;
  set startDate(Date? startDate) => _$this._startDate = startDate;

  Date? _endDate;
  Date? get endDate => _$this._endDate;
  set endDate(Date? endDate) => _$this._endDate = endDate;

  int? _months;
  int? get months => _$this._months;
  set months(int? months) => _$this._months = months;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  int? _serviceRequestCount;
  int? get serviceRequestCount => _$this._serviceRequestCount;
  set serviceRequestCount(int? serviceRequestCount) =>
      _$this._serviceRequestCount = serviceRequestCount;

  WarrantyResponseBuilder() {
    WarrantyResponse._defaults(this);
  }

  WarrantyResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _project = $v.project?.toBuilder();
      _installationJobId = $v.installationJobId;
      _installationNumber = $v.installationNumber;
      _startDate = $v.startDate;
      _endDate = $v.endDate;
      _months = $v.months;
      _isActive = $v.isActive;
      _serviceRequestCount = $v.serviceRequestCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarrantyResponse other) {
    _$v = other as _$WarrantyResponse;
  }

  @override
  void update(void Function(WarrantyResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarrantyResponse build() => _build();

  _$WarrantyResponse _build() {
    _$WarrantyResponse _$result;
    try {
      _$result = _$v ??
          _$WarrantyResponse._(
            id: id,
            number: number,
            project: _project?.build(),
            installationJobId: installationJobId,
            installationNumber: installationNumber,
            startDate: startDate,
            endDate: endDate,
            months: months,
            isActive: isActive,
            serviceRequestCount: serviceRequestCount,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'project';
        _project?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WarrantyResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
