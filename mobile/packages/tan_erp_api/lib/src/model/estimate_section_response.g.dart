// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateSectionResponse extends EstimateSectionResponse {
  @override
  final String? id;
  @override
  final String? estimateRevisionId;
  @override
  final String? code;
  @override
  final String? nameTh;
  @override
  final String? nameEn;
  @override
  final int? sortOrder;
  @override
  final double? subtotalCost;
  @override
  final double? subtotalSellingPrice;
  @override
  final BuiltList<EstimateWorkItemResponse>? workItems;

  factory _$EstimateSectionResponse(
          [void Function(EstimateSectionResponseBuilder)? updates]) =>
      (EstimateSectionResponseBuilder()..update(updates))._build();

  _$EstimateSectionResponse._(
      {this.id,
      this.estimateRevisionId,
      this.code,
      this.nameTh,
      this.nameEn,
      this.sortOrder,
      this.subtotalCost,
      this.subtotalSellingPrice,
      this.workItems})
      : super._();
  @override
  EstimateSectionResponse rebuild(
          void Function(EstimateSectionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateSectionResponseBuilder toBuilder() =>
      EstimateSectionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateSectionResponse &&
        id == other.id &&
        estimateRevisionId == other.estimateRevisionId &&
        code == other.code &&
        nameTh == other.nameTh &&
        nameEn == other.nameEn &&
        sortOrder == other.sortOrder &&
        subtotalCost == other.subtotalCost &&
        subtotalSellingPrice == other.subtotalSellingPrice &&
        workItems == other.workItems;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, estimateRevisionId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, subtotalCost.hashCode);
    _$hash = $jc(_$hash, subtotalSellingPrice.hashCode);
    _$hash = $jc(_$hash, workItems.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateSectionResponse')
          ..add('id', id)
          ..add('estimateRevisionId', estimateRevisionId)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('nameEn', nameEn)
          ..add('sortOrder', sortOrder)
          ..add('subtotalCost', subtotalCost)
          ..add('subtotalSellingPrice', subtotalSellingPrice)
          ..add('workItems', workItems))
        .toString();
  }
}

class EstimateSectionResponseBuilder
    implements
        Builder<EstimateSectionResponse, EstimateSectionResponseBuilder> {
  _$EstimateSectionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _estimateRevisionId;
  String? get estimateRevisionId => _$this._estimateRevisionId;
  set estimateRevisionId(String? estimateRevisionId) =>
      _$this._estimateRevisionId = estimateRevisionId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _nameTh;
  String? get nameTh => _$this._nameTh;
  set nameTh(String? nameTh) => _$this._nameTh = nameTh;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  double? _subtotalCost;
  double? get subtotalCost => _$this._subtotalCost;
  set subtotalCost(double? subtotalCost) => _$this._subtotalCost = subtotalCost;

  double? _subtotalSellingPrice;
  double? get subtotalSellingPrice => _$this._subtotalSellingPrice;
  set subtotalSellingPrice(double? subtotalSellingPrice) =>
      _$this._subtotalSellingPrice = subtotalSellingPrice;

  ListBuilder<EstimateWorkItemResponse>? _workItems;
  ListBuilder<EstimateWorkItemResponse> get workItems =>
      _$this._workItems ??= ListBuilder<EstimateWorkItemResponse>();
  set workItems(ListBuilder<EstimateWorkItemResponse>? workItems) =>
      _$this._workItems = workItems;

  EstimateSectionResponseBuilder() {
    EstimateSectionResponse._defaults(this);
  }

  EstimateSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _estimateRevisionId = $v.estimateRevisionId;
      _code = $v.code;
      _nameTh = $v.nameTh;
      _nameEn = $v.nameEn;
      _sortOrder = $v.sortOrder;
      _subtotalCost = $v.subtotalCost;
      _subtotalSellingPrice = $v.subtotalSellingPrice;
      _workItems = $v.workItems?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateSectionResponse other) {
    _$v = other as _$EstimateSectionResponse;
  }

  @override
  void update(void Function(EstimateSectionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateSectionResponse build() => _build();

  _$EstimateSectionResponse _build() {
    _$EstimateSectionResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateSectionResponse._(
            id: id,
            estimateRevisionId: estimateRevisionId,
            code: code,
            nameTh: nameTh,
            nameEn: nameEn,
            sortOrder: sortOrder,
            subtotalCost: subtotalCost,
            subtotalSellingPrice: subtotalSellingPrice,
            workItems: _workItems?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workItems';
        _workItems?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateSectionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
