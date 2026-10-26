// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_survey_evidence_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSurveyEvidenceResponse extends SiteSurveyEvidenceResponse {
  @override
  final String? id;
  @override
  final String? fileId;
  @override
  final String? kind;
  @override
  final String? caption;
  @override
  final int? sortOrder;

  factory _$SiteSurveyEvidenceResponse(
          [void Function(SiteSurveyEvidenceResponseBuilder)? updates]) =>
      (SiteSurveyEvidenceResponseBuilder()..update(updates))._build();

  _$SiteSurveyEvidenceResponse._(
      {this.id, this.fileId, this.kind, this.caption, this.sortOrder})
      : super._();
  @override
  SiteSurveyEvidenceResponse rebuild(
          void Function(SiteSurveyEvidenceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSurveyEvidenceResponseBuilder toBuilder() =>
      SiteSurveyEvidenceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSurveyEvidenceResponse &&
        id == other.id &&
        fileId == other.fileId &&
        kind == other.kind &&
        caption == other.caption &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSurveyEvidenceResponse')
          ..add('id', id)
          ..add('fileId', fileId)
          ..add('kind', kind)
          ..add('caption', caption)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class SiteSurveyEvidenceResponseBuilder
    implements
        Builder<SiteSurveyEvidenceResponse, SiteSurveyEvidenceResponseBuilder> {
  _$SiteSurveyEvidenceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _caption;
  String? get caption => _$this._caption;
  set caption(String? caption) => _$this._caption = caption;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  SiteSurveyEvidenceResponseBuilder() {
    SiteSurveyEvidenceResponse._defaults(this);
  }

  SiteSurveyEvidenceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _fileId = $v.fileId;
      _kind = $v.kind;
      _caption = $v.caption;
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSurveyEvidenceResponse other) {
    _$v = other as _$SiteSurveyEvidenceResponse;
  }

  @override
  void update(void Function(SiteSurveyEvidenceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSurveyEvidenceResponse build() => _build();

  _$SiteSurveyEvidenceResponse _build() {
    final _$result = _$v ??
        _$SiteSurveyEvidenceResponse._(
          id: id,
          fileId: fileId,
          kind: kind,
          caption: caption,
          sortOrder: sortOrder,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
