// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'confirm_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ConfirmRequest extends ConfirmRequest {
  @override
  final String? externalRef;
  @override
  final double? externalAmount;

  factory _$ConfirmRequest([void Function(ConfirmRequestBuilder)? updates]) =>
      (ConfirmRequestBuilder()..update(updates))._build();

  _$ConfirmRequest._({this.externalRef, this.externalAmount}) : super._();
  @override
  ConfirmRequest rebuild(void Function(ConfirmRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ConfirmRequestBuilder toBuilder() => ConfirmRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ConfirmRequest &&
        externalRef == other.externalRef &&
        externalAmount == other.externalAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, externalRef.hashCode);
    _$hash = $jc(_$hash, externalAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ConfirmRequest')
          ..add('externalRef', externalRef)
          ..add('externalAmount', externalAmount))
        .toString();
  }
}

class ConfirmRequestBuilder
    implements Builder<ConfirmRequest, ConfirmRequestBuilder> {
  _$ConfirmRequest? _$v;

  String? _externalRef;
  String? get externalRef => _$this._externalRef;
  set externalRef(String? externalRef) => _$this._externalRef = externalRef;

  double? _externalAmount;
  double? get externalAmount => _$this._externalAmount;
  set externalAmount(double? externalAmount) =>
      _$this._externalAmount = externalAmount;

  ConfirmRequestBuilder() {
    ConfirmRequest._defaults(this);
  }

  ConfirmRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _externalRef = $v.externalRef;
      _externalAmount = $v.externalAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ConfirmRequest other) {
    _$v = other as _$ConfirmRequest;
  }

  @override
  void update(void Function(ConfirmRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ConfirmRequest build() => _build();

  _$ConfirmRequest _build() {
    final _$result = _$v ??
        _$ConfirmRequest._(
          externalRef: externalRef,
          externalAmount: externalAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
