// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_billing_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBillingSummaryResponse extends ProjectBillingSummaryResponse {
  @override
  final FinanceProjectRefResponse? project;
  @override
  final double? contractAmount;
  @override
  final double? billed;
  @override
  final double? paid;
  @override
  final double? outstanding;
  @override
  final double? unbilled;

  factory _$ProjectBillingSummaryResponse(
          [void Function(ProjectBillingSummaryResponseBuilder)? updates]) =>
      (ProjectBillingSummaryResponseBuilder()..update(updates))._build();

  _$ProjectBillingSummaryResponse._(
      {this.project,
      this.contractAmount,
      this.billed,
      this.paid,
      this.outstanding,
      this.unbilled})
      : super._();
  @override
  ProjectBillingSummaryResponse rebuild(
          void Function(ProjectBillingSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBillingSummaryResponseBuilder toBuilder() =>
      ProjectBillingSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBillingSummaryResponse &&
        project == other.project &&
        contractAmount == other.contractAmount &&
        billed == other.billed &&
        paid == other.paid &&
        outstanding == other.outstanding &&
        unbilled == other.unbilled;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, contractAmount.hashCode);
    _$hash = $jc(_$hash, billed.hashCode);
    _$hash = $jc(_$hash, paid.hashCode);
    _$hash = $jc(_$hash, outstanding.hashCode);
    _$hash = $jc(_$hash, unbilled.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBillingSummaryResponse')
          ..add('project', project)
          ..add('contractAmount', contractAmount)
          ..add('billed', billed)
          ..add('paid', paid)
          ..add('outstanding', outstanding)
          ..add('unbilled', unbilled))
        .toString();
  }
}

class ProjectBillingSummaryResponseBuilder
    implements
        Builder<ProjectBillingSummaryResponse,
            ProjectBillingSummaryResponseBuilder> {
  _$ProjectBillingSummaryResponse? _$v;

  FinanceProjectRefResponseBuilder? _project;
  FinanceProjectRefResponseBuilder get project =>
      _$this._project ??= FinanceProjectRefResponseBuilder();
  set project(FinanceProjectRefResponseBuilder? project) =>
      _$this._project = project;

  double? _contractAmount;
  double? get contractAmount => _$this._contractAmount;
  set contractAmount(double? contractAmount) =>
      _$this._contractAmount = contractAmount;

  double? _billed;
  double? get billed => _$this._billed;
  set billed(double? billed) => _$this._billed = billed;

  double? _paid;
  double? get paid => _$this._paid;
  set paid(double? paid) => _$this._paid = paid;

  double? _outstanding;
  double? get outstanding => _$this._outstanding;
  set outstanding(double? outstanding) => _$this._outstanding = outstanding;

  double? _unbilled;
  double? get unbilled => _$this._unbilled;
  set unbilled(double? unbilled) => _$this._unbilled = unbilled;

  ProjectBillingSummaryResponseBuilder() {
    ProjectBillingSummaryResponse._defaults(this);
  }

  ProjectBillingSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _project = $v.project?.toBuilder();
      _contractAmount = $v.contractAmount;
      _billed = $v.billed;
      _paid = $v.paid;
      _outstanding = $v.outstanding;
      _unbilled = $v.unbilled;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBillingSummaryResponse other) {
    _$v = other as _$ProjectBillingSummaryResponse;
  }

  @override
  void update(void Function(ProjectBillingSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBillingSummaryResponse build() => _build();

  _$ProjectBillingSummaryResponse _build() {
    _$ProjectBillingSummaryResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectBillingSummaryResponse._(
            project: _project?.build(),
            contractAmount: contractAmount,
            billed: billed,
            paid: paid,
            outstanding: outstanding,
            unbilled: unbilled,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'project';
        _project?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectBillingSummaryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
