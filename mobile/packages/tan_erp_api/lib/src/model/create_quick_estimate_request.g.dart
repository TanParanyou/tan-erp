// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_quick_estimate_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateQuickEstimateRequest extends CreateQuickEstimateRequest {
  @override
  final String? customerId;
  @override
  final String? opportunityId;

  factory _$CreateQuickEstimateRequest(
          [void Function(CreateQuickEstimateRequestBuilder)? updates]) =>
      (CreateQuickEstimateRequestBuilder()..update(updates))._build();

  _$CreateQuickEstimateRequest._({this.customerId, this.opportunityId})
      : super._();
  @override
  CreateQuickEstimateRequest rebuild(
          void Function(CreateQuickEstimateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateQuickEstimateRequestBuilder toBuilder() =>
      CreateQuickEstimateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateQuickEstimateRequest &&
        customerId == other.customerId &&
        opportunityId == other.opportunityId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateQuickEstimateRequest')
          ..add('customerId', customerId)
          ..add('opportunityId', opportunityId))
        .toString();
  }
}

class CreateQuickEstimateRequestBuilder
    implements
        Builder<CreateQuickEstimateRequest, CreateQuickEstimateRequestBuilder> {
  _$CreateQuickEstimateRequest? _$v;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  CreateQuickEstimateRequestBuilder() {
    CreateQuickEstimateRequest._defaults(this);
  }

  CreateQuickEstimateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _customerId = $v.customerId;
      _opportunityId = $v.opportunityId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateQuickEstimateRequest other) {
    _$v = other as _$CreateQuickEstimateRequest;
  }

  @override
  void update(void Function(CreateQuickEstimateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateQuickEstimateRequest build() => _build();

  _$CreateQuickEstimateRequest _build() {
    final _$result = _$v ??
        _$CreateQuickEstimateRequest._(
          customerId: customerId,
          opportunityId: opportunityId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
