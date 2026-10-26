// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_survey_evidence_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateSurveyEvidenceRequest extends UpdateSurveyEvidenceRequest {
  @override
  final String? fileId;
  @override
  final String? kind;
  @override
  final String? caption;
  @override
  final int? sortOrder;

  factory _$UpdateSurveyEvidenceRequest(
          [void Function(UpdateSurveyEvidenceRequestBuilder)? updates]) =>
      (UpdateSurveyEvidenceRequestBuilder()..update(updates))._build();

  _$UpdateSurveyEvidenceRequest._(
      {this.fileId, this.kind, this.caption, this.sortOrder})
      : super._();
  @override
  UpdateSurveyEvidenceRequest rebuild(
          void Function(UpdateSurveyEvidenceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateSurveyEvidenceRequestBuilder toBuilder() =>
      UpdateSurveyEvidenceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateSurveyEvidenceRequest &&
        fileId == other.fileId &&
        kind == other.kind &&
        caption == other.caption &&
        sortOrder == other.sortOrder;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateSurveyEvidenceRequest')
          ..add('fileId', fileId)
          ..add('kind', kind)
          ..add('caption', caption)
          ..add('sortOrder', sortOrder))
        .toString();
  }
}

class UpdateSurveyEvidenceRequestBuilder
    implements
        Builder<UpdateSurveyEvidenceRequest,
            UpdateSurveyEvidenceRequestBuilder> {
  _$UpdateSurveyEvidenceRequest? _$v;

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

  UpdateSurveyEvidenceRequestBuilder() {
    UpdateSurveyEvidenceRequest._defaults(this);
  }

  UpdateSurveyEvidenceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fileId = $v.fileId;
      _kind = $v.kind;
      _caption = $v.caption;
      _sortOrder = $v.sortOrder;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateSurveyEvidenceRequest other) {
    _$v = other as _$UpdateSurveyEvidenceRequest;
  }

  @override
  void update(void Function(UpdateSurveyEvidenceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateSurveyEvidenceRequest build() => _build();

  _$UpdateSurveyEvidenceRequest _build() {
    final _$result = _$v ??
        _$UpdateSurveyEvidenceRequest._(
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
