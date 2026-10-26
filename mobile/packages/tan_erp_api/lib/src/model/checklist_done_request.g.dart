// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'checklist_done_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ChecklistDoneRequest extends ChecklistDoneRequest {
  @override
  final bool? done;

  factory _$ChecklistDoneRequest(
          [void Function(ChecklistDoneRequestBuilder)? updates]) =>
      (ChecklistDoneRequestBuilder()..update(updates))._build();

  _$ChecklistDoneRequest._({this.done}) : super._();
  @override
  ChecklistDoneRequest rebuild(
          void Function(ChecklistDoneRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ChecklistDoneRequestBuilder toBuilder() =>
      ChecklistDoneRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ChecklistDoneRequest && done == other.done;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, done.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ChecklistDoneRequest')
          ..add('done', done))
        .toString();
  }
}

class ChecklistDoneRequestBuilder
    implements Builder<ChecklistDoneRequest, ChecklistDoneRequestBuilder> {
  _$ChecklistDoneRequest? _$v;

  bool? _done;
  bool? get done => _$this._done;
  set done(bool? done) => _$this._done = done;

  ChecklistDoneRequestBuilder() {
    ChecklistDoneRequest._defaults(this);
  }

  ChecklistDoneRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _done = $v.done;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ChecklistDoneRequest other) {
    _$v = other as _$ChecklistDoneRequest;
  }

  @override
  void update(void Function(ChecklistDoneRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ChecklistDoneRequest build() => _build();

  _$ChecklistDoneRequest _build() {
    final _$result = _$v ??
        _$ChecklistDoneRequest._(
          done: done,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
