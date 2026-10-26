// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_localized_label.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyLocalizedLabel extends SurveyLocalizedLabel {
  @override
  final String? thai;
  @override
  final String? english;

  factory _$SurveyLocalizedLabel(
          [void Function(SurveyLocalizedLabelBuilder)? updates]) =>
      (SurveyLocalizedLabelBuilder()..update(updates))._build();

  _$SurveyLocalizedLabel._({this.thai, this.english}) : super._();
  @override
  SurveyLocalizedLabel rebuild(
          void Function(SurveyLocalizedLabelBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyLocalizedLabelBuilder toBuilder() =>
      SurveyLocalizedLabelBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyLocalizedLabel &&
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
    return (newBuiltValueToStringHelper(r'SurveyLocalizedLabel')
          ..add('thai', thai)
          ..add('english', english))
        .toString();
  }
}

class SurveyLocalizedLabelBuilder
    implements Builder<SurveyLocalizedLabel, SurveyLocalizedLabelBuilder> {
  _$SurveyLocalizedLabel? _$v;

  String? _thai;
  String? get thai => _$this._thai;
  set thai(String? thai) => _$this._thai = thai;

  String? _english;
  String? get english => _$this._english;
  set english(String? english) => _$this._english = english;

  SurveyLocalizedLabelBuilder() {
    SurveyLocalizedLabel._defaults(this);
  }

  SurveyLocalizedLabelBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _thai = $v.thai;
      _english = $v.english;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyLocalizedLabel other) {
    _$v = other as _$SurveyLocalizedLabel;
  }

  @override
  void update(void Function(SurveyLocalizedLabelBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyLocalizedLabel build() => _build();

  _$SurveyLocalizedLabel _build() {
    final _$result = _$v ??
        _$SurveyLocalizedLabel._(
          thai: thai,
          english: english,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
