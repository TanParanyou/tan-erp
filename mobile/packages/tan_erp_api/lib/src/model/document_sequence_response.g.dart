// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'document_sequence_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DocumentSequenceResponse extends DocumentSequenceResponse {
  @override
  final String? id;
  @override
  final String? documentType;
  @override
  final String? prefix;
  @override
  final String? formatPattern;
  @override
  final String? resetPeriod;
  @override
  final int? padding;
  @override
  final bool? isBranchSpecific;
  @override
  final bool? isActive;
  @override
  final String? samplePreview;
  @override
  final String? rowVersion;

  factory _$DocumentSequenceResponse(
          [void Function(DocumentSequenceResponseBuilder)? updates]) =>
      (DocumentSequenceResponseBuilder()..update(updates))._build();

  _$DocumentSequenceResponse._(
      {this.id,
      this.documentType,
      this.prefix,
      this.formatPattern,
      this.resetPeriod,
      this.padding,
      this.isBranchSpecific,
      this.isActive,
      this.samplePreview,
      this.rowVersion})
      : super._();
  @override
  DocumentSequenceResponse rebuild(
          void Function(DocumentSequenceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DocumentSequenceResponseBuilder toBuilder() =>
      DocumentSequenceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DocumentSequenceResponse &&
        id == other.id &&
        documentType == other.documentType &&
        prefix == other.prefix &&
        formatPattern == other.formatPattern &&
        resetPeriod == other.resetPeriod &&
        padding == other.padding &&
        isBranchSpecific == other.isBranchSpecific &&
        isActive == other.isActive &&
        samplePreview == other.samplePreview &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, documentType.hashCode);
    _$hash = $jc(_$hash, prefix.hashCode);
    _$hash = $jc(_$hash, formatPattern.hashCode);
    _$hash = $jc(_$hash, resetPeriod.hashCode);
    _$hash = $jc(_$hash, padding.hashCode);
    _$hash = $jc(_$hash, isBranchSpecific.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, samplePreview.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DocumentSequenceResponse')
          ..add('id', id)
          ..add('documentType', documentType)
          ..add('prefix', prefix)
          ..add('formatPattern', formatPattern)
          ..add('resetPeriod', resetPeriod)
          ..add('padding', padding)
          ..add('isBranchSpecific', isBranchSpecific)
          ..add('isActive', isActive)
          ..add('samplePreview', samplePreview)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class DocumentSequenceResponseBuilder
    implements
        Builder<DocumentSequenceResponse, DocumentSequenceResponseBuilder> {
  _$DocumentSequenceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _documentType;
  String? get documentType => _$this._documentType;
  set documentType(String? documentType) => _$this._documentType = documentType;

  String? _prefix;
  String? get prefix => _$this._prefix;
  set prefix(String? prefix) => _$this._prefix = prefix;

  String? _formatPattern;
  String? get formatPattern => _$this._formatPattern;
  set formatPattern(String? formatPattern) =>
      _$this._formatPattern = formatPattern;

  String? _resetPeriod;
  String? get resetPeriod => _$this._resetPeriod;
  set resetPeriod(String? resetPeriod) => _$this._resetPeriod = resetPeriod;

  int? _padding;
  int? get padding => _$this._padding;
  set padding(int? padding) => _$this._padding = padding;

  bool? _isBranchSpecific;
  bool? get isBranchSpecific => _$this._isBranchSpecific;
  set isBranchSpecific(bool? isBranchSpecific) =>
      _$this._isBranchSpecific = isBranchSpecific;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  String? _samplePreview;
  String? get samplePreview => _$this._samplePreview;
  set samplePreview(String? samplePreview) =>
      _$this._samplePreview = samplePreview;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DocumentSequenceResponseBuilder() {
    DocumentSequenceResponse._defaults(this);
  }

  DocumentSequenceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _documentType = $v.documentType;
      _prefix = $v.prefix;
      _formatPattern = $v.formatPattern;
      _resetPeriod = $v.resetPeriod;
      _padding = $v.padding;
      _isBranchSpecific = $v.isBranchSpecific;
      _isActive = $v.isActive;
      _samplePreview = $v.samplePreview;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DocumentSequenceResponse other) {
    _$v = other as _$DocumentSequenceResponse;
  }

  @override
  void update(void Function(DocumentSequenceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DocumentSequenceResponse build() => _build();

  _$DocumentSequenceResponse _build() {
    final _$result = _$v ??
        _$DocumentSequenceResponse._(
          id: id,
          documentType: documentType,
          prefix: prefix,
          formatPattern: formatPattern,
          resetPeriod: resetPeriod,
          padding: padding,
          isBranchSpecific: isBranchSpecific,
          isActive: isActive,
          samplePreview: samplePreview,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
