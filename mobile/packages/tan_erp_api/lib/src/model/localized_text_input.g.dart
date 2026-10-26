// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'localized_text_input.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$LocalizedTextInput extends LocalizedTextInput {
  @override
  final String thai;
  @override
  final String? english;

  factory _$LocalizedTextInput(
          [void Function(LocalizedTextInputBuilder)? updates]) =>
      (LocalizedTextInputBuilder()..update(updates))._build();

  _$LocalizedTextInput._({required this.thai, this.english}) : super._();
  @override
  LocalizedTextInput rebuild(
          void Function(LocalizedTextInputBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  LocalizedTextInputBuilder toBuilder() =>
      LocalizedTextInputBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is LocalizedTextInput &&
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
    return (newBuiltValueToStringHelper(r'LocalizedTextInput')
          ..add('thai', thai)
          ..add('english', english))
        .toString();
  }
}

class LocalizedTextInputBuilder
    implements Builder<LocalizedTextInput, LocalizedTextInputBuilder> {
  _$LocalizedTextInput? _$v;

  String? _thai;
  String? get thai => _$this._thai;
  set thai(String? thai) => _$this._thai = thai;

  String? _english;
  String? get english => _$this._english;
  set english(String? english) => _$this._english = english;

  LocalizedTextInputBuilder() {
    LocalizedTextInput._defaults(this);
  }

  LocalizedTextInputBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _thai = $v.thai;
      _english = $v.english;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(LocalizedTextInput other) {
    _$v = other as _$LocalizedTextInput;
  }

  @override
  void update(void Function(LocalizedTextInputBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  LocalizedTextInput build() => _build();

  _$LocalizedTextInput _build() {
    final _$result = _$v ??
        _$LocalizedTextInput._(
          thai: BuiltValueNullFieldError.checkNotNull(
              thai, r'LocalizedTextInput', 'thai'),
          english: english,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
