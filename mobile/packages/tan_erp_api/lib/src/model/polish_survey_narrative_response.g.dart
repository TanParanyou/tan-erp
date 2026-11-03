// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'polish_survey_narrative_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PolishSurveyNarrativeResponse extends PolishSurveyNarrativeResponse {
  @override
  final String? text;
  @override
  final String? provider;

  factory _$PolishSurveyNarrativeResponse(
          [void Function(PolishSurveyNarrativeResponseBuilder)? updates]) =>
      (PolishSurveyNarrativeResponseBuilder()..update(updates))._build();

  _$PolishSurveyNarrativeResponse._({this.text, this.provider}) : super._();
  @override
  PolishSurveyNarrativeResponse rebuild(
          void Function(PolishSurveyNarrativeResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PolishSurveyNarrativeResponseBuilder toBuilder() =>
      PolishSurveyNarrativeResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PolishSurveyNarrativeResponse &&
        text == other.text &&
        provider == other.provider;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, text.hashCode);
    _$hash = $jc(_$hash, provider.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PolishSurveyNarrativeResponse')
          ..add('text', text)
          ..add('provider', provider))
        .toString();
  }
}

class PolishSurveyNarrativeResponseBuilder
    implements
        Builder<PolishSurveyNarrativeResponse,
            PolishSurveyNarrativeResponseBuilder> {
  _$PolishSurveyNarrativeResponse? _$v;

  String? _text;
  String? get text => _$this._text;
  set text(String? text) => _$this._text = text;

  String? _provider;
  String? get provider => _$this._provider;
  set provider(String? provider) => _$this._provider = provider;

  PolishSurveyNarrativeResponseBuilder() {
    PolishSurveyNarrativeResponse._defaults(this);
  }

  PolishSurveyNarrativeResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _text = $v.text;
      _provider = $v.provider;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PolishSurveyNarrativeResponse other) {
    _$v = other as _$PolishSurveyNarrativeResponse;
  }

  @override
  void update(void Function(PolishSurveyNarrativeResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PolishSurveyNarrativeResponse build() => _build();

  _$PolishSurveyNarrativeResponse _build() {
    final _$result = _$v ??
        _$PolishSurveyNarrativeResponse._(
          text: text,
          provider: provider,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
