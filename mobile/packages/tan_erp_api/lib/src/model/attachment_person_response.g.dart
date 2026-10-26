// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attachment_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachmentPersonResponse extends AttachmentPersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;

  factory _$AttachmentPersonResponse(
          [void Function(AttachmentPersonResponseBuilder)? updates]) =>
      (AttachmentPersonResponseBuilder()..update(updates))._build();

  _$AttachmentPersonResponse._({this.id, this.displayName}) : super._();
  @override
  AttachmentPersonResponse rebuild(
          void Function(AttachmentPersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachmentPersonResponseBuilder toBuilder() =>
      AttachmentPersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachmentPersonResponse &&
        id == other.id &&
        displayName == other.displayName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AttachmentPersonResponse')
          ..add('id', id)
          ..add('displayName', displayName))
        .toString();
  }
}

class AttachmentPersonResponseBuilder
    implements
        Builder<AttachmentPersonResponse, AttachmentPersonResponseBuilder> {
  _$AttachmentPersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  AttachmentPersonResponseBuilder() {
    AttachmentPersonResponse._defaults(this);
  }

  AttachmentPersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachmentPersonResponse other) {
    _$v = other as _$AttachmentPersonResponse;
  }

  @override
  void update(void Function(AttachmentPersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachmentPersonResponse build() => _build();

  _$AttachmentPersonResponse _build() {
    final _$result = _$v ??
        _$AttachmentPersonResponse._(
          id: id,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
