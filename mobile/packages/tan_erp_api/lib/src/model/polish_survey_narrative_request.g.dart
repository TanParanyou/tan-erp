// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'polish_survey_narrative_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PolishSurveyNarrativeRequest extends PolishSurveyNarrativeRequest {
  @override
  final String? text;
  @override
  final String? language;

  factory _$PolishSurveyNarrativeRequest(
          [void Function(PolishSurveyNarrativeRequestBuilder)? updates]) =>
      (PolishSurveyNarrativeRequestBuilder()..update(updates))._build();

  _$PolishSurveyNarrativeRequest._({this.text, this.language}) : super._();
  @override
  PolishSurveyNarrativeRequest rebuild(
          void Function(PolishSurveyNarrativeRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PolishSurveyNarrativeRequestBuilder toBuilder() =>
      PolishSurveyNarrativeRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PolishSurveyNarrativeRequest &&
        text == other.text &&
        language == other.language;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, text.hashCode);
    _$hash = $jc(_$hash, language.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PolishSurveyNarrativeRequest')
          ..add('text', text)
          ..add('language', language))
        .toString();
  }
}

class PolishSurveyNarrativeRequestBuilder
    implements
        Builder<PolishSurveyNarrativeRequest,
            PolishSurveyNarrativeRequestBuilder> {
  _$PolishSurveyNarrativeRequest? _$v;

  String? _text;
  String? get text => _$this._text;
  set text(String? text) => _$this._text = text;

  String? _language;
  String? get language => _$this._language;
  set language(String? language) => _$this._language = language;

  PolishSurveyNarrativeRequestBuilder() {
    PolishSurveyNarrativeRequest._defaults(this);
  }

  PolishSurveyNarrativeRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _text = $v.text;
      _language = $v.language;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PolishSurveyNarrativeRequest other) {
    _$v = other as _$PolishSurveyNarrativeRequest;
  }

  @override
  void update(void Function(PolishSurveyNarrativeRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PolishSurveyNarrativeRequest build() => _build();

  _$PolishSurveyNarrativeRequest _build() {
    final _$result = _$v ??
        _$PolishSurveyNarrativeRequest._(
          text: text,
          language: language,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
