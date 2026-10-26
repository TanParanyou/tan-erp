// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'localized_text_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$LocalizedTextResponse extends LocalizedTextResponse {
  @override
  final String? thai;
  @override
  final String? english;

  factory _$LocalizedTextResponse(
          [void Function(LocalizedTextResponseBuilder)? updates]) =>
      (LocalizedTextResponseBuilder()..update(updates))._build();

  _$LocalizedTextResponse._({this.thai, this.english}) : super._();
  @override
  LocalizedTextResponse rebuild(
          void Function(LocalizedTextResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  LocalizedTextResponseBuilder toBuilder() =>
      LocalizedTextResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is LocalizedTextResponse &&
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
    return (newBuiltValueToStringHelper(r'LocalizedTextResponse')
          ..add('thai', thai)
          ..add('english', english))
        .toString();
  }
}

class LocalizedTextResponseBuilder
    implements Builder<LocalizedTextResponse, LocalizedTextResponseBuilder> {
  _$LocalizedTextResponse? _$v;

  String? _thai;
  String? get thai => _$this._thai;
  set thai(String? thai) => _$this._thai = thai;

  String? _english;
  String? get english => _$this._english;
  set english(String? english) => _$this._english = english;

  LocalizedTextResponseBuilder() {
    LocalizedTextResponse._defaults(this);
  }

  LocalizedTextResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _thai = $v.thai;
      _english = $v.english;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(LocalizedTextResponse other) {
    _$v = other as _$LocalizedTextResponse;
  }

  @override
  void update(void Function(LocalizedTextResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  LocalizedTextResponse build() => _build();

  _$LocalizedTextResponse _build() {
    final _$result = _$v ??
        _$LocalizedTextResponse._(
          thai: thai,
          english: english,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
