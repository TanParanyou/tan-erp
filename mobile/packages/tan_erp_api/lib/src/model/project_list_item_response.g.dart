// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectListItemResponse extends ProjectListItemResponse {
  @override
  final String? id;
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
  final double? contractAmount;
  @override
  final DateTime? createdAtUtc;

  factory _$ProjectListItemResponse(
          [void Function(ProjectListItemResponseBuilder)? updates]) =>
      (ProjectListItemResponseBuilder()..update(updates))._build();

  _$ProjectListItemResponse._(
      {this.id,
      this.code,
      this.name,
      this.status,
      this.plannedStartDate,
      this.owner,
      this.customer,
      this.contractAmount,
      this.createdAtUtc})
      : super._();
  @override
  ProjectListItemResponse rebuild(
          void Function(ProjectListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectListItemResponseBuilder toBuilder() =>
      ProjectListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectListItemResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        status == other.status &&
        plannedStartDate == other.plannedStartDate &&
        owner == other.owner &&
        customer == other.customer &&
        contractAmount == other.contractAmount &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, plannedStartDate.hashCode);
    _$hash = $jc(_$hash, owner.hashCode);
    _$hash = $jc(_$hash, customer.hashCode);
    _$hash = $jc(_$hash, contractAmount.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectListItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('status', status)
          ..add('plannedStartDate', plannedStartDate)
          ..add('owner', owner)
          ..add('customer', customer)
          ..add('contractAmount', contractAmount)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class ProjectListItemResponseBuilder
    implements
        Builder<ProjectListItemResponse, ProjectListItemResponseBuilder> {
  _$ProjectListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  double? _contractAmount;
  double? get contractAmount => _$this._contractAmount;
  set contractAmount(double? contractAmount) =>
      _$this._contractAmount = contractAmount;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ProjectListItemResponseBuilder() {
    ProjectListItemResponse._defaults(this);
  }

  ProjectListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _status = $v.status;
      _plannedStartDate = $v.plannedStartDate;
      _owner = $v.owner?.toBuilder();
      _customer = $v.customer?.toBuilder();
      _contractAmount = $v.contractAmount;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectListItemResponse other) {
    _$v = other as _$ProjectListItemResponse;
  }

  @override
  void update(void Function(ProjectListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectListItemResponse build() => _build();

  _$ProjectListItemResponse _build() {
    _$ProjectListItemResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectListItemResponse._(
            id: id,
            code: code,
            name: name,
            status: status,
            plannedStartDate: plannedStartDate,
            owner: _owner?.build(),
            customer: _customer?.build(),
            contractAmount: contractAmount,
            createdAtUtc: createdAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'owner';
        _owner?.build();
        _$failedField = 'customer';
        _customer?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectListItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
