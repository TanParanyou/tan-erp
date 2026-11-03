// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_revision_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateRevisionSummaryResponse
    extends EstimateRevisionSummaryResponse {
  @override
  final String id;
  @override
  final int revisionNo;
  @override
  final String status;
  @override
  final String currency;
  @override
  final double netCost;
  @override
  final double netBeforeTax;
  @override
  final double grandTotal;
  @override
  final double marginRate;
  @override
  final DateTime createdAtUtc;
  @override
  final DateTime updatedAtUtc;

  factory _$EstimateRevisionSummaryResponse(
          [void Function(EstimateRevisionSummaryResponseBuilder)? updates]) =>
      (EstimateRevisionSummaryResponseBuilder()..update(updates))._build();

  _$EstimateRevisionSummaryResponse._(
      {required this.id,
      required this.revisionNo,
      required this.status,
      required this.currency,
      required this.netCost,
      required this.netBeforeTax,
      required this.grandTotal,
      required this.marginRate,
      required this.createdAtUtc,
      required this.updatedAtUtc})
      : super._();
  @override
  EstimateRevisionSummaryResponse rebuild(
          void Function(EstimateRevisionSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateRevisionSummaryResponseBuilder toBuilder() =>
      EstimateRevisionSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateRevisionSummaryResponse &&
        id == other.id &&
        revisionNo == other.revisionNo &&
        status == other.status &&
        currency == other.currency &&
        netCost == other.netCost &&
        netBeforeTax == other.netBeforeTax &&
        grandTotal == other.grandTotal &&
        marginRate == other.marginRate &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, revisionNo.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, netCost.hashCode);
    _$hash = $jc(_$hash, netBeforeTax.hashCode);
    _$hash = $jc(_$hash, grandTotal.hashCode);
    _$hash = $jc(_$hash, marginRate.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateRevisionSummaryResponse')
          ..add('id', id)
          ..add('revisionNo', revisionNo)
          ..add('status', status)
          ..add('currency', currency)
          ..add('netCost', netCost)
          ..add('netBeforeTax', netBeforeTax)
          ..add('grandTotal', grandTotal)
          ..add('marginRate', marginRate)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class EstimateRevisionSummaryResponseBuilder
    implements
        Builder<EstimateRevisionSummaryResponse,
            EstimateRevisionSummaryResponseBuilder> {
  _$EstimateRevisionSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _revisionNo;
  int? get revisionNo => _$this._revisionNo;
  set revisionNo(int? revisionNo) => _$this._revisionNo = revisionNo;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  double? _netCost;
  double? get netCost => _$this._netCost;
  set netCost(double? netCost) => _$this._netCost = netCost;

  double? _netBeforeTax;
  double? get netBeforeTax => _$this._netBeforeTax;
  set netBeforeTax(double? netBeforeTax) => _$this._netBeforeTax = netBeforeTax;

  double? _grandTotal;
  double? get grandTotal => _$this._grandTotal;
  set grandTotal(double? grandTotal) => _$this._grandTotal = grandTotal;

  double? _marginRate;
  double? get marginRate => _$this._marginRate;
  set marginRate(double? marginRate) => _$this._marginRate = marginRate;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  EstimateRevisionSummaryResponseBuilder() {
    EstimateRevisionSummaryResponse._defaults(this);
  }

  EstimateRevisionSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _revisionNo = $v.revisionNo;
      _status = $v.status;
      _currency = $v.currency;
      _netCost = $v.netCost;
      _netBeforeTax = $v.netBeforeTax;
      _grandTotal = $v.grandTotal;
      _marginRate = $v.marginRate;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateRevisionSummaryResponse other) {
    _$v = other as _$EstimateRevisionSummaryResponse;
  }

  @override
  void update(void Function(EstimateRevisionSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateRevisionSummaryResponse build() => _build();

  _$EstimateRevisionSummaryResponse _build() {
    final _$result = _$v ??
        _$EstimateRevisionSummaryResponse._(
          id: BuiltValueNullFieldError.checkNotNull(
              id, r'EstimateRevisionSummaryResponse', 'id'),
          revisionNo: BuiltValueNullFieldError.checkNotNull(
              revisionNo, r'EstimateRevisionSummaryResponse', 'revisionNo'),
          status: BuiltValueNullFieldError.checkNotNull(
              status, r'EstimateRevisionSummaryResponse', 'status'),
          currency: BuiltValueNullFieldError.checkNotNull(
              currency, r'EstimateRevisionSummaryResponse', 'currency'),
          netCost: BuiltValueNullFieldError.checkNotNull(
              netCost, r'EstimateRevisionSummaryResponse', 'netCost'),
          netBeforeTax: BuiltValueNullFieldError.checkNotNull(
              netBeforeTax, r'EstimateRevisionSummaryResponse', 'netBeforeTax'),
          grandTotal: BuiltValueNullFieldError.checkNotNull(
              grandTotal, r'EstimateRevisionSummaryResponse', 'grandTotal'),
          marginRate: BuiltValueNullFieldError.checkNotNull(
              marginRate, r'EstimateRevisionSummaryResponse', 'marginRate'),
          createdAtUtc: BuiltValueNullFieldError.checkNotNull(
              createdAtUtc, r'EstimateRevisionSummaryResponse', 'createdAtUtc'),
          updatedAtUtc: BuiltValueNullFieldError.checkNotNull(
              updatedAtUtc, r'EstimateRevisionSummaryResponse', 'updatedAtUtc'),
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
