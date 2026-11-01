// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'charge_service_request_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ChargeServiceRequestRequest extends ChargeServiceRequestRequest {
  @override
  final double? amount;
  @override
  final String? note;

  factory _$ChargeServiceRequestRequest(
          [void Function(ChargeServiceRequestRequestBuilder)? updates]) =>
      (ChargeServiceRequestRequestBuilder()..update(updates))._build();

  _$ChargeServiceRequestRequest._({this.amount, this.note}) : super._();
  @override
  ChargeServiceRequestRequest rebuild(
          void Function(ChargeServiceRequestRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ChargeServiceRequestRequestBuilder toBuilder() =>
      ChargeServiceRequestRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ChargeServiceRequestRequest &&
        amount == other.amount &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ChargeServiceRequestRequest')
          ..add('amount', amount)
          ..add('note', note))
        .toString();
  }
}

class ChargeServiceRequestRequestBuilder
    implements
        Builder<ChargeServiceRequestRequest,
            ChargeServiceRequestRequestBuilder> {
  _$ChargeServiceRequestRequest? _$v;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ChargeServiceRequestRequestBuilder() {
    ChargeServiceRequestRequest._defaults(this);
  }

  ChargeServiceRequestRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _amount = $v.amount;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ChargeServiceRequestRequest other) {
    _$v = other as _$ChargeServiceRequestRequest;
  }

  @override
  void update(void Function(ChargeServiceRequestRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ChargeServiceRequestRequest build() => _build();

  _$ChargeServiceRequestRequest _build() {
    final _$result = _$v ??
        _$ChargeServiceRequestRequest._(
          amount: amount,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
