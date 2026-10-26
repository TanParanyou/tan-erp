// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'accept_quotation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AcceptQuotationResponse extends AcceptQuotationResponse {
  @override
  final String? quotationId;
  @override
  final String? opportunityId;
  @override
  final String? opportunityStage;
  @override
  final String? opportunityRowVersion;
  @override
  final DateTime? acceptedAtUtc;

  factory _$AcceptQuotationResponse(
          [void Function(AcceptQuotationResponseBuilder)? updates]) =>
      (AcceptQuotationResponseBuilder()..update(updates))._build();

  _$AcceptQuotationResponse._(
      {this.quotationId,
      this.opportunityId,
      this.opportunityStage,
      this.opportunityRowVersion,
      this.acceptedAtUtc})
      : super._();
  @override
  AcceptQuotationResponse rebuild(
          void Function(AcceptQuotationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AcceptQuotationResponseBuilder toBuilder() =>
      AcceptQuotationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AcceptQuotationResponse &&
        quotationId == other.quotationId &&
        opportunityId == other.opportunityId &&
        opportunityStage == other.opportunityStage &&
        opportunityRowVersion == other.opportunityRowVersion &&
        acceptedAtUtc == other.acceptedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, quotationId.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, opportunityStage.hashCode);
    _$hash = $jc(_$hash, opportunityRowVersion.hashCode);
    _$hash = $jc(_$hash, acceptedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AcceptQuotationResponse')
          ..add('quotationId', quotationId)
          ..add('opportunityId', opportunityId)
          ..add('opportunityStage', opportunityStage)
          ..add('opportunityRowVersion', opportunityRowVersion)
          ..add('acceptedAtUtc', acceptedAtUtc))
        .toString();
  }
}

class AcceptQuotationResponseBuilder
    implements
        Builder<AcceptQuotationResponse, AcceptQuotationResponseBuilder> {
  _$AcceptQuotationResponse? _$v;

  String? _quotationId;
  String? get quotationId => _$this._quotationId;
  set quotationId(String? quotationId) => _$this._quotationId = quotationId;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  String? _opportunityStage;
  String? get opportunityStage => _$this._opportunityStage;
  set opportunityStage(String? opportunityStage) =>
      _$this._opportunityStage = opportunityStage;

  String? _opportunityRowVersion;
  String? get opportunityRowVersion => _$this._opportunityRowVersion;
  set opportunityRowVersion(String? opportunityRowVersion) =>
      _$this._opportunityRowVersion = opportunityRowVersion;

  DateTime? _acceptedAtUtc;
  DateTime? get acceptedAtUtc => _$this._acceptedAtUtc;
  set acceptedAtUtc(DateTime? acceptedAtUtc) =>
      _$this._acceptedAtUtc = acceptedAtUtc;

  AcceptQuotationResponseBuilder() {
    AcceptQuotationResponse._defaults(this);
  }

  AcceptQuotationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _quotationId = $v.quotationId;
      _opportunityId = $v.opportunityId;
      _opportunityStage = $v.opportunityStage;
      _opportunityRowVersion = $v.opportunityRowVersion;
      _acceptedAtUtc = $v.acceptedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AcceptQuotationResponse other) {
    _$v = other as _$AcceptQuotationResponse;
  }

  @override
  void update(void Function(AcceptQuotationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AcceptQuotationResponse build() => _build();

  _$AcceptQuotationResponse _build() {
    final _$result = _$v ??
        _$AcceptQuotationResponse._(
          quotationId: quotationId,
          opportunityId: opportunityId,
          opportunityStage: opportunityStage,
          opportunityRowVersion: opportunityRowVersion,
          acceptedAtUtc: acceptedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
