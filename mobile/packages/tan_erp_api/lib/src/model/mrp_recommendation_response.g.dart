// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_recommendation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpRecommendationResponse extends MrpRecommendationResponse {
  @override
  final String? id;
  @override
  final int? lineNo;
  @override
  final MrpItemResponse? item;
  @override
  final String? action;
  @override
  final double? quantity;
  @override
  final Date? needBy;
  @override
  final Date? orderBy;
  @override
  final int? level;
  @override
  final double? grossRequirement;
  @override
  final double? stockUsed;
  @override
  final double? scheduledReceiptsUsed;
  @override
  final BuiltList<MrpReasonResponse>? reasons;
  @override
  final String? status;
  @override
  final MrpPersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final MrpConvertedResponse? converted;
  @override
  final String? rowVersion;

  factory _$MrpRecommendationResponse(
          [void Function(MrpRecommendationResponseBuilder)? updates]) =>
      (MrpRecommendationResponseBuilder()..update(updates))._build();

  _$MrpRecommendationResponse._(
      {this.id,
      this.lineNo,
      this.item,
      this.action,
      this.quantity,
      this.needBy,
      this.orderBy,
      this.level,
      this.grossRequirement,
      this.stockUsed,
      this.scheduledReceiptsUsed,
      this.reasons,
      this.status,
      this.decidedBy,
      this.decidedAtUtc,
      this.converted,
      this.rowVersion})
      : super._();
  @override
  MrpRecommendationResponse rebuild(
          void Function(MrpRecommendationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpRecommendationResponseBuilder toBuilder() =>
      MrpRecommendationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpRecommendationResponse &&
        id == other.id &&
        lineNo == other.lineNo &&
        item == other.item &&
        action == other.action &&
        quantity == other.quantity &&
        needBy == other.needBy &&
        orderBy == other.orderBy &&
        level == other.level &&
        grossRequirement == other.grossRequirement &&
        stockUsed == other.stockUsed &&
        scheduledReceiptsUsed == other.scheduledReceiptsUsed &&
        reasons == other.reasons &&
        status == other.status &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        converted == other.converted &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, lineNo.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, action.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, needBy.hashCode);
    _$hash = $jc(_$hash, orderBy.hashCode);
    _$hash = $jc(_$hash, level.hashCode);
    _$hash = $jc(_$hash, grossRequirement.hashCode);
    _$hash = $jc(_$hash, stockUsed.hashCode);
    _$hash = $jc(_$hash, scheduledReceiptsUsed.hashCode);
    _$hash = $jc(_$hash, reasons.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, converted.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpRecommendationResponse')
          ..add('id', id)
          ..add('lineNo', lineNo)
          ..add('item', item)
          ..add('action', action)
          ..add('quantity', quantity)
          ..add('needBy', needBy)
          ..add('orderBy', orderBy)
          ..add('level', level)
          ..add('grossRequirement', grossRequirement)
          ..add('stockUsed', stockUsed)
          ..add('scheduledReceiptsUsed', scheduledReceiptsUsed)
          ..add('reasons', reasons)
          ..add('status', status)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('converted', converted)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class MrpRecommendationResponseBuilder
    implements
        Builder<MrpRecommendationResponse, MrpRecommendationResponseBuilder> {
  _$MrpRecommendationResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _lineNo;
  int? get lineNo => _$this._lineNo;
  set lineNo(int? lineNo) => _$this._lineNo = lineNo;

  MrpItemResponseBuilder? _item;
  MrpItemResponseBuilder get item => _$this._item ??= MrpItemResponseBuilder();
  set item(MrpItemResponseBuilder? item) => _$this._item = item;

  String? _action;
  String? get action => _$this._action;
  set action(String? action) => _$this._action = action;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  Date? _needBy;
  Date? get needBy => _$this._needBy;
  set needBy(Date? needBy) => _$this._needBy = needBy;

  Date? _orderBy;
  Date? get orderBy => _$this._orderBy;
  set orderBy(Date? orderBy) => _$this._orderBy = orderBy;

  int? _level;
  int? get level => _$this._level;
  set level(int? level) => _$this._level = level;

  double? _grossRequirement;
  double? get grossRequirement => _$this._grossRequirement;
  set grossRequirement(double? grossRequirement) =>
      _$this._grossRequirement = grossRequirement;

  double? _stockUsed;
  double? get stockUsed => _$this._stockUsed;
  set stockUsed(double? stockUsed) => _$this._stockUsed = stockUsed;

  double? _scheduledReceiptsUsed;
  double? get scheduledReceiptsUsed => _$this._scheduledReceiptsUsed;
  set scheduledReceiptsUsed(double? scheduledReceiptsUsed) =>
      _$this._scheduledReceiptsUsed = scheduledReceiptsUsed;

  ListBuilder<MrpReasonResponse>? _reasons;
  ListBuilder<MrpReasonResponse> get reasons =>
      _$this._reasons ??= ListBuilder<MrpReasonResponse>();
  set reasons(ListBuilder<MrpReasonResponse>? reasons) =>
      _$this._reasons = reasons;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  MrpPersonResponseBuilder? _decidedBy;
  MrpPersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= MrpPersonResponseBuilder();
  set decidedBy(MrpPersonResponseBuilder? decidedBy) =>
      _$this._decidedBy = decidedBy;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  MrpConvertedResponseBuilder? _converted;
  MrpConvertedResponseBuilder get converted =>
      _$this._converted ??= MrpConvertedResponseBuilder();
  set converted(MrpConvertedResponseBuilder? converted) =>
      _$this._converted = converted;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  MrpRecommendationResponseBuilder() {
    MrpRecommendationResponse._defaults(this);
  }

  MrpRecommendationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _lineNo = $v.lineNo;
      _item = $v.item?.toBuilder();
      _action = $v.action;
      _quantity = $v.quantity;
      _needBy = $v.needBy;
      _orderBy = $v.orderBy;
      _level = $v.level;
      _grossRequirement = $v.grossRequirement;
      _stockUsed = $v.stockUsed;
      _scheduledReceiptsUsed = $v.scheduledReceiptsUsed;
      _reasons = $v.reasons?.toBuilder();
      _status = $v.status;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _converted = $v.converted?.toBuilder();
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpRecommendationResponse other) {
    _$v = other as _$MrpRecommendationResponse;
  }

  @override
  void update(void Function(MrpRecommendationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpRecommendationResponse build() => _build();

  _$MrpRecommendationResponse _build() {
    _$MrpRecommendationResponse _$result;
    try {
      _$result = _$v ??
          _$MrpRecommendationResponse._(
            id: id,
            lineNo: lineNo,
            item: _item?.build(),
            action: action,
            quantity: quantity,
            needBy: needBy,
            orderBy: orderBy,
            level: level,
            grossRequirement: grossRequirement,
            stockUsed: stockUsed,
            scheduledReceiptsUsed: scheduledReceiptsUsed,
            reasons: _reasons?.build(),
            status: status,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            converted: _converted?.build(),
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();

        _$failedField = 'reasons';
        _reasons?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();

        _$failedField = 'converted';
        _converted?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'MrpRecommendationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
