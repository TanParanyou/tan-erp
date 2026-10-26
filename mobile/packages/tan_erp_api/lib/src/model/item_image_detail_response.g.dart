// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_image_detail_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImageDetailResponse extends ItemImageDetailResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? itemId;
  @override
  final String? fileId;
  @override
  final String? role;
  @override
  final bool? isPrimary;
  @override
  final int? displayOrder;
  @override
  final LocalizedTextResponse? altText;
  @override
  final LocalizedTextResponse? caption;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? createdByUserId;
  @override
  final String? fileName;
  @override
  final String? contentType;
  @override
  final int? byteSize;
  @override
  final int? width;
  @override
  final int? height;

  factory _$ItemImageDetailResponse(
          [void Function(ItemImageDetailResponseBuilder)? updates]) =>
      (ItemImageDetailResponseBuilder()..update(updates))._build();

  _$ItemImageDetailResponse._(
      {this.id,
      this.organizationId,
      this.itemId,
      this.fileId,
      this.role,
      this.isPrimary,
      this.displayOrder,
      this.altText,
      this.caption,
      this.status,
      this.rowVersion,
      this.createdAtUtc,
      this.createdByUserId,
      this.fileName,
      this.contentType,
      this.byteSize,
      this.width,
      this.height})
      : super._();
  @override
  ItemImageDetailResponse rebuild(
          void Function(ItemImageDetailResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImageDetailResponseBuilder toBuilder() =>
      ItemImageDetailResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImageDetailResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        itemId == other.itemId &&
        fileId == other.fileId &&
        role == other.role &&
        isPrimary == other.isPrimary &&
        displayOrder == other.displayOrder &&
        altText == other.altText &&
        caption == other.caption &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        createdByUserId == other.createdByUserId &&
        fileName == other.fileName &&
        contentType == other.contentType &&
        byteSize == other.byteSize &&
        width == other.width &&
        height == other.height;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jc(_$hash, displayOrder.hashCode);
    _$hash = $jc(_$hash, altText.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, createdByUserId.hashCode);
    _$hash = $jc(_$hash, fileName.hashCode);
    _$hash = $jc(_$hash, contentType.hashCode);
    _$hash = $jc(_$hash, byteSize.hashCode);
    _$hash = $jc(_$hash, width.hashCode);
    _$hash = $jc(_$hash, height.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImageDetailResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('itemId', itemId)
          ..add('fileId', fileId)
          ..add('role', role)
          ..add('isPrimary', isPrimary)
          ..add('displayOrder', displayOrder)
          ..add('altText', altText)
          ..add('caption', caption)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('createdByUserId', createdByUserId)
          ..add('fileName', fileName)
          ..add('contentType', contentType)
          ..add('byteSize', byteSize)
          ..add('width', width)
          ..add('height', height))
        .toString();
  }
}

class ItemImageDetailResponseBuilder
    implements
        Builder<ItemImageDetailResponse, ItemImageDetailResponseBuilder> {
  _$ItemImageDetailResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _role;
  String? get role => _$this._role;
  set role(String? role) => _$this._role = role;

  bool? _isPrimary;
  bool? get isPrimary => _$this._isPrimary;
  set isPrimary(bool? isPrimary) => _$this._isPrimary = isPrimary;

  int? _displayOrder;
  int? get displayOrder => _$this._displayOrder;
  set displayOrder(int? displayOrder) => _$this._displayOrder = displayOrder;

  LocalizedTextResponseBuilder? _altText;
  LocalizedTextResponseBuilder get altText =>
      _$this._altText ??= LocalizedTextResponseBuilder();
  set altText(LocalizedTextResponseBuilder? altText) =>
      _$this._altText = altText;

  LocalizedTextResponseBuilder? _caption;
  LocalizedTextResponseBuilder get caption =>
      _$this._caption ??= LocalizedTextResponseBuilder();
  set caption(LocalizedTextResponseBuilder? caption) =>
      _$this._caption = caption;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _createdByUserId;
  String? get createdByUserId => _$this._createdByUserId;
  set createdByUserId(String? createdByUserId) =>
      _$this._createdByUserId = createdByUserId;

  String? _fileName;
  String? get fileName => _$this._fileName;
  set fileName(String? fileName) => _$this._fileName = fileName;

  String? _contentType;
  String? get contentType => _$this._contentType;
  set contentType(String? contentType) => _$this._contentType = contentType;

  int? _byteSize;
  int? get byteSize => _$this._byteSize;
  set byteSize(int? byteSize) => _$this._byteSize = byteSize;

  int? _width;
  int? get width => _$this._width;
  set width(int? width) => _$this._width = width;

  int? _height;
  int? get height => _$this._height;
  set height(int? height) => _$this._height = height;

  ItemImageDetailResponseBuilder() {
    ItemImageDetailResponse._defaults(this);
  }

  ItemImageDetailResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _itemId = $v.itemId;
      _fileId = $v.fileId;
      _role = $v.role;
      _isPrimary = $v.isPrimary;
      _displayOrder = $v.displayOrder;
      _altText = $v.altText?.toBuilder();
      _caption = $v.caption?.toBuilder();
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _createdByUserId = $v.createdByUserId;
      _fileName = $v.fileName;
      _contentType = $v.contentType;
      _byteSize = $v.byteSize;
      _width = $v.width;
      _height = $v.height;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImageDetailResponse other) {
    _$v = other as _$ItemImageDetailResponse;
  }

  @override
  void update(void Function(ItemImageDetailResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImageDetailResponse build() => _build();

  _$ItemImageDetailResponse _build() {
    _$ItemImageDetailResponse _$result;
    try {
      _$result = _$v ??
          _$ItemImageDetailResponse._(
            id: id,
            organizationId: organizationId,
            itemId: itemId,
            fileId: fileId,
            role: role,
            isPrimary: isPrimary,
            displayOrder: displayOrder,
            altText: _altText?.build(),
            caption: _caption?.build(),
            status: status,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            createdByUserId: createdByUserId,
            fileName: fileName,
            contentType: contentType,
            byteSize: byteSize,
            width: width,
            height: height,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'altText';
        _altText?.build();
        _$failedField = 'caption';
        _caption?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemImageDetailResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
