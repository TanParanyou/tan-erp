// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_alias_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AddAliasRequest extends AddAliasRequest {
  @override
  final LocalizedTextInput alias;

  factory _$AddAliasRequest([void Function(AddAliasRequestBuilder)? updates]) =>
      (AddAliasRequestBuilder()..update(updates))._build();

  _$AddAliasRequest._({required this.alias}) : super._();
  @override
  AddAliasRequest rebuild(void Function(AddAliasRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AddAliasRequestBuilder toBuilder() => AddAliasRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AddAliasRequest && alias == other.alias;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, alias.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AddAliasRequest')
          ..add('alias', alias))
        .toString();
  }
}

class AddAliasRequestBuilder
    implements Builder<AddAliasRequest, AddAliasRequestBuilder> {
  _$AddAliasRequest? _$v;

  LocalizedTextInputBuilder? _alias;
  LocalizedTextInputBuilder get alias =>
      _$this._alias ??= LocalizedTextInputBuilder();
  set alias(LocalizedTextInputBuilder? alias) => _$this._alias = alias;

  AddAliasRequestBuilder() {
    AddAliasRequest._defaults(this);
  }

  AddAliasRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _alias = $v.alias.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AddAliasRequest other) {
    _$v = other as _$AddAliasRequest;
  }

  @override
  void update(void Function(AddAliasRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AddAliasRequest build() => _build();

  _$AddAliasRequest _build() {
    _$AddAliasRequest _$result;
    try {
      _$result = _$v ??
          _$AddAliasRequest._(
            alias: alias.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'alias';
        alias.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AddAliasRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
