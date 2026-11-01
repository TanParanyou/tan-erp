// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_note_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingNoteRequest extends BillingNoteRequest {
  @override
  final String? noteType;
  @override
  final String? reason;
  @override
  final double? taxBaseDelta;

  factory _$BillingNoteRequest(
          [void Function(BillingNoteRequestBuilder)? updates]) =>
      (BillingNoteRequestBuilder()..update(updates))._build();

  _$BillingNoteRequest._({this.noteType, this.reason, this.taxBaseDelta})
      : super._();
  @override
  BillingNoteRequest rebuild(
          void Function(BillingNoteRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingNoteRequestBuilder toBuilder() =>
      BillingNoteRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingNoteRequest &&
        noteType == other.noteType &&
        reason == other.reason &&
        taxBaseDelta == other.taxBaseDelta;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, noteType.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, taxBaseDelta.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BillingNoteRequest')
          ..add('noteType', noteType)
          ..add('reason', reason)
          ..add('taxBaseDelta', taxBaseDelta))
        .toString();
  }
}

class BillingNoteRequestBuilder
    implements Builder<BillingNoteRequest, BillingNoteRequestBuilder> {
  _$BillingNoteRequest? _$v;

  String? _noteType;
  String? get noteType => _$this._noteType;
  set noteType(String? noteType) => _$this._noteType = noteType;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  double? _taxBaseDelta;
  double? get taxBaseDelta => _$this._taxBaseDelta;
  set taxBaseDelta(double? taxBaseDelta) => _$this._taxBaseDelta = taxBaseDelta;

  BillingNoteRequestBuilder() {
    BillingNoteRequest._defaults(this);
  }

  BillingNoteRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _noteType = $v.noteType;
      _reason = $v.reason;
      _taxBaseDelta = $v.taxBaseDelta;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingNoteRequest other) {
    _$v = other as _$BillingNoteRequest;
  }

  @override
  void update(void Function(BillingNoteRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingNoteRequest build() => _build();

  _$BillingNoteRequest _build() {
    final _$result = _$v ??
        _$BillingNoteRequest._(
          noteType: noteType,
          reason: reason,
          taxBaseDelta: taxBaseDelta,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
