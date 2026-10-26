// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_image_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteImageResponse extends SiteImageResponse {
  @override
  final String? id;
  @override
  final String? fileId;
  @override
  final String? caption;
  @override
  final int? displayOrder;
  @override
  final DateTime? createdAtUtc;

  factory _$SiteImageResponse(
          [void Function(SiteImageResponseBuilder)? updates]) =>
      (SiteImageResponseBuilder()..update(updates))._build();

  _$SiteImageResponse._(
      {this.id,
      this.fileId,
      this.caption,
      this.displayOrder,
      this.createdAtUtc})
      : super._();
  @override
  SiteImageResponse rebuild(void Function(SiteImageResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteImageResponseBuilder toBuilder() =>
      SiteImageResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteImageResponse &&
        id == other.id &&
        fileId == other.fileId &&
        caption == other.caption &&
        displayOrder == other.displayOrder &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jc(_$hash, displayOrder.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteImageResponse')
          ..add('id', id)
          ..add('fileId', fileId)
          ..add('caption', caption)
          ..add('displayOrder', displayOrder)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class SiteImageResponseBuilder
    implements Builder<SiteImageResponse, SiteImageResponseBuilder> {
  _$SiteImageResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _caption;
  String? get caption => _$this._caption;
  set caption(String? caption) => _$this._caption = caption;

  int? _displayOrder;
  int? get displayOrder => _$this._displayOrder;
  set displayOrder(int? displayOrder) => _$this._displayOrder = displayOrder;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  SiteImageResponseBuilder() {
    SiteImageResponse._defaults(this);
  }

  SiteImageResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _fileId = $v.fileId;
      _caption = $v.caption;
      _displayOrder = $v.displayOrder;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteImageResponse other) {
    _$v = other as _$SiteImageResponse;
  }

  @override
  void update(void Function(SiteImageResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteImageResponse build() => _build();

  _$SiteImageResponse _build() {
    final _$result = _$v ??
        _$SiteImageResponse._(
          id: id,
          fileId: fileId,
          caption: caption,
          displayOrder: displayOrder,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
