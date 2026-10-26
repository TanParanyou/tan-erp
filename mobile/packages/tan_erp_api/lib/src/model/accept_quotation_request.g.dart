// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'accept_quotation_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AcceptQuotationRequest extends AcceptQuotationRequest {
  @override
  final String? expectedOpportunityVersion;
  @override
  final String? decisionNote;

  factory _$AcceptQuotationRequest(
          [void Function(AcceptQuotationRequestBuilder)? updates]) =>
      (AcceptQuotationRequestBuilder()..update(updates))._build();

  _$AcceptQuotationRequest._(
      {this.expectedOpportunityVersion, this.decisionNote})
      : super._();
  @override
  AcceptQuotationRequest rebuild(
          void Function(AcceptQuotationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AcceptQuotationRequestBuilder toBuilder() =>
      AcceptQuotationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AcceptQuotationRequest &&
        expectedOpportunityVersion == other.expectedOpportunityVersion &&
        decisionNote == other.decisionNote;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedOpportunityVersion.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AcceptQuotationRequest')
          ..add('expectedOpportunityVersion', expectedOpportunityVersion)
          ..add('decisionNote', decisionNote))
        .toString();
  }
}

class AcceptQuotationRequestBuilder
    implements Builder<AcceptQuotationRequest, AcceptQuotationRequestBuilder> {
  _$AcceptQuotationRequest? _$v;

  String? _expectedOpportunityVersion;
  String? get expectedOpportunityVersion => _$this._expectedOpportunityVersion;
  set expectedOpportunityVersion(String? expectedOpportunityVersion) =>
      _$this._expectedOpportunityVersion = expectedOpportunityVersion;

  String? _decisionNote;
  String? get decisionNote => _$this._decisionNote;
  set decisionNote(String? decisionNote) => _$this._decisionNote = decisionNote;

  AcceptQuotationRequestBuilder() {
    AcceptQuotationRequest._defaults(this);
  }

  AcceptQuotationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedOpportunityVersion = $v.expectedOpportunityVersion;
      _decisionNote = $v.decisionNote;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AcceptQuotationRequest other) {
    _$v = other as _$AcceptQuotationRequest;
  }

  @override
  void update(void Function(AcceptQuotationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AcceptQuotationRequest build() => _build();

  _$AcceptQuotationRequest _build() {
    final _$result = _$v ??
        _$AcceptQuotationRequest._(
          expectedOpportunityVersion: expectedOpportunityVersion,
          decisionNote: decisionNote,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
