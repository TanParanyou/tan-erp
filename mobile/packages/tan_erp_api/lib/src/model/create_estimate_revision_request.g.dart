// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_estimate_revision_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateEstimateRevisionRequest extends CreateEstimateRevisionRequest {
  @override
  final String? reason;

  factory _$CreateEstimateRevisionRequest(
          [void Function(CreateEstimateRevisionRequestBuilder)? updates]) =>
      (CreateEstimateRevisionRequestBuilder()..update(updates))._build();

  _$CreateEstimateRevisionRequest._({this.reason}) : super._();
  @override
  CreateEstimateRevisionRequest rebuild(
          void Function(CreateEstimateRevisionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateEstimateRevisionRequestBuilder toBuilder() =>
      CreateEstimateRevisionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateEstimateRevisionRequest && reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateEstimateRevisionRequest')
          ..add('reason', reason))
        .toString();
  }
}

class CreateEstimateRevisionRequestBuilder
    implements
        Builder<CreateEstimateRevisionRequest,
            CreateEstimateRevisionRequestBuilder> {
  _$CreateEstimateRevisionRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  CreateEstimateRevisionRequestBuilder() {
    CreateEstimateRevisionRequest._defaults(this);
  }

  CreateEstimateRevisionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateEstimateRevisionRequest other) {
    _$v = other as _$CreateEstimateRevisionRequest;
  }

  @override
  void update(void Function(CreateEstimateRevisionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateEstimateRevisionRequest build() => _build();

  _$CreateEstimateRevisionRequest _build() {
    final _$result = _$v ??
        _$CreateEstimateRevisionRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
