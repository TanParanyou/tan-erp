// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_action_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderActionRequest extends PurchaseOrderActionRequest {
  @override
  final String? note;

  factory _$PurchaseOrderActionRequest(
          [void Function(PurchaseOrderActionRequestBuilder)? updates]) =>
      (PurchaseOrderActionRequestBuilder()..update(updates))._build();

  _$PurchaseOrderActionRequest._({this.note}) : super._();
  @override
  PurchaseOrderActionRequest rebuild(
          void Function(PurchaseOrderActionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderActionRequestBuilder toBuilder() =>
      PurchaseOrderActionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderActionRequest && note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderActionRequest')
          ..add('note', note))
        .toString();
  }
}

class PurchaseOrderActionRequestBuilder
    implements
        Builder<PurchaseOrderActionRequest, PurchaseOrderActionRequestBuilder> {
  _$PurchaseOrderActionRequest? _$v;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  PurchaseOrderActionRequestBuilder() {
    PurchaseOrderActionRequest._defaults(this);
  }

  PurchaseOrderActionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderActionRequest other) {
    _$v = other as _$PurchaseOrderActionRequest;
  }

  @override
  void update(void Function(PurchaseOrderActionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderActionRequest build() => _build();

  _$PurchaseOrderActionRequest _build() {
    final _$result = _$v ??
        _$PurchaseOrderActionRequest._(
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
