// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_source_request_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderSourceRequestResponse
    extends PurchaseOrderSourceRequestResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final DateTime? convertedAtUtc;

  factory _$PurchaseOrderSourceRequestResponse(
          [void Function(PurchaseOrderSourceRequestResponseBuilder)?
              updates]) =>
      (PurchaseOrderSourceRequestResponseBuilder()..update(updates))._build();

  _$PurchaseOrderSourceRequestResponse._(
      {this.id, this.number, this.convertedAtUtc})
      : super._();
  @override
  PurchaseOrderSourceRequestResponse rebuild(
          void Function(PurchaseOrderSourceRequestResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderSourceRequestResponseBuilder toBuilder() =>
      PurchaseOrderSourceRequestResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderSourceRequestResponse &&
        id == other.id &&
        number == other.number &&
        convertedAtUtc == other.convertedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, convertedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderSourceRequestResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('convertedAtUtc', convertedAtUtc))
        .toString();
  }
}

class PurchaseOrderSourceRequestResponseBuilder
    implements
        Builder<PurchaseOrderSourceRequestResponse,
            PurchaseOrderSourceRequestResponseBuilder> {
  _$PurchaseOrderSourceRequestResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  DateTime? _convertedAtUtc;
  DateTime? get convertedAtUtc => _$this._convertedAtUtc;
  set convertedAtUtc(DateTime? convertedAtUtc) =>
      _$this._convertedAtUtc = convertedAtUtc;

  PurchaseOrderSourceRequestResponseBuilder() {
    PurchaseOrderSourceRequestResponse._defaults(this);
  }

  PurchaseOrderSourceRequestResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _convertedAtUtc = $v.convertedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderSourceRequestResponse other) {
    _$v = other as _$PurchaseOrderSourceRequestResponse;
  }

  @override
  void update(
      void Function(PurchaseOrderSourceRequestResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderSourceRequestResponse build() => _build();

  _$PurchaseOrderSourceRequestResponse _build() {
    final _$result = _$v ??
        _$PurchaseOrderSourceRequestResponse._(
          id: id,
          number: number,
          convertedAtUtc: convertedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
