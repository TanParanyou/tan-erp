// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_reason_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationReasonRequest extends QuotationReasonRequest {
  @override
  final String? reason;

  factory _$QuotationReasonRequest(
          [void Function(QuotationReasonRequestBuilder)? updates]) =>
      (QuotationReasonRequestBuilder()..update(updates))._build();

  _$QuotationReasonRequest._({this.reason}) : super._();
  @override
  QuotationReasonRequest rebuild(
          void Function(QuotationReasonRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationReasonRequestBuilder toBuilder() =>
      QuotationReasonRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationReasonRequest && reason == other.reason;
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
    return (newBuiltValueToStringHelper(r'QuotationReasonRequest')
          ..add('reason', reason))
        .toString();
  }
}

class QuotationReasonRequestBuilder
    implements Builder<QuotationReasonRequest, QuotationReasonRequestBuilder> {
  _$QuotationReasonRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  QuotationReasonRequestBuilder() {
    QuotationReasonRequest._defaults(this);
  }

  QuotationReasonRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationReasonRequest other) {
    _$v = other as _$QuotationReasonRequest;
  }

  @override
  void update(void Function(QuotationReasonRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationReasonRequest build() => _build();

  _$QuotationReasonRequest _build() {
    final _$result = _$v ??
        _$QuotationReasonRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
