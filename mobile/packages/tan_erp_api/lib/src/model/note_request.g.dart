// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'note_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$NoteRequest extends NoteRequest {
  @override
  final String? note;

  factory _$NoteRequest([void Function(NoteRequestBuilder)? updates]) =>
      (NoteRequestBuilder()..update(updates))._build();

  _$NoteRequest._({this.note}) : super._();
  @override
  NoteRequest rebuild(void Function(NoteRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  NoteRequestBuilder toBuilder() => NoteRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is NoteRequest && note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'NoteRequest')..add('note', note))
        .toString();
  }
}

class NoteRequestBuilder implements Builder<NoteRequest, NoteRequestBuilder> {
  _$NoteRequest? _$v;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  NoteRequestBuilder() {
    NoteRequest._defaults(this);
  }

  NoteRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(NoteRequest other) {
    _$v = other as _$NoteRequest;
  }

  @override
  void update(void Function(NoteRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  NoteRequest build() => _build();

  _$NoteRequest _build() {
    final _$result = _$v ??
        _$NoteRequest._(
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
