// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_action_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestActionRequest extends PurchaseRequestActionRequest {
  @override
  final String? note;

  factory _$PurchaseRequestActionRequest(
          [void Function(PurchaseRequestActionRequestBuilder)? updates]) =>
      (PurchaseRequestActionRequestBuilder()..update(updates))._build();

  _$PurchaseRequestActionRequest._({this.note}) : super._();
  @override
  PurchaseRequestActionRequest rebuild(
          void Function(PurchaseRequestActionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestActionRequestBuilder toBuilder() =>
      PurchaseRequestActionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestActionRequest && note == other.note;
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
    return (newBuiltValueToStringHelper(r'PurchaseRequestActionRequest')
          ..add('note', note))
        .toString();
  }
}

class PurchaseRequestActionRequestBuilder
    implements
        Builder<PurchaseRequestActionRequest,
            PurchaseRequestActionRequestBuilder> {
  _$PurchaseRequestActionRequest? _$v;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  PurchaseRequestActionRequestBuilder() {
    PurchaseRequestActionRequest._defaults(this);
  }

  PurchaseRequestActionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestActionRequest other) {
    _$v = other as _$PurchaseRequestActionRequest;
  }

  @override
  void update(void Function(PurchaseRequestActionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestActionRequest build() => _build();

  _$PurchaseRequestActionRequest _build() {
    final _$result = _$v ??
        _$PurchaseRequestActionRequest._(
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
