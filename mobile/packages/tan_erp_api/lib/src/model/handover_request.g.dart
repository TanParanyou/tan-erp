// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'handover_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$HandoverRequest extends HandoverRequest {
  @override
  final String? outcome;
  @override
  final String? signerName;
  @override
  final String? note;
  @override
  final int? warrantyMonths;
  @override
  final Date? handoverDate;

  factory _$HandoverRequest([void Function(HandoverRequestBuilder)? updates]) =>
      (HandoverRequestBuilder()..update(updates))._build();

  _$HandoverRequest._(
      {this.outcome,
      this.signerName,
      this.note,
      this.warrantyMonths,
      this.handoverDate})
      : super._();
  @override
  HandoverRequest rebuild(void Function(HandoverRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  HandoverRequestBuilder toBuilder() => HandoverRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is HandoverRequest &&
        outcome == other.outcome &&
        signerName == other.signerName &&
        note == other.note &&
        warrantyMonths == other.warrantyMonths &&
        handoverDate == other.handoverDate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, outcome.hashCode);
    _$hash = $jc(_$hash, signerName.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, warrantyMonths.hashCode);
    _$hash = $jc(_$hash, handoverDate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'HandoverRequest')
          ..add('outcome', outcome)
          ..add('signerName', signerName)
          ..add('note', note)
          ..add('warrantyMonths', warrantyMonths)
          ..add('handoverDate', handoverDate))
        .toString();
  }
}

class HandoverRequestBuilder
    implements Builder<HandoverRequest, HandoverRequestBuilder> {
  _$HandoverRequest? _$v;

  String? _outcome;
  String? get outcome => _$this._outcome;
  set outcome(String? outcome) => _$this._outcome = outcome;

  String? _signerName;
  String? get signerName => _$this._signerName;
  set signerName(String? signerName) => _$this._signerName = signerName;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  int? _warrantyMonths;
  int? get warrantyMonths => _$this._warrantyMonths;
  set warrantyMonths(int? warrantyMonths) =>
      _$this._warrantyMonths = warrantyMonths;

  Date? _handoverDate;
  Date? get handoverDate => _$this._handoverDate;
  set handoverDate(Date? handoverDate) => _$this._handoverDate = handoverDate;

  HandoverRequestBuilder() {
    HandoverRequest._defaults(this);
  }

  HandoverRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _outcome = $v.outcome;
      _signerName = $v.signerName;
      _note = $v.note;
      _warrantyMonths = $v.warrantyMonths;
      _handoverDate = $v.handoverDate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(HandoverRequest other) {
    _$v = other as _$HandoverRequest;
  }

  @override
  void update(void Function(HandoverRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  HandoverRequest build() => _build();

  _$HandoverRequest _build() {
    final _$result = _$v ??
        _$HandoverRequest._(
          outcome: outcome,
          signerName: signerName,
          note: note,
          warrantyMonths: warrantyMonths,
          handoverDate: handoverDate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
