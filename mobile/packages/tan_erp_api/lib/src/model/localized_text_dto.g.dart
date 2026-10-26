// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'localized_text_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$LocalizedTextDto extends LocalizedTextDto {
  @override
  final String? thai;
  @override
  final String? english;

  factory _$LocalizedTextDto(
          [void Function(LocalizedTextDtoBuilder)? updates]) =>
      (LocalizedTextDtoBuilder()..update(updates))._build();

  _$LocalizedTextDto._({this.thai, this.english}) : super._();
  @override
  LocalizedTextDto rebuild(void Function(LocalizedTextDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  LocalizedTextDtoBuilder toBuilder() =>
      LocalizedTextDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is LocalizedTextDto &&
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
    return (newBuiltValueToStringHelper(r'LocalizedTextDto')
          ..add('thai', thai)
          ..add('english', english))
        .toString();
  }
}

class LocalizedTextDtoBuilder
    implements Builder<LocalizedTextDto, LocalizedTextDtoBuilder> {
  _$LocalizedTextDto? _$v;

  String? _thai;
  String? get thai => _$this._thai;
  set thai(String? thai) => _$this._thai = thai;

  String? _english;
  String? get english => _$this._english;
  set english(String? english) => _$this._english = english;

  LocalizedTextDtoBuilder() {
    LocalizedTextDto._defaults(this);
  }

  LocalizedTextDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _thai = $v.thai;
      _english = $v.english;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(LocalizedTextDto other) {
    _$v = other as _$LocalizedTextDto;
  }

  @override
  void update(void Function(LocalizedTextDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  LocalizedTextDto build() => _build();

  _$LocalizedTextDto _build() {
    final _$result = _$v ??
        _$LocalizedTextDto._(
          thai: thai,
          english: english,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
