// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_revision_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomRevisionResponse extends BomRevisionResponse {
  @override
  final String? id;
  @override
  final int? revisionNo;
  @override
  final String? status;
  @override
  final double? outputQuantity;
  @override
  final String? note;
  @override
  final ProductionPersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final ProductionPersonResponse? approvedBy;
  @override
  final DateTime? approvedAtUtc;
  @override
  final String? rowVersion;
  @override
  final BuiltList<BomLineResponse>? lines;

  factory _$BomRevisionResponse(
          [void Function(BomRevisionResponseBuilder)? updates]) =>
      (BomRevisionResponseBuilder()..update(updates))._build();

  _$BomRevisionResponse._(
      {this.id,
      this.revisionNo,
      this.status,
      this.outputQuantity,
      this.note,
      this.createdBy,
      this.createdAtUtc,
      this.approvedBy,
      this.approvedAtUtc,
      this.rowVersion,
      this.lines})
      : super._();
  @override
  BomRevisionResponse rebuild(
          void Function(BomRevisionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomRevisionResponseBuilder toBuilder() =>
      BomRevisionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomRevisionResponse &&
        id == other.id &&
        revisionNo == other.revisionNo &&
        status == other.status &&
        outputQuantity == other.outputQuantity &&
        note == other.note &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        approvedBy == other.approvedBy &&
        approvedAtUtc == other.approvedAtUtc &&
        rowVersion == other.rowVersion &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, revisionNo.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, outputQuantity.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, approvedBy.hashCode);
    _$hash = $jc(_$hash, approvedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomRevisionResponse')
          ..add('id', id)
          ..add('revisionNo', revisionNo)
          ..add('status', status)
          ..add('outputQuantity', outputQuantity)
          ..add('note', note)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('approvedBy', approvedBy)
          ..add('approvedAtUtc', approvedAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('lines', lines))
        .toString();
  }
}

class BomRevisionResponseBuilder
    implements Builder<BomRevisionResponse, BomRevisionResponseBuilder> {
  _$BomRevisionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _revisionNo;
  int? get revisionNo => _$this._revisionNo;
  set revisionNo(int? revisionNo) => _$this._revisionNo = revisionNo;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _outputQuantity;
  double? get outputQuantity => _$this._outputQuantity;
  set outputQuantity(double? outputQuantity) =>
      _$this._outputQuantity = outputQuantity;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ProductionPersonResponseBuilder? _createdBy;
  ProductionPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ProductionPersonResponseBuilder();
  set createdBy(ProductionPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ProductionPersonResponseBuilder? _approvedBy;
  ProductionPersonResponseBuilder get approvedBy =>
      _$this._approvedBy ??= ProductionPersonResponseBuilder();
  set approvedBy(ProductionPersonResponseBuilder? approvedBy) =>
      _$this._approvedBy = approvedBy;

  DateTime? _approvedAtUtc;
  DateTime? get approvedAtUtc => _$this._approvedAtUtc;
  set approvedAtUtc(DateTime? approvedAtUtc) =>
      _$this._approvedAtUtc = approvedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<BomLineResponse>? _lines;
  ListBuilder<BomLineResponse> get lines =>
      _$this._lines ??= ListBuilder<BomLineResponse>();
  set lines(ListBuilder<BomLineResponse>? lines) => _$this._lines = lines;

  BomRevisionResponseBuilder() {
    BomRevisionResponse._defaults(this);
  }

  BomRevisionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _revisionNo = $v.revisionNo;
      _status = $v.status;
      _outputQuantity = $v.outputQuantity;
      _note = $v.note;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _approvedBy = $v.approvedBy?.toBuilder();
      _approvedAtUtc = $v.approvedAtUtc;
      _rowVersion = $v.rowVersion;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomRevisionResponse other) {
    _$v = other as _$BomRevisionResponse;
  }

  @override
  void update(void Function(BomRevisionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomRevisionResponse build() => _build();

  _$BomRevisionResponse _build() {
    _$BomRevisionResponse _$result;
    try {
      _$result = _$v ??
          _$BomRevisionResponse._(
            id: id,
            revisionNo: revisionNo,
            status: status,
            outputQuantity: outputQuantity,
            note: note,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            approvedBy: _approvedBy?.build(),
            approvedAtUtc: approvedAtUtc,
            rowVersion: rowVersion,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'approvedBy';
        _approvedBy?.build();

        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BomRevisionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
