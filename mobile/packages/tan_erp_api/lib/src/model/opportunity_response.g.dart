// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'opportunity_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OpportunityResponse extends OpportunityResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? title;
  @override
  final String? scopeSummary;
  @override
  final BuiltList<String>? workTypes;
  @override
  final String? sourceCode;
  @override
  final double? expectedBudget;
  @override
  final String? currencyCode;
  @override
  final Date? targetDecisionDate;
  @override
  final DateTime? nextActionAtUtc;
  @override
  final String? nextActionNote;
  @override
  final String? stage;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final CustomerSummaryResponse? customer;
  @override
  final SiteSummaryResponse? primarySite;
  @override
  final BranchSummaryResponse? branch;
  @override
  final OwnerSummaryResponse? owner;

  factory _$OpportunityResponse(
          [void Function(OpportunityResponseBuilder)? updates]) =>
      (OpportunityResponseBuilder()..update(updates))._build();

  _$OpportunityResponse._(
      {this.id,
      this.code,
      this.title,
      this.scopeSummary,
      this.workTypes,
      this.sourceCode,
      this.expectedBudget,
      this.currencyCode,
      this.targetDecisionDate,
      this.nextActionAtUtc,
      this.nextActionNote,
      this.stage,
      this.rowVersion,
      this.createdAtUtc,
      this.customer,
      this.primarySite,
      this.branch,
      this.owner})
      : super._();
  @override
  OpportunityResponse rebuild(
          void Function(OpportunityResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OpportunityResponseBuilder toBuilder() =>
      OpportunityResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OpportunityResponse &&
        id == other.id &&
        code == other.code &&
        title == other.title &&
        scopeSummary == other.scopeSummary &&
        workTypes == other.workTypes &&
        sourceCode == other.sourceCode &&
        expectedBudget == other.expectedBudget &&
        currencyCode == other.currencyCode &&
        targetDecisionDate == other.targetDecisionDate &&
        nextActionAtUtc == other.nextActionAtUtc &&
        nextActionNote == other.nextActionNote &&
        stage == other.stage &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        customer == other.customer &&
        primarySite == other.primarySite &&
        branch == other.branch &&
        owner == other.owner;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, scopeSummary.hashCode);
    _$hash = $jc(_$hash, workTypes.hashCode);
    _$hash = $jc(_$hash, sourceCode.hashCode);
    _$hash = $jc(_$hash, expectedBudget.hashCode);
    _$hash = $jc(_$hash, currencyCode.hashCode);
    _$hash = $jc(_$hash, targetDecisionDate.hashCode);
    _$hash = $jc(_$hash, nextActionAtUtc.hashCode);
    _$hash = $jc(_$hash, nextActionNote.hashCode);
    _$hash = $jc(_$hash, stage.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, customer.hashCode);
    _$hash = $jc(_$hash, primarySite.hashCode);
    _$hash = $jc(_$hash, branch.hashCode);
    _$hash = $jc(_$hash, owner.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OpportunityResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('title', title)
          ..add('scopeSummary', scopeSummary)
          ..add('workTypes', workTypes)
          ..add('sourceCode', sourceCode)
          ..add('expectedBudget', expectedBudget)
          ..add('currencyCode', currencyCode)
          ..add('targetDecisionDate', targetDecisionDate)
          ..add('nextActionAtUtc', nextActionAtUtc)
          ..add('nextActionNote', nextActionNote)
          ..add('stage', stage)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('customer', customer)
          ..add('primarySite', primarySite)
          ..add('branch', branch)
          ..add('owner', owner))
        .toString();
  }
}

class OpportunityResponseBuilder
    implements Builder<OpportunityResponse, OpportunityResponseBuilder> {
  _$OpportunityResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _scopeSummary;
  String? get scopeSummary => _$this._scopeSummary;
  set scopeSummary(String? scopeSummary) => _$this._scopeSummary = scopeSummary;

  ListBuilder<String>? _workTypes;
  ListBuilder<String> get workTypes =>
      _$this._workTypes ??= ListBuilder<String>();
  set workTypes(ListBuilder<String>? workTypes) =>
      _$this._workTypes = workTypes;

  String? _sourceCode;
  String? get sourceCode => _$this._sourceCode;
  set sourceCode(String? sourceCode) => _$this._sourceCode = sourceCode;

  double? _expectedBudget;
  double? get expectedBudget => _$this._expectedBudget;
  set expectedBudget(double? expectedBudget) =>
      _$this._expectedBudget = expectedBudget;

  String? _currencyCode;
  String? get currencyCode => _$this._currencyCode;
  set currencyCode(String? currencyCode) => _$this._currencyCode = currencyCode;

  Date? _targetDecisionDate;
  Date? get targetDecisionDate => _$this._targetDecisionDate;
  set targetDecisionDate(Date? targetDecisionDate) =>
      _$this._targetDecisionDate = targetDecisionDate;

  DateTime? _nextActionAtUtc;
  DateTime? get nextActionAtUtc => _$this._nextActionAtUtc;
  set nextActionAtUtc(DateTime? nextActionAtUtc) =>
      _$this._nextActionAtUtc = nextActionAtUtc;

  String? _nextActionNote;
  String? get nextActionNote => _$this._nextActionNote;
  set nextActionNote(String? nextActionNote) =>
      _$this._nextActionNote = nextActionNote;

  String? _stage;
  String? get stage => _$this._stage;
  set stage(String? stage) => _$this._stage = stage;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  CustomerSummaryResponseBuilder? _customer;
  CustomerSummaryResponseBuilder get customer =>
      _$this._customer ??= CustomerSummaryResponseBuilder();
  set customer(CustomerSummaryResponseBuilder? customer) =>
      _$this._customer = customer;

  SiteSummaryResponseBuilder? _primarySite;
  SiteSummaryResponseBuilder get primarySite =>
      _$this._primarySite ??= SiteSummaryResponseBuilder();
  set primarySite(SiteSummaryResponseBuilder? primarySite) =>
      _$this._primarySite = primarySite;

  BranchSummaryResponseBuilder? _branch;
  BranchSummaryResponseBuilder get branch =>
      _$this._branch ??= BranchSummaryResponseBuilder();
  set branch(BranchSummaryResponseBuilder? branch) => _$this._branch = branch;

  OwnerSummaryResponseBuilder? _owner;
  OwnerSummaryResponseBuilder get owner =>
      _$this._owner ??= OwnerSummaryResponseBuilder();
  set owner(OwnerSummaryResponseBuilder? owner) => _$this._owner = owner;

  OpportunityResponseBuilder() {
    OpportunityResponse._defaults(this);
  }

  OpportunityResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _title = $v.title;
      _scopeSummary = $v.scopeSummary;
      _workTypes = $v.workTypes?.toBuilder();
      _sourceCode = $v.sourceCode;
      _expectedBudget = $v.expectedBudget;
      _currencyCode = $v.currencyCode;
      _targetDecisionDate = $v.targetDecisionDate;
      _nextActionAtUtc = $v.nextActionAtUtc;
      _nextActionNote = $v.nextActionNote;
      _stage = $v.stage;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _customer = $v.customer?.toBuilder();
      _primarySite = $v.primarySite?.toBuilder();
      _branch = $v.branch?.toBuilder();
      _owner = $v.owner?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OpportunityResponse other) {
    _$v = other as _$OpportunityResponse;
  }

  @override
  void update(void Function(OpportunityResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OpportunityResponse build() => _build();

  _$OpportunityResponse _build() {
    _$OpportunityResponse _$result;
    try {
      _$result = _$v ??
          _$OpportunityResponse._(
            id: id,
            code: code,
            title: title,
            scopeSummary: scopeSummary,
            workTypes: _workTypes?.build(),
            sourceCode: sourceCode,
            expectedBudget: expectedBudget,
            currencyCode: currencyCode,
            targetDecisionDate: targetDecisionDate,
            nextActionAtUtc: nextActionAtUtc,
            nextActionNote: nextActionNote,
            stage: stage,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            customer: _customer?.build(),
            primarySite: _primarySite?.build(),
            branch: _branch?.build(),
            owner: _owner?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workTypes';
        _workTypes?.build();

        _$failedField = 'customer';
        _customer?.build();
        _$failedField = 'primarySite';
        _primarySite?.build();
        _$failedField = 'branch';
        _branch?.build();
        _$failedField = 'owner';
        _owner?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'OpportunityResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
