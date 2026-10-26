// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mark_all_read_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MarkAllReadResponse extends MarkAllReadResponse {
  @override
  final int? updatedCount;

  factory _$MarkAllReadResponse(
          [void Function(MarkAllReadResponseBuilder)? updates]) =>
      (MarkAllReadResponseBuilder()..update(updates))._build();

  _$MarkAllReadResponse._({this.updatedCount}) : super._();
  @override
  MarkAllReadResponse rebuild(
          void Function(MarkAllReadResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MarkAllReadResponseBuilder toBuilder() =>
      MarkAllReadResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MarkAllReadResponse && updatedCount == other.updatedCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, updatedCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MarkAllReadResponse')
          ..add('updatedCount', updatedCount))
        .toString();
  }
}

class MarkAllReadResponseBuilder
    implements Builder<MarkAllReadResponse, MarkAllReadResponseBuilder> {
  _$MarkAllReadResponse? _$v;

  int? _updatedCount;
  int? get updatedCount => _$this._updatedCount;
  set updatedCount(int? updatedCount) => _$this._updatedCount = updatedCount;

  MarkAllReadResponseBuilder() {
    MarkAllReadResponse._defaults(this);
  }

  MarkAllReadResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _updatedCount = $v.updatedCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MarkAllReadResponse other) {
    _$v = other as _$MarkAllReadResponse;
  }

  @override
  void update(void Function(MarkAllReadResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MarkAllReadResponse build() => _build();

  _$MarkAllReadResponse _build() {
    final _$result = _$v ??
        _$MarkAllReadResponse._(
          updatedCount: updatedCount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
