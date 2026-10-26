// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reassign_opportunity_owner_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReassignOpportunityOwnerRequest
    extends ReassignOpportunityOwnerRequest {
  @override
  final String? targetOwnerUserId;
  @override
  final String? expectedVersion;

  factory _$ReassignOpportunityOwnerRequest(
          [void Function(ReassignOpportunityOwnerRequestBuilder)? updates]) =>
      (ReassignOpportunityOwnerRequestBuilder()..update(updates))._build();

  _$ReassignOpportunityOwnerRequest._(
      {this.targetOwnerUserId, this.expectedVersion})
      : super._();
  @override
  ReassignOpportunityOwnerRequest rebuild(
          void Function(ReassignOpportunityOwnerRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReassignOpportunityOwnerRequestBuilder toBuilder() =>
      ReassignOpportunityOwnerRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReassignOpportunityOwnerRequest &&
        targetOwnerUserId == other.targetOwnerUserId &&
        expectedVersion == other.expectedVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, targetOwnerUserId.hashCode);
    _$hash = $jc(_$hash, expectedVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReassignOpportunityOwnerRequest')
          ..add('targetOwnerUserId', targetOwnerUserId)
          ..add('expectedVersion', expectedVersion))
        .toString();
  }
}

class ReassignOpportunityOwnerRequestBuilder
    implements
        Builder<ReassignOpportunityOwnerRequest,
            ReassignOpportunityOwnerRequestBuilder> {
  _$ReassignOpportunityOwnerRequest? _$v;

  String? _targetOwnerUserId;
  String? get targetOwnerUserId => _$this._targetOwnerUserId;
  set targetOwnerUserId(String? targetOwnerUserId) =>
      _$this._targetOwnerUserId = targetOwnerUserId;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  ReassignOpportunityOwnerRequestBuilder() {
    ReassignOpportunityOwnerRequest._defaults(this);
  }

  ReassignOpportunityOwnerRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _targetOwnerUserId = $v.targetOwnerUserId;
      _expectedVersion = $v.expectedVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReassignOpportunityOwnerRequest other) {
    _$v = other as _$ReassignOpportunityOwnerRequest;
  }

  @override
  void update(void Function(ReassignOpportunityOwnerRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReassignOpportunityOwnerRequest build() => _build();

  _$ReassignOpportunityOwnerRequest _build() {
    final _$result = _$v ??
        _$ReassignOpportunityOwnerRequest._(
          targetOwnerUserId: targetOwnerUserId,
          expectedVersion: expectedVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
