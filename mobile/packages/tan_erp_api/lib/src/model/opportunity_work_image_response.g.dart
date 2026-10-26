// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'opportunity_work_image_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OpportunityWorkImageResponse extends OpportunityWorkImageResponse {
  @override
  final String? id;
  @override
  final String? fileId;
  @override
  final String? stageAtAttach;
  @override
  final String? caption;
  @override
  final int? displayOrder;
  @override
  final DateTime? createdAtUtc;
  @override
  final WorkImageUserSummaryResponse? createdBy;

  factory _$OpportunityWorkImageResponse(
          [void Function(OpportunityWorkImageResponseBuilder)? updates]) =>
      (OpportunityWorkImageResponseBuilder()..update(updates))._build();

  _$OpportunityWorkImageResponse._(
      {this.id,
      this.fileId,
      this.stageAtAttach,
      this.caption,
      this.displayOrder,
      this.createdAtUtc,
      this.createdBy})
      : super._();
  @override
  OpportunityWorkImageResponse rebuild(
          void Function(OpportunityWorkImageResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OpportunityWorkImageResponseBuilder toBuilder() =>
      OpportunityWorkImageResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OpportunityWorkImageResponse &&
        id == other.id &&
        fileId == other.fileId &&
        stageAtAttach == other.stageAtAttach &&
        caption == other.caption &&
        displayOrder == other.displayOrder &&
        createdAtUtc == other.createdAtUtc &&
        createdBy == other.createdBy;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, stageAtAttach.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jc(_$hash, displayOrder.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OpportunityWorkImageResponse')
          ..add('id', id)
          ..add('fileId', fileId)
          ..add('stageAtAttach', stageAtAttach)
          ..add('caption', caption)
          ..add('displayOrder', displayOrder)
          ..add('createdAtUtc', createdAtUtc)
          ..add('createdBy', createdBy))
        .toString();
  }
}

class OpportunityWorkImageResponseBuilder
    implements
        Builder<OpportunityWorkImageResponse,
            OpportunityWorkImageResponseBuilder> {
  _$OpportunityWorkImageResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _stageAtAttach;
  String? get stageAtAttach => _$this._stageAtAttach;
  set stageAtAttach(String? stageAtAttach) =>
      _$this._stageAtAttach = stageAtAttach;

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

  WorkImageUserSummaryResponseBuilder? _createdBy;
  WorkImageUserSummaryResponseBuilder get createdBy =>
      _$this._createdBy ??= WorkImageUserSummaryResponseBuilder();
  set createdBy(WorkImageUserSummaryResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  OpportunityWorkImageResponseBuilder() {
    OpportunityWorkImageResponse._defaults(this);
  }

  OpportunityWorkImageResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _fileId = $v.fileId;
      _stageAtAttach = $v.stageAtAttach;
      _caption = $v.caption;
      _displayOrder = $v.displayOrder;
      _createdAtUtc = $v.createdAtUtc;
      _createdBy = $v.createdBy?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OpportunityWorkImageResponse other) {
    _$v = other as _$OpportunityWorkImageResponse;
  }

  @override
  void update(void Function(OpportunityWorkImageResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OpportunityWorkImageResponse build() => _build();

  _$OpportunityWorkImageResponse _build() {
    _$OpportunityWorkImageResponse _$result;
    try {
      _$result = _$v ??
          _$OpportunityWorkImageResponse._(
            id: id,
            fileId: fileId,
            stageAtAttach: stageAtAttach,
            caption: caption,
            displayOrder: displayOrder,
            createdAtUtc: createdAtUtc,
            createdBy: _createdBy?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'OpportunityWorkImageResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
