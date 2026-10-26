// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_open_opportunity_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateOpenOpportunityRequest extends UpdateOpenOpportunityRequest {
  @override
  final String? title;
  @override
  final String? primarySiteId;
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

  factory _$UpdateOpenOpportunityRequest(
          [void Function(UpdateOpenOpportunityRequestBuilder)? updates]) =>
      (UpdateOpenOpportunityRequestBuilder()..update(updates))._build();

  _$UpdateOpenOpportunityRequest._(
      {this.title,
      this.primarySiteId,
      this.scopeSummary,
      this.workTypes,
      this.sourceCode,
      this.expectedBudget,
      this.currencyCode,
      this.targetDecisionDate,
      this.nextActionAtUtc,
      this.nextActionNote})
      : super._();
  @override
  UpdateOpenOpportunityRequest rebuild(
          void Function(UpdateOpenOpportunityRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateOpenOpportunityRequestBuilder toBuilder() =>
      UpdateOpenOpportunityRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateOpenOpportunityRequest &&
        title == other.title &&
        primarySiteId == other.primarySiteId &&
        scopeSummary == other.scopeSummary &&
        workTypes == other.workTypes &&
        sourceCode == other.sourceCode &&
        expectedBudget == other.expectedBudget &&
        currencyCode == other.currencyCode &&
        targetDecisionDate == other.targetDecisionDate &&
        nextActionAtUtc == other.nextActionAtUtc &&
        nextActionNote == other.nextActionNote;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, primarySiteId.hashCode);
    _$hash = $jc(_$hash, scopeSummary.hashCode);
    _$hash = $jc(_$hash, workTypes.hashCode);
    _$hash = $jc(_$hash, sourceCode.hashCode);
    _$hash = $jc(_$hash, expectedBudget.hashCode);
    _$hash = $jc(_$hash, currencyCode.hashCode);
    _$hash = $jc(_$hash, targetDecisionDate.hashCode);
    _$hash = $jc(_$hash, nextActionAtUtc.hashCode);
    _$hash = $jc(_$hash, nextActionNote.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateOpenOpportunityRequest')
          ..add('title', title)
          ..add('primarySiteId', primarySiteId)
          ..add('scopeSummary', scopeSummary)
          ..add('workTypes', workTypes)
          ..add('sourceCode', sourceCode)
          ..add('expectedBudget', expectedBudget)
          ..add('currencyCode', currencyCode)
          ..add('targetDecisionDate', targetDecisionDate)
          ..add('nextActionAtUtc', nextActionAtUtc)
          ..add('nextActionNote', nextActionNote))
        .toString();
  }
}

class UpdateOpenOpportunityRequestBuilder
    implements
        Builder<UpdateOpenOpportunityRequest,
            UpdateOpenOpportunityRequestBuilder> {
  _$UpdateOpenOpportunityRequest? _$v;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _primarySiteId;
  String? get primarySiteId => _$this._primarySiteId;
  set primarySiteId(String? primarySiteId) =>
      _$this._primarySiteId = primarySiteId;

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

  UpdateOpenOpportunityRequestBuilder() {
    UpdateOpenOpportunityRequest._defaults(this);
  }

  UpdateOpenOpportunityRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _title = $v.title;
      _primarySiteId = $v.primarySiteId;
      _scopeSummary = $v.scopeSummary;
      _workTypes = $v.workTypes?.toBuilder();
      _sourceCode = $v.sourceCode;
      _expectedBudget = $v.expectedBudget;
      _currencyCode = $v.currencyCode;
      _targetDecisionDate = $v.targetDecisionDate;
      _nextActionAtUtc = $v.nextActionAtUtc;
      _nextActionNote = $v.nextActionNote;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateOpenOpportunityRequest other) {
    _$v = other as _$UpdateOpenOpportunityRequest;
  }

  @override
  void update(void Function(UpdateOpenOpportunityRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateOpenOpportunityRequest build() => _build();

  _$UpdateOpenOpportunityRequest _build() {
    _$UpdateOpenOpportunityRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateOpenOpportunityRequest._(
            title: title,
            primarySiteId: primarySiteId,
            scopeSummary: scopeSummary,
            workTypes: _workTypes?.build(),
            sourceCode: sourceCode,
            expectedBudget: expectedBudget,
            currencyCode: currencyCode,
            targetDecisionDate: targetDecisionDate,
            nextActionAtUtc: nextActionAtUtc,
            nextActionNote: nextActionNote,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workTypes';
        _workTypes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateOpenOpportunityRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
