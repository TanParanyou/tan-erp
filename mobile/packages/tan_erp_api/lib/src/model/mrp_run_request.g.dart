// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_run_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpRunRequest extends MrpRunRequest {
  @override
  final Date? asOfDate;
  @override
  final int? purchaseLeadTimeDays;
  @override
  final int? productionLeadTimeDays;
  @override
  final bool? includeOpenWorkOrders;
  @override
  final BuiltList<MrpDemandRequest>? demands;

  factory _$MrpRunRequest([void Function(MrpRunRequestBuilder)? updates]) =>
      (MrpRunRequestBuilder()..update(updates))._build();

  _$MrpRunRequest._(
      {this.asOfDate,
      this.purchaseLeadTimeDays,
      this.productionLeadTimeDays,
      this.includeOpenWorkOrders,
      this.demands})
      : super._();
  @override
  MrpRunRequest rebuild(void Function(MrpRunRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpRunRequestBuilder toBuilder() => MrpRunRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpRunRequest &&
        asOfDate == other.asOfDate &&
        purchaseLeadTimeDays == other.purchaseLeadTimeDays &&
        productionLeadTimeDays == other.productionLeadTimeDays &&
        includeOpenWorkOrders == other.includeOpenWorkOrders &&
        demands == other.demands;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, asOfDate.hashCode);
    _$hash = $jc(_$hash, purchaseLeadTimeDays.hashCode);
    _$hash = $jc(_$hash, productionLeadTimeDays.hashCode);
    _$hash = $jc(_$hash, includeOpenWorkOrders.hashCode);
    _$hash = $jc(_$hash, demands.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpRunRequest')
          ..add('asOfDate', asOfDate)
          ..add('purchaseLeadTimeDays', purchaseLeadTimeDays)
          ..add('productionLeadTimeDays', productionLeadTimeDays)
          ..add('includeOpenWorkOrders', includeOpenWorkOrders)
          ..add('demands', demands))
        .toString();
  }
}

class MrpRunRequestBuilder
    implements Builder<MrpRunRequest, MrpRunRequestBuilder> {
  _$MrpRunRequest? _$v;

  Date? _asOfDate;
  Date? get asOfDate => _$this._asOfDate;
  set asOfDate(Date? asOfDate) => _$this._asOfDate = asOfDate;

  int? _purchaseLeadTimeDays;
  int? get purchaseLeadTimeDays => _$this._purchaseLeadTimeDays;
  set purchaseLeadTimeDays(int? purchaseLeadTimeDays) =>
      _$this._purchaseLeadTimeDays = purchaseLeadTimeDays;

  int? _productionLeadTimeDays;
  int? get productionLeadTimeDays => _$this._productionLeadTimeDays;
  set productionLeadTimeDays(int? productionLeadTimeDays) =>
      _$this._productionLeadTimeDays = productionLeadTimeDays;

  bool? _includeOpenWorkOrders;
  bool? get includeOpenWorkOrders => _$this._includeOpenWorkOrders;
  set includeOpenWorkOrders(bool? includeOpenWorkOrders) =>
      _$this._includeOpenWorkOrders = includeOpenWorkOrders;

  ListBuilder<MrpDemandRequest>? _demands;
  ListBuilder<MrpDemandRequest> get demands =>
      _$this._demands ??= ListBuilder<MrpDemandRequest>();
  set demands(ListBuilder<MrpDemandRequest>? demands) =>
      _$this._demands = demands;

  MrpRunRequestBuilder() {
    MrpRunRequest._defaults(this);
  }

  MrpRunRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _asOfDate = $v.asOfDate;
      _purchaseLeadTimeDays = $v.purchaseLeadTimeDays;
      _productionLeadTimeDays = $v.productionLeadTimeDays;
      _includeOpenWorkOrders = $v.includeOpenWorkOrders;
      _demands = $v.demands?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpRunRequest other) {
    _$v = other as _$MrpRunRequest;
  }

  @override
  void update(void Function(MrpRunRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpRunRequest build() => _build();

  _$MrpRunRequest _build() {
    _$MrpRunRequest _$result;
    try {
      _$result = _$v ??
          _$MrpRunRequest._(
            asOfDate: asOfDate,
            purchaseLeadTimeDays: purchaseLeadTimeDays,
            productionLeadTimeDays: productionLeadTimeDays,
            includeOpenWorkOrders: includeOpenWorkOrders,
            demands: _demands?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'demands';
        _demands?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'MrpRunRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
