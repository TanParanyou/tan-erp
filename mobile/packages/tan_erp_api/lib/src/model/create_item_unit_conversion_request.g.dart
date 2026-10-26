// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_item_unit_conversion_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateItemUnitConversionRequest
    extends CreateItemUnitConversionRequest {
  @override
  final String fromUnitId;
  @override
  final String toUnitId;
  @override
  final String factor;
  @override
  final Date effectiveFrom;
  @override
  final Date? effectiveTo;
  @override
  final String reason;

  factory _$CreateItemUnitConversionRequest(
          [void Function(CreateItemUnitConversionRequestBuilder)? updates]) =>
      (CreateItemUnitConversionRequestBuilder()..update(updates))._build();

  _$CreateItemUnitConversionRequest._(
      {required this.fromUnitId,
      required this.toUnitId,
      required this.factor,
      required this.effectiveFrom,
      this.effectiveTo,
      required this.reason})
      : super._();
  @override
  CreateItemUnitConversionRequest rebuild(
          void Function(CreateItemUnitConversionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateItemUnitConversionRequestBuilder toBuilder() =>
      CreateItemUnitConversionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateItemUnitConversionRequest &&
        fromUnitId == other.fromUnitId &&
        toUnitId == other.toUnitId &&
        factor == other.factor &&
        effectiveFrom == other.effectiveFrom &&
        effectiveTo == other.effectiveTo &&
        reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fromUnitId.hashCode);
    _$hash = $jc(_$hash, toUnitId.hashCode);
    _$hash = $jc(_$hash, factor.hashCode);
    _$hash = $jc(_$hash, effectiveFrom.hashCode);
    _$hash = $jc(_$hash, effectiveTo.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateItemUnitConversionRequest')
          ..add('fromUnitId', fromUnitId)
          ..add('toUnitId', toUnitId)
          ..add('factor', factor)
          ..add('effectiveFrom', effectiveFrom)
          ..add('effectiveTo', effectiveTo)
          ..add('reason', reason))
        .toString();
  }
}

class CreateItemUnitConversionRequestBuilder
    implements
        Builder<CreateItemUnitConversionRequest,
            CreateItemUnitConversionRequestBuilder> {
  _$CreateItemUnitConversionRequest? _$v;

  String? _fromUnitId;
  String? get fromUnitId => _$this._fromUnitId;
  set fromUnitId(String? fromUnitId) => _$this._fromUnitId = fromUnitId;

  String? _toUnitId;
  String? get toUnitId => _$this._toUnitId;
  set toUnitId(String? toUnitId) => _$this._toUnitId = toUnitId;

  String? _factor;
  String? get factor => _$this._factor;
  set factor(String? factor) => _$this._factor = factor;

  Date? _effectiveFrom;
  Date? get effectiveFrom => _$this._effectiveFrom;
  set effectiveFrom(Date? effectiveFrom) =>
      _$this._effectiveFrom = effectiveFrom;

  Date? _effectiveTo;
  Date? get effectiveTo => _$this._effectiveTo;
  set effectiveTo(Date? effectiveTo) => _$this._effectiveTo = effectiveTo;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  CreateItemUnitConversionRequestBuilder() {
    CreateItemUnitConversionRequest._defaults(this);
  }

  CreateItemUnitConversionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fromUnitId = $v.fromUnitId;
      _toUnitId = $v.toUnitId;
      _factor = $v.factor;
      _effectiveFrom = $v.effectiveFrom;
      _effectiveTo = $v.effectiveTo;
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateItemUnitConversionRequest other) {
    _$v = other as _$CreateItemUnitConversionRequest;
  }

  @override
  void update(void Function(CreateItemUnitConversionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateItemUnitConversionRequest build() => _build();

  _$CreateItemUnitConversionRequest _build() {
    final _$result = _$v ??
        _$CreateItemUnitConversionRequest._(
          fromUnitId: BuiltValueNullFieldError.checkNotNull(
              fromUnitId, r'CreateItemUnitConversionRequest', 'fromUnitId'),
          toUnitId: BuiltValueNullFieldError.checkNotNull(
              toUnitId, r'CreateItemUnitConversionRequest', 'toUnitId'),
          factor: BuiltValueNullFieldError.checkNotNull(
              factor, r'CreateItemUnitConversionRequest', 'factor'),
          effectiveFrom: BuiltValueNullFieldError.checkNotNull(effectiveFrom,
              r'CreateItemUnitConversionRequest', 'effectiveFrom'),
          effectiveTo: effectiveTo,
          reason: BuiltValueNullFieldError.checkNotNull(
              reason, r'CreateItemUnitConversionRequest', 'reason'),
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
