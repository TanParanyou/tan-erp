// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_localized_name_contract.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyLocalizedNameContract extends SurveyLocalizedNameContract {
  @override
  final String? thai;
  @override
  final String? english;

  factory _$SurveyLocalizedNameContract(
          [void Function(SurveyLocalizedNameContractBuilder)? updates]) =>
      (SurveyLocalizedNameContractBuilder()..update(updates))._build();

  _$SurveyLocalizedNameContract._({this.thai, this.english}) : super._();
  @override
  SurveyLocalizedNameContract rebuild(
          void Function(SurveyLocalizedNameContractBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyLocalizedNameContractBuilder toBuilder() =>
      SurveyLocalizedNameContractBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyLocalizedNameContract &&
        thai == other.thai &&
        english == other.english;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, thai.hashCode);
    _$hash = $jc(_$hash, english.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyLocalizedNameContract')
          ..add('thai', thai)
          ..add('english', english))
        .toString();
  }
}

class SurveyLocalizedNameContractBuilder
    implements
        Builder<SurveyLocalizedNameContract,
            SurveyLocalizedNameContractBuilder> {
  _$SurveyLocalizedNameContract? _$v;

  String? _thai;
  String? get thai => _$this._thai;
  set thai(String? thai) => _$this._thai = thai;

  String? _english;
  String? get english => _$this._english;
  set english(String? english) => _$this._english = english;

  SurveyLocalizedNameContractBuilder() {
    SurveyLocalizedNameContract._defaults(this);
  }

  SurveyLocalizedNameContractBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _thai = $v.thai;
      _english = $v.english;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyLocalizedNameContract other) {
    _$v = other as _$SurveyLocalizedNameContract;
  }

  @override
  void update(void Function(SurveyLocalizedNameContractBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyLocalizedNameContract build() => _build();

  _$SurveyLocalizedNameContract _build() {
    final _$result = _$v ??
        _$SurveyLocalizedNameContract._(
          thai: thai,
          english: english,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
