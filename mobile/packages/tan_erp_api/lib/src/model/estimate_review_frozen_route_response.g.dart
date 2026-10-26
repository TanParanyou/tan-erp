// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_frozen_route_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewFrozenRouteResponse
    extends EstimateReviewFrozenRouteResponse {
  @override
  final String? policyCode;
  @override
  final int? policyVersion;
  @override
  final String? routeHash;
  @override
  final String? scopeType;
  @override
  final String? scopeId;
  @override
  final String? permissionKey;
  @override
  final EstimateReviewQueueReviewerResponse? reviewer;
  @override
  final BuiltList<EstimateReviewApprovalTriggerResponse>? triggers;
  @override
  final EstimateReviewApprovalThresholdsResponse? thresholds;

  factory _$EstimateReviewFrozenRouteResponse(
          [void Function(EstimateReviewFrozenRouteResponseBuilder)? updates]) =>
      (EstimateReviewFrozenRouteResponseBuilder()..update(updates))._build();

  _$EstimateReviewFrozenRouteResponse._(
      {this.policyCode,
      this.policyVersion,
      this.routeHash,
      this.scopeType,
      this.scopeId,
      this.permissionKey,
      this.reviewer,
      this.triggers,
      this.thresholds})
      : super._();
  @override
  EstimateReviewFrozenRouteResponse rebuild(
          void Function(EstimateReviewFrozenRouteResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewFrozenRouteResponseBuilder toBuilder() =>
      EstimateReviewFrozenRouteResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewFrozenRouteResponse &&
        policyCode == other.policyCode &&
        policyVersion == other.policyVersion &&
        routeHash == other.routeHash &&
        scopeType == other.scopeType &&
        scopeId == other.scopeId &&
        permissionKey == other.permissionKey &&
        reviewer == other.reviewer &&
        triggers == other.triggers &&
        thresholds == other.thresholds;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, policyCode.hashCode);
    _$hash = $jc(_$hash, policyVersion.hashCode);
    _$hash = $jc(_$hash, routeHash.hashCode);
    _$hash = $jc(_$hash, scopeType.hashCode);
    _$hash = $jc(_$hash, scopeId.hashCode);
    _$hash = $jc(_$hash, permissionKey.hashCode);
    _$hash = $jc(_$hash, reviewer.hashCode);
    _$hash = $jc(_$hash, triggers.hashCode);
    _$hash = $jc(_$hash, thresholds.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewFrozenRouteResponse')
          ..add('policyCode', policyCode)
          ..add('policyVersion', policyVersion)
          ..add('routeHash', routeHash)
          ..add('scopeType', scopeType)
          ..add('scopeId', scopeId)
          ..add('permissionKey', permissionKey)
          ..add('reviewer', reviewer)
          ..add('triggers', triggers)
          ..add('thresholds', thresholds))
        .toString();
  }
}

class EstimateReviewFrozenRouteResponseBuilder
    implements
        Builder<EstimateReviewFrozenRouteResponse,
            EstimateReviewFrozenRouteResponseBuilder> {
  _$EstimateReviewFrozenRouteResponse? _$v;

  String? _policyCode;
  String? get policyCode => _$this._policyCode;
  set policyCode(String? policyCode) => _$this._policyCode = policyCode;

  int? _policyVersion;
  int? get policyVersion => _$this._policyVersion;
  set policyVersion(int? policyVersion) =>
      _$this._policyVersion = policyVersion;

  String? _routeHash;
  String? get routeHash => _$this._routeHash;
  set routeHash(String? routeHash) => _$this._routeHash = routeHash;

  String? _scopeType;
  String? get scopeType => _$this._scopeType;
  set scopeType(String? scopeType) => _$this._scopeType = scopeType;

  String? _scopeId;
  String? get scopeId => _$this._scopeId;
  set scopeId(String? scopeId) => _$this._scopeId = scopeId;

  String? _permissionKey;
  String? get permissionKey => _$this._permissionKey;
  set permissionKey(String? permissionKey) =>
      _$this._permissionKey = permissionKey;

  EstimateReviewQueueReviewerResponseBuilder? _reviewer;
  EstimateReviewQueueReviewerResponseBuilder get reviewer =>
      _$this._reviewer ??= EstimateReviewQueueReviewerResponseBuilder();
  set reviewer(EstimateReviewQueueReviewerResponseBuilder? reviewer) =>
      _$this._reviewer = reviewer;

  ListBuilder<EstimateReviewApprovalTriggerResponse>? _triggers;
  ListBuilder<EstimateReviewApprovalTriggerResponse> get triggers =>
      _$this._triggers ??= ListBuilder<EstimateReviewApprovalTriggerResponse>();
  set triggers(ListBuilder<EstimateReviewApprovalTriggerResponse>? triggers) =>
      _$this._triggers = triggers;

  EstimateReviewApprovalThresholdsResponseBuilder? _thresholds;
  EstimateReviewApprovalThresholdsResponseBuilder get thresholds =>
      _$this._thresholds ??= EstimateReviewApprovalThresholdsResponseBuilder();
  set thresholds(EstimateReviewApprovalThresholdsResponseBuilder? thresholds) =>
      _$this._thresholds = thresholds;

  EstimateReviewFrozenRouteResponseBuilder() {
    EstimateReviewFrozenRouteResponse._defaults(this);
  }

  EstimateReviewFrozenRouteResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _policyCode = $v.policyCode;
      _policyVersion = $v.policyVersion;
      _routeHash = $v.routeHash;
      _scopeType = $v.scopeType;
      _scopeId = $v.scopeId;
      _permissionKey = $v.permissionKey;
      _reviewer = $v.reviewer?.toBuilder();
      _triggers = $v.triggers?.toBuilder();
      _thresholds = $v.thresholds?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewFrozenRouteResponse other) {
    _$v = other as _$EstimateReviewFrozenRouteResponse;
  }

  @override
  void update(
      void Function(EstimateReviewFrozenRouteResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewFrozenRouteResponse build() => _build();

  _$EstimateReviewFrozenRouteResponse _build() {
    _$EstimateReviewFrozenRouteResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateReviewFrozenRouteResponse._(
            policyCode: policyCode,
            policyVersion: policyVersion,
            routeHash: routeHash,
            scopeType: scopeType,
            scopeId: scopeId,
            permissionKey: permissionKey,
            reviewer: _reviewer?.build(),
            triggers: _triggers?.build(),
            thresholds: _thresholds?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'reviewer';
        _reviewer?.build();
        _$failedField = 'triggers';
        _triggers?.build();
        _$failedField = 'thresholds';
        _thresholds?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateReviewFrozenRouteResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
