// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_estimate_draft_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateEstimateDraftRequest extends CreateEstimateDraftRequest {
  @override
  final String? opportunityId;
  @override
  final String? siteSurveyRevisionId;
  @override
  final String? currency;

  factory _$CreateEstimateDraftRequest(
          [void Function(CreateEstimateDraftRequestBuilder)? updates]) =>
      (CreateEstimateDraftRequestBuilder()..update(updates))._build();

  _$CreateEstimateDraftRequest._(
      {this.opportunityId, this.siteSurveyRevisionId, this.currency})
      : super._();
  @override
  CreateEstimateDraftRequest rebuild(
          void Function(CreateEstimateDraftRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateEstimateDraftRequestBuilder toBuilder() =>
      CreateEstimateDraftRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateEstimateDraftRequest &&
        opportunityId == other.opportunityId &&
        siteSurveyRevisionId == other.siteSurveyRevisionId &&
        currency == other.currency;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, siteSurveyRevisionId.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateEstimateDraftRequest')
          ..add('opportunityId', opportunityId)
          ..add('siteSurveyRevisionId', siteSurveyRevisionId)
          ..add('currency', currency))
        .toString();
  }
}

class CreateEstimateDraftRequestBuilder
    implements
        Builder<CreateEstimateDraftRequest, CreateEstimateDraftRequestBuilder> {
  _$CreateEstimateDraftRequest? _$v;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  String? _siteSurveyRevisionId;
  String? get siteSurveyRevisionId => _$this._siteSurveyRevisionId;
  set siteSurveyRevisionId(String? siteSurveyRevisionId) =>
      _$this._siteSurveyRevisionId = siteSurveyRevisionId;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  CreateEstimateDraftRequestBuilder() {
    CreateEstimateDraftRequest._defaults(this);
  }

  CreateEstimateDraftRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _opportunityId = $v.opportunityId;
      _siteSurveyRevisionId = $v.siteSurveyRevisionId;
      _currency = $v.currency;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateEstimateDraftRequest other) {
    _$v = other as _$CreateEstimateDraftRequest;
  }

  @override
  void update(void Function(CreateEstimateDraftRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateEstimateDraftRequest build() => _build();

  _$CreateEstimateDraftRequest _build() {
    final _$result = _$v ??
        _$CreateEstimateDraftRequest._(
          opportunityId: opportunityId,
          siteSurveyRevisionId: siteSurveyRevisionId,
          currency: currency,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
