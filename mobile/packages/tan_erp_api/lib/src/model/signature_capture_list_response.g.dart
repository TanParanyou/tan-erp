// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'signature_capture_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SignatureCaptureListResponse extends SignatureCaptureListResponse {
  @override
  final BuiltList<SignatureCaptureResponse>? items;

  factory _$SignatureCaptureListResponse(
          [void Function(SignatureCaptureListResponseBuilder)? updates]) =>
      (SignatureCaptureListResponseBuilder()..update(updates))._build();

  _$SignatureCaptureListResponse._({this.items}) : super._();
  @override
  SignatureCaptureListResponse rebuild(
          void Function(SignatureCaptureListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SignatureCaptureListResponseBuilder toBuilder() =>
      SignatureCaptureListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SignatureCaptureListResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SignatureCaptureListResponse')
          ..add('items', items))
        .toString();
  }
}

class SignatureCaptureListResponseBuilder
    implements
        Builder<SignatureCaptureListResponse,
            SignatureCaptureListResponseBuilder> {
  _$SignatureCaptureListResponse? _$v;

  ListBuilder<SignatureCaptureResponse>? _items;
  ListBuilder<SignatureCaptureResponse> get items =>
      _$this._items ??= ListBuilder<SignatureCaptureResponse>();
  set items(ListBuilder<SignatureCaptureResponse>? items) =>
      _$this._items = items;

  SignatureCaptureListResponseBuilder() {
    SignatureCaptureListResponse._defaults(this);
  }

  SignatureCaptureListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SignatureCaptureListResponse other) {
    _$v = other as _$SignatureCaptureListResponse;
  }

  @override
  void update(void Function(SignatureCaptureListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SignatureCaptureListResponse build() => _build();

  _$SignatureCaptureListResponse _build() {
    _$SignatureCaptureListResponse _$result;
    try {
      _$result = _$v ??
          _$SignatureCaptureListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SignatureCaptureListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
