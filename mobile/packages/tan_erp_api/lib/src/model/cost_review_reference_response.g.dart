// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cost_review_reference_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CostReviewReferenceResponse extends CostReviewReferenceResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;

  factory _$CostReviewReferenceResponse(
          [void Function(CostReviewReferenceResponseBuilder)? updates]) =>
      (CostReviewReferenceResponseBuilder()..update(updates))._build();

  _$CostReviewReferenceResponse._({this.id, this.code, this.name}) : super._();
  @override
  CostReviewReferenceResponse rebuild(
          void Function(CostReviewReferenceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CostReviewReferenceResponseBuilder toBuilder() =>
      CostReviewReferenceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CostReviewReferenceResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CostReviewReferenceResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class CostReviewReferenceResponseBuilder
    implements
        Builder<CostReviewReferenceResponse,
            CostReviewReferenceResponseBuilder> {
  _$CostReviewReferenceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  CostReviewReferenceResponseBuilder() {
    CostReviewReferenceResponse._defaults(this);
  }

  CostReviewReferenceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CostReviewReferenceResponse other) {
    _$v = other as _$CostReviewReferenceResponse;
  }

  @override
  void update(void Function(CostReviewReferenceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CostReviewReferenceResponse build() => _build();

  _$CostReviewReferenceResponse _build() {
    _$CostReviewReferenceResponse _$result;
    try {
      _$result = _$v ??
          _$CostReviewReferenceResponse._(
            id: id,
            code: code,
            name: _name?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CostReviewReferenceResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
