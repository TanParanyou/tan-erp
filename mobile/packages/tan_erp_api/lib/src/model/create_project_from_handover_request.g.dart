// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_project_from_handover_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateProjectFromHandoverRequest
    extends CreateProjectFromHandoverRequest {
  @override
  final String? quotationId;
  @override
  final String? expectedQuotationVersion;
  @override
  final String? ownerUserId;
  @override
  final Date? plannedStartDate;
  @override
  final String? name;

  factory _$CreateProjectFromHandoverRequest(
          [void Function(CreateProjectFromHandoverRequestBuilder)? updates]) =>
      (CreateProjectFromHandoverRequestBuilder()..update(updates))._build();

  _$CreateProjectFromHandoverRequest._(
      {this.quotationId,
      this.expectedQuotationVersion,
      this.ownerUserId,
      this.plannedStartDate,
      this.name})
      : super._();
  @override
  CreateProjectFromHandoverRequest rebuild(
          void Function(CreateProjectFromHandoverRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateProjectFromHandoverRequestBuilder toBuilder() =>
      CreateProjectFromHandoverRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateProjectFromHandoverRequest &&
        quotationId == other.quotationId &&
        expectedQuotationVersion == other.expectedQuotationVersion &&
        ownerUserId == other.ownerUserId &&
        plannedStartDate == other.plannedStartDate &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, quotationId.hashCode);
    _$hash = $jc(_$hash, expectedQuotationVersion.hashCode);
    _$hash = $jc(_$hash, ownerUserId.hashCode);
    _$hash = $jc(_$hash, plannedStartDate.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateProjectFromHandoverRequest')
          ..add('quotationId', quotationId)
          ..add('expectedQuotationVersion', expectedQuotationVersion)
          ..add('ownerUserId', ownerUserId)
          ..add('plannedStartDate', plannedStartDate)
          ..add('name', name))
        .toString();
  }
}

class CreateProjectFromHandoverRequestBuilder
    implements
        Builder<CreateProjectFromHandoverRequest,
            CreateProjectFromHandoverRequestBuilder> {
  _$CreateProjectFromHandoverRequest? _$v;

  String? _quotationId;
  String? get quotationId => _$this._quotationId;
  set quotationId(String? quotationId) => _$this._quotationId = quotationId;

  String? _expectedQuotationVersion;
  String? get expectedQuotationVersion => _$this._expectedQuotationVersion;
  set expectedQuotationVersion(String? expectedQuotationVersion) =>
      _$this._expectedQuotationVersion = expectedQuotationVersion;

  String? _ownerUserId;
  String? get ownerUserId => _$this._ownerUserId;
  set ownerUserId(String? ownerUserId) => _$this._ownerUserId = ownerUserId;

  Date? _plannedStartDate;
  Date? get plannedStartDate => _$this._plannedStartDate;
  set plannedStartDate(Date? plannedStartDate) =>
      _$this._plannedStartDate = plannedStartDate;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  CreateProjectFromHandoverRequestBuilder() {
    CreateProjectFromHandoverRequest._defaults(this);
  }

  CreateProjectFromHandoverRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _quotationId = $v.quotationId;
      _expectedQuotationVersion = $v.expectedQuotationVersion;
      _ownerUserId = $v.ownerUserId;
      _plannedStartDate = $v.plannedStartDate;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateProjectFromHandoverRequest other) {
    _$v = other as _$CreateProjectFromHandoverRequest;
  }

  @override
  void update(void Function(CreateProjectFromHandoverRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateProjectFromHandoverRequest build() => _build();

  _$CreateProjectFromHandoverRequest _build() {
    final _$result = _$v ??
        _$CreateProjectFromHandoverRequest._(
          quotationId: quotationId,
          expectedQuotationVersion: expectedQuotationVersion,
          ownerUserId: ownerUserId,
          plannedStartDate: plannedStartDate,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
