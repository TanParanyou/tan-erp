// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectResponse extends ProjectResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? status;
  @override
  final Date? plannedStartDate;
  @override
  final ProjectPersonResponse? owner;
  @override
  final ProjectCustomerResponse? customer;
  @override
  final ProjectSiteResponse? site;
  @override
  final ProjectOpportunityResponse? opportunity;
  @override
  final ProjectBaselineResponse? baseline;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;

  factory _$ProjectResponse([void Function(ProjectResponseBuilder)? updates]) =>
      (ProjectResponseBuilder()..update(updates))._build();

  _$ProjectResponse._(
      {this.id,
      this.branchId,
      this.code,
      this.name,
      this.status,
      this.plannedStartDate,
      this.owner,
      this.customer,
      this.site,
      this.opportunity,
      this.baseline,
      this.rowVersion,
      this.createdAtUtc})
      : super._();
  @override
  ProjectResponse rebuild(void Function(ProjectResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectResponseBuilder toBuilder() => ProjectResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectResponse &&
        id == other.id &&
        branchId == other.branchId &&
        code == other.code &&
        name == other.name &&
        status == other.status &&
        plannedStartDate == other.plannedStartDate &&
        owner == other.owner &&
        customer == other.customer &&
        site == other.site &&
        opportunity == other.opportunity &&
        baseline == other.baseline &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, plannedStartDate.hashCode);
    _$hash = $jc(_$hash, owner.hashCode);
    _$hash = $jc(_$hash, customer.hashCode);
    _$hash = $jc(_$hash, site.hashCode);
    _$hash = $jc(_$hash, opportunity.hashCode);
    _$hash = $jc(_$hash, baseline.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('code', code)
          ..add('name', name)
          ..add('status', status)
          ..add('plannedStartDate', plannedStartDate)
          ..add('owner', owner)
          ..add('customer', customer)
          ..add('site', site)
          ..add('opportunity', opportunity)
          ..add('baseline', baseline)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class ProjectResponseBuilder
    implements Builder<ProjectResponse, ProjectResponseBuilder> {
  _$ProjectResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  Date? _plannedStartDate;
  Date? get plannedStartDate => _$this._plannedStartDate;
  set plannedStartDate(Date? plannedStartDate) =>
      _$this._plannedStartDate = plannedStartDate;

  ProjectPersonResponseBuilder? _owner;
  ProjectPersonResponseBuilder get owner =>
      _$this._owner ??= ProjectPersonResponseBuilder();
  set owner(ProjectPersonResponseBuilder? owner) => _$this._owner = owner;

  ProjectCustomerResponseBuilder? _customer;
  ProjectCustomerResponseBuilder get customer =>
      _$this._customer ??= ProjectCustomerResponseBuilder();
  set customer(ProjectCustomerResponseBuilder? customer) =>
      _$this._customer = customer;

  ProjectSiteResponseBuilder? _site;
  ProjectSiteResponseBuilder get site =>
      _$this._site ??= ProjectSiteResponseBuilder();
  set site(ProjectSiteResponseBuilder? site) => _$this._site = site;

  ProjectOpportunityResponseBuilder? _opportunity;
  ProjectOpportunityResponseBuilder get opportunity =>
      _$this._opportunity ??= ProjectOpportunityResponseBuilder();
  set opportunity(ProjectOpportunityResponseBuilder? opportunity) =>
      _$this._opportunity = opportunity;

  ProjectBaselineResponseBuilder? _baseline;
  ProjectBaselineResponseBuilder get baseline =>
      _$this._baseline ??= ProjectBaselineResponseBuilder();
  set baseline(ProjectBaselineResponseBuilder? baseline) =>
      _$this._baseline = baseline;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ProjectResponseBuilder() {
    ProjectResponse._defaults(this);
  }

  ProjectResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _code = $v.code;
      _name = $v.name;
      _status = $v.status;
      _plannedStartDate = $v.plannedStartDate;
      _owner = $v.owner?.toBuilder();
      _customer = $v.customer?.toBuilder();
      _site = $v.site?.toBuilder();
      _opportunity = $v.opportunity?.toBuilder();
      _baseline = $v.baseline?.toBuilder();
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectResponse other) {
    _$v = other as _$ProjectResponse;
  }

  @override
  void update(void Function(ProjectResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectResponse build() => _build();

  _$ProjectResponse _build() {
    _$ProjectResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectResponse._(
            id: id,
            branchId: branchId,
            code: code,
            name: name,
            status: status,
            plannedStartDate: plannedStartDate,
            owner: _owner?.build(),
            customer: _customer?.build(),
            site: _site?.build(),
            opportunity: _opportunity?.build(),
            baseline: _baseline?.build(),
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'owner';
        _owner?.build();
        _$failedField = 'customer';
        _customer?.build();
        _$failedField = 'site';
        _site?.build();
        _$failedField = 'opportunity';
        _opportunity?.build();
        _$failedField = 'baseline';
        _baseline?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
