// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestRequest extends PurchaseRequestRequest {
  @override
  final String? projectId;
  @override
  final Date? requiredByDate;
  @override
  final String? note;
  @override
  final BuiltList<PurchaseRequestLineRequest>? lines;

  factory _$PurchaseRequestRequest(
          [void Function(PurchaseRequestRequestBuilder)? updates]) =>
      (PurchaseRequestRequestBuilder()..update(updates))._build();

  _$PurchaseRequestRequest._(
      {this.projectId, this.requiredByDate, this.note, this.lines})
      : super._();
  @override
  PurchaseRequestRequest rebuild(
          void Function(PurchaseRequestRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestRequestBuilder toBuilder() =>
      PurchaseRequestRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestRequest &&
        projectId == other.projectId &&
        requiredByDate == other.requiredByDate &&
        note == other.note &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, requiredByDate.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseRequestRequest')
          ..add('projectId', projectId)
          ..add('requiredByDate', requiredByDate)
          ..add('note', note)
          ..add('lines', lines))
        .toString();
  }
}

class PurchaseRequestRequestBuilder
    implements Builder<PurchaseRequestRequest, PurchaseRequestRequestBuilder> {
  _$PurchaseRequestRequest? _$v;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  Date? _requiredByDate;
  Date? get requiredByDate => _$this._requiredByDate;
  set requiredByDate(Date? requiredByDate) =>
      _$this._requiredByDate = requiredByDate;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ListBuilder<PurchaseRequestLineRequest>? _lines;
  ListBuilder<PurchaseRequestLineRequest> get lines =>
      _$this._lines ??= ListBuilder<PurchaseRequestLineRequest>();
  set lines(ListBuilder<PurchaseRequestLineRequest>? lines) =>
      _$this._lines = lines;

  PurchaseRequestRequestBuilder() {
    PurchaseRequestRequest._defaults(this);
  }

  PurchaseRequestRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _projectId = $v.projectId;
      _requiredByDate = $v.requiredByDate;
      _note = $v.note;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestRequest other) {
    _$v = other as _$PurchaseRequestRequest;
  }

  @override
  void update(void Function(PurchaseRequestRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestRequest build() => _build();

  _$PurchaseRequestRequest _build() {
    _$PurchaseRequestRequest _$result;
    try {
      _$result = _$v ??
          _$PurchaseRequestRequest._(
            projectId: projectId,
            requiredByDate: requiredByDate,
            note: note,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PurchaseRequestRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
