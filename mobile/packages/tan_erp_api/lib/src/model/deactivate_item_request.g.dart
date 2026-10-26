// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deactivate_item_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DeactivateItemRequest extends DeactivateItemRequest {
  @override
  final String reasonCode;
  @override
  final String? reason;

  factory _$DeactivateItemRequest(
          [void Function(DeactivateItemRequestBuilder)? updates]) =>
      (DeactivateItemRequestBuilder()..update(updates))._build();

  _$DeactivateItemRequest._({required this.reasonCode, this.reason})
      : super._();
  @override
  DeactivateItemRequest rebuild(
          void Function(DeactivateItemRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DeactivateItemRequestBuilder toBuilder() =>
      DeactivateItemRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DeactivateItemRequest &&
        reasonCode == other.reasonCode &&
        reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DeactivateItemRequest')
          ..add('reasonCode', reasonCode)
          ..add('reason', reason))
        .toString();
  }
}

class DeactivateItemRequestBuilder
    implements Builder<DeactivateItemRequest, DeactivateItemRequestBuilder> {
  _$DeactivateItemRequest? _$v;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  DeactivateItemRequestBuilder() {
    DeactivateItemRequest._defaults(this);
  }

  DeactivateItemRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reasonCode = $v.reasonCode;
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DeactivateItemRequest other) {
    _$v = other as _$DeactivateItemRequest;
  }

  @override
  void update(void Function(DeactivateItemRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DeactivateItemRequest build() => _build();

  _$DeactivateItemRequest _build() {
    final _$result = _$v ??
        _$DeactivateItemRequest._(
          reasonCode: BuiltValueNullFieldError.checkNotNull(
              reasonCode, r'DeactivateItemRequest', 'reasonCode'),
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
