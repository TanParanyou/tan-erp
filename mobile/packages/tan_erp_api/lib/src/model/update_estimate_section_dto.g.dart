// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_estimate_section_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateEstimateSectionDto extends UpdateEstimateSectionDto {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? nameTh;
  @override
  final String? nameEn;
  @override
  final int? sortOrder;
  @override
  final BuiltList<UpdateEstimateWorkItemDto>? workItems;

  factory _$UpdateEstimateSectionDto(
          [void Function(UpdateEstimateSectionDtoBuilder)? updates]) =>
      (UpdateEstimateSectionDtoBuilder()..update(updates))._build();

  _$UpdateEstimateSectionDto._(
      {this.id,
      this.code,
      this.nameTh,
      this.nameEn,
      this.sortOrder,
      this.workItems})
      : super._();
  @override
  UpdateEstimateSectionDto rebuild(
          void Function(UpdateEstimateSectionDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateEstimateSectionDtoBuilder toBuilder() =>
      UpdateEstimateSectionDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateEstimateSectionDto &&
        id == other.id &&
        code == other.code &&
        nameTh == other.nameTh &&
        nameEn == other.nameEn &&
        sortOrder == other.sortOrder &&
        workItems == other.workItems;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, workItems.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateEstimateSectionDto')
          ..add('id', id)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('nameEn', nameEn)
          ..add('sortOrder', sortOrder)
          ..add('workItems', workItems))
        .toString();
  }
}

class UpdateEstimateSectionDtoBuilder
    implements
        Builder<UpdateEstimateSectionDto, UpdateEstimateSectionDtoBuilder> {
  _$UpdateEstimateSectionDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _nameTh;
  String? get nameTh => _$this._nameTh;
  set nameTh(String? nameTh) => _$this._nameTh = nameTh;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  ListBuilder<UpdateEstimateWorkItemDto>? _workItems;
  ListBuilder<UpdateEstimateWorkItemDto> get workItems =>
      _$this._workItems ??= ListBuilder<UpdateEstimateWorkItemDto>();
  set workItems(ListBuilder<UpdateEstimateWorkItemDto>? workItems) =>
      _$this._workItems = workItems;

  UpdateEstimateSectionDtoBuilder() {
    UpdateEstimateSectionDto._defaults(this);
  }

  UpdateEstimateSectionDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _nameTh = $v.nameTh;
      _nameEn = $v.nameEn;
      _sortOrder = $v.sortOrder;
      _workItems = $v.workItems?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateEstimateSectionDto other) {
    _$v = other as _$UpdateEstimateSectionDto;
  }

  @override
  void update(void Function(UpdateEstimateSectionDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateEstimateSectionDto build() => _build();

  _$UpdateEstimateSectionDto _build() {
    _$UpdateEstimateSectionDto _$result;
    try {
      _$result = _$v ??
          _$UpdateEstimateSectionDto._(
            id: id,
            code: code,
            nameTh: nameTh,
            nameEn: nameEn,
            sortOrder: sortOrder,
            workItems: _workItems?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workItems';
        _workItems?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateEstimateSectionDto', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
