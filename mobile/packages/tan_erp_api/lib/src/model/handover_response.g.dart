// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'handover_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$HandoverResponse extends HandoverResponse {
  @override
  final Date? date;
  @override
  final String? signerName;
  @override
  final String? note;
  @override
  final int? warrantyMonths;

  factory _$HandoverResponse(
          [void Function(HandoverResponseBuilder)? updates]) =>
      (HandoverResponseBuilder()..update(updates))._build();

  _$HandoverResponse._(
      {this.date, this.signerName, this.note, this.warrantyMonths})
      : super._();
  @override
  HandoverResponse rebuild(void Function(HandoverResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  HandoverResponseBuilder toBuilder() =>
      HandoverResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is HandoverResponse &&
        date == other.date &&
        signerName == other.signerName &&
        note == other.note &&
        warrantyMonths == other.warrantyMonths;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, date.hashCode);
    _$hash = $jc(_$hash, signerName.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, warrantyMonths.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'HandoverResponse')
          ..add('date', date)
          ..add('signerName', signerName)
          ..add('note', note)
          ..add('warrantyMonths', warrantyMonths))
        .toString();
  }
}

class HandoverResponseBuilder
    implements Builder<HandoverResponse, HandoverResponseBuilder> {
  _$HandoverResponse? _$v;

  Date? _date;
  Date? get date => _$this._date;
  set date(Date? date) => _$this._date = date;

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

  HandoverResponseBuilder() {
    HandoverResponse._defaults(this);
  }

  HandoverResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _date = $v.date;
      _signerName = $v.signerName;
      _note = $v.note;
      _warrantyMonths = $v.warrantyMonths;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(HandoverResponse other) {
    _$v = other as _$HandoverResponse;
  }

  @override
  void update(void Function(HandoverResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  HandoverResponse build() => _build();

  _$HandoverResponse _build() {
    final _$result = _$v ??
        _$HandoverResponse._(
          date: date,
          signerName: signerName,
          note: note,
          warrantyMonths: warrantyMonths,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
