// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deactivate_customer_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DeactivateCustomerRequest extends DeactivateCustomerRequest {
  @override
  final String? reason;

  factory _$DeactivateCustomerRequest(
          [void Function(DeactivateCustomerRequestBuilder)? updates]) =>
      (DeactivateCustomerRequestBuilder()..update(updates))._build();

  _$DeactivateCustomerRequest._({this.reason}) : super._();
  @override
  DeactivateCustomerRequest rebuild(
          void Function(DeactivateCustomerRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DeactivateCustomerRequestBuilder toBuilder() =>
      DeactivateCustomerRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DeactivateCustomerRequest && reason == other.reason;
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
    return (newBuiltValueToStringHelper(r'DeactivateCustomerRequest')
          ..add('reason', reason))
        .toString();
  }
}

class DeactivateCustomerRequestBuilder
    implements
        Builder<DeactivateCustomerRequest, DeactivateCustomerRequestBuilder> {
  _$DeactivateCustomerRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  DeactivateCustomerRequestBuilder() {
    DeactivateCustomerRequest._defaults(this);
  }

  DeactivateCustomerRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DeactivateCustomerRequest other) {
    _$v = other as _$DeactivateCustomerRequest;
  }

  @override
  void update(void Function(DeactivateCustomerRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DeactivateCustomerRequest build() => _build();

  _$DeactivateCustomerRequest _build() {
    final _$result = _$v ??
        _$DeactivateCustomerRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
