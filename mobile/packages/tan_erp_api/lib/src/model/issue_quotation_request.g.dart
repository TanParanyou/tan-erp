// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'issue_quotation_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$IssueQuotationRequest extends IssueQuotationRequest {
  @override
  final String? expectedEstimateVersion;
  @override
  final String? expectedOpportunityVersion;

  factory _$IssueQuotationRequest(
          [void Function(IssueQuotationRequestBuilder)? updates]) =>
      (IssueQuotationRequestBuilder()..update(updates))._build();

  _$IssueQuotationRequest._(
      {this.expectedEstimateVersion, this.expectedOpportunityVersion})
      : super._();
  @override
  IssueQuotationRequest rebuild(
          void Function(IssueQuotationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  IssueQuotationRequestBuilder toBuilder() =>
      IssueQuotationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is IssueQuotationRequest &&
        expectedEstimateVersion == other.expectedEstimateVersion &&
        expectedOpportunityVersion == other.expectedOpportunityVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedEstimateVersion.hashCode);
    _$hash = $jc(_$hash, expectedOpportunityVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'IssueQuotationRequest')
          ..add('expectedEstimateVersion', expectedEstimateVersion)
          ..add('expectedOpportunityVersion', expectedOpportunityVersion))
        .toString();
  }
}

class IssueQuotationRequestBuilder
    implements Builder<IssueQuotationRequest, IssueQuotationRequestBuilder> {
  _$IssueQuotationRequest? _$v;

  String? _expectedEstimateVersion;
  String? get expectedEstimateVersion => _$this._expectedEstimateVersion;
  set expectedEstimateVersion(String? expectedEstimateVersion) =>
      _$this._expectedEstimateVersion = expectedEstimateVersion;

  String? _expectedOpportunityVersion;
  String? get expectedOpportunityVersion => _$this._expectedOpportunityVersion;
  set expectedOpportunityVersion(String? expectedOpportunityVersion) =>
      _$this._expectedOpportunityVersion = expectedOpportunityVersion;

  IssueQuotationRequestBuilder() {
    IssueQuotationRequest._defaults(this);
  }

  IssueQuotationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedEstimateVersion = $v.expectedEstimateVersion;
      _expectedOpportunityVersion = $v.expectedOpportunityVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(IssueQuotationRequest other) {
    _$v = other as _$IssueQuotationRequest;
  }

  @override
  void update(void Function(IssueQuotationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  IssueQuotationRequest build() => _build();

  _$IssueQuotationRequest _build() {
    final _$result = _$v ??
        _$IssueQuotationRequest._(
          expectedEstimateVersion: expectedEstimateVersion,
          expectedOpportunityVersion: expectedOpportunityVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
