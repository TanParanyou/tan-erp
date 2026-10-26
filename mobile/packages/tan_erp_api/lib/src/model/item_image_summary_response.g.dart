// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_image_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImageSummaryResponse extends ItemImageSummaryResponse {
  @override
  final String? id;
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

  factory _$ItemImageSummaryResponse(
          [void Function(ItemImageSummaryResponseBuilder)? updates]) =>
      (ItemImageSummaryResponseBuilder()..update(updates))._build();

  _$ItemImageSummaryResponse._(
      {this.id,
      this.fileId,
      this.role,
      this.isPrimary,
      this.displayOrder,
      this.altText,
      this.caption})
      : super._();
  @override
  ItemImageSummaryResponse rebuild(
          void Function(ItemImageSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImageSummaryResponseBuilder toBuilder() =>
      ItemImageSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImageSummaryResponse &&
        id == other.id &&
        fileId == other.fileId &&
        role == other.role &&
        isPrimary == other.isPrimary &&
        displayOrder == other.displayOrder &&
        altText == other.altText &&
        caption == other.caption;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jc(_$hash, displayOrder.hashCode);
    _$hash = $jc(_$hash, altText.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImageSummaryResponse')
          ..add('id', id)
          ..add('fileId', fileId)
          ..add('role', role)
          ..add('isPrimary', isPrimary)
          ..add('displayOrder', displayOrder)
          ..add('altText', altText)
          ..add('caption', caption))
        .toString();
  }
}

class ItemImageSummaryResponseBuilder
    implements
        Builder<ItemImageSummaryResponse, ItemImageSummaryResponseBuilder> {
  _$ItemImageSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  ItemImageSummaryResponseBuilder() {
    ItemImageSummaryResponse._defaults(this);
  }

  ItemImageSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _fileId = $v.fileId;
      _role = $v.role;
      _isPrimary = $v.isPrimary;
      _displayOrder = $v.displayOrder;
      _altText = $v.altText?.toBuilder();
      _caption = $v.caption?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImageSummaryResponse other) {
    _$v = other as _$ItemImageSummaryResponse;
  }

  @override
  void update(void Function(ItemImageSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImageSummaryResponse build() => _build();

  _$ItemImageSummaryResponse _build() {
    _$ItemImageSummaryResponse _$result;
    try {
      _$result = _$v ??
          _$ItemImageSummaryResponse._(
            id: id,
            fileId: fileId,
            role: role,
            isPrimary: isPrimary,
            displayOrder: displayOrder,
            altText: _altText?.build(),
            caption: _caption?.build(),
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
            r'ItemImageSummaryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
