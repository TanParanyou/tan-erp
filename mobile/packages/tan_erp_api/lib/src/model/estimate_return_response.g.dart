// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_return_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReturnResponse extends EstimateReturnResponse {
  @override
  final String reasonCode;
  @override
  final String note;
  @override
  final DateTime returnedAtUtc;
  @override
  final EstimateReturnReviewerResponse returnedBy;

  factory _$EstimateReturnResponse(
          [void Function(EstimateReturnResponseBuilder)? updates]) =>
      (EstimateReturnResponseBuilder()..update(updates))._build();

  _$EstimateReturnResponse._(
      {required this.reasonCode,
      required this.note,
      required this.returnedAtUtc,
      required this.returnedBy})
      : super._();
  @override
  EstimateReturnResponse rebuild(
          void Function(EstimateReturnResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReturnResponseBuilder toBuilder() =>
      EstimateReturnResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReturnResponse &&
        reasonCode == other.reasonCode &&
        note == other.note &&
        returnedAtUtc == other.returnedAtUtc &&
        returnedBy == other.returnedBy;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, returnedAtUtc.hashCode);
    _$hash = $jc(_$hash, returnedBy.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReturnResponse')
          ..add('reasonCode', reasonCode)
          ..add('note', note)
          ..add('returnedAtUtc', returnedAtUtc)
          ..add('returnedBy', returnedBy))
        .toString();
  }
}

class EstimateReturnResponseBuilder
    implements Builder<EstimateReturnResponse, EstimateReturnResponseBuilder> {
  _$EstimateReturnResponse? _$v;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  DateTime? _returnedAtUtc;
  DateTime? get returnedAtUtc => _$this._returnedAtUtc;
  set returnedAtUtc(DateTime? returnedAtUtc) =>
      _$this._returnedAtUtc = returnedAtUtc;

  EstimateReturnReviewerResponseBuilder? _returnedBy;
  EstimateReturnReviewerResponseBuilder get returnedBy =>
      _$this._returnedBy ??= EstimateReturnReviewerResponseBuilder();
  set returnedBy(EstimateReturnReviewerResponseBuilder? returnedBy) =>
      _$this._returnedBy = returnedBy;

  EstimateReturnResponseBuilder() {
    EstimateReturnResponse._defaults(this);
  }

  EstimateReturnResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reasonCode = $v.reasonCode;
      _note = $v.note;
      _returnedAtUtc = $v.returnedAtUtc;
      _returnedBy = $v.returnedBy.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReturnResponse other) {
    _$v = other as _$EstimateReturnResponse;
  }

  @override
  void update(void Function(EstimateReturnResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReturnResponse build() => _build();

  _$EstimateReturnResponse _build() {
    _$EstimateReturnResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateReturnResponse._(
            reasonCode: BuiltValueNullFieldError.checkNotNull(
                reasonCode, r'EstimateReturnResponse', 'reasonCode'),
            note: BuiltValueNullFieldError.checkNotNull(
                note, r'EstimateReturnResponse', 'note'),
            returnedAtUtc: BuiltValueNullFieldError.checkNotNull(
                returnedAtUtc, r'EstimateReturnResponse', 'returnedAtUtc'),
            returnedBy: returnedBy.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'returnedBy';
        returnedBy.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateReturnResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
