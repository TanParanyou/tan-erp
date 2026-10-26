// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_branch_availability_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemBranchAvailabilityResponse extends ItemBranchAvailabilityResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? branchCode;
  @override
  final String? branchName;
  @override
  final String? status;
  @override
  final DateTime? effectiveFromUtc;
  @override
  final DateTime? effectiveToUtc;

  factory _$ItemBranchAvailabilityResponse(
          [void Function(ItemBranchAvailabilityResponseBuilder)? updates]) =>
      (ItemBranchAvailabilityResponseBuilder()..update(updates))._build();

  _$ItemBranchAvailabilityResponse._(
      {this.id,
      this.branchId,
      this.branchCode,
      this.branchName,
      this.status,
      this.effectiveFromUtc,
      this.effectiveToUtc})
      : super._();
  @override
  ItemBranchAvailabilityResponse rebuild(
          void Function(ItemBranchAvailabilityResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemBranchAvailabilityResponseBuilder toBuilder() =>
      ItemBranchAvailabilityResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemBranchAvailabilityResponse &&
        id == other.id &&
        branchId == other.branchId &&
        branchCode == other.branchCode &&
        branchName == other.branchName &&
        status == other.status &&
        effectiveFromUtc == other.effectiveFromUtc &&
        effectiveToUtc == other.effectiveToUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, branchName.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, effectiveToUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemBranchAvailabilityResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('branchCode', branchCode)
          ..add('branchName', branchName)
          ..add('status', status)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('effectiveToUtc', effectiveToUtc))
        .toString();
  }
}

class ItemBranchAvailabilityResponseBuilder
    implements
        Builder<ItemBranchAvailabilityResponse,
            ItemBranchAvailabilityResponseBuilder> {
  _$ItemBranchAvailabilityResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  String? _branchName;
  String? get branchName => _$this._branchName;
  set branchName(String? branchName) => _$this._branchName = branchName;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  DateTime? _effectiveFromUtc;
  DateTime? get effectiveFromUtc => _$this._effectiveFromUtc;
  set effectiveFromUtc(DateTime? effectiveFromUtc) =>
      _$this._effectiveFromUtc = effectiveFromUtc;

  DateTime? _effectiveToUtc;
  DateTime? get effectiveToUtc => _$this._effectiveToUtc;
  set effectiveToUtc(DateTime? effectiveToUtc) =>
      _$this._effectiveToUtc = effectiveToUtc;

  ItemBranchAvailabilityResponseBuilder() {
    ItemBranchAvailabilityResponse._defaults(this);
  }

  ItemBranchAvailabilityResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _branchCode = $v.branchCode;
      _branchName = $v.branchName;
      _status = $v.status;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _effectiveToUtc = $v.effectiveToUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemBranchAvailabilityResponse other) {
    _$v = other as _$ItemBranchAvailabilityResponse;
  }

  @override
  void update(void Function(ItemBranchAvailabilityResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemBranchAvailabilityResponse build() => _build();

  _$ItemBranchAvailabilityResponse _build() {
    final _$result = _$v ??
        _$ItemBranchAvailabilityResponse._(
          id: id,
          branchId: branchId,
          branchCode: branchCode,
          branchName: branchName,
          status: status,
          effectiveFromUtc: effectiveFromUtc,
          effectiveToUtc: effectiveToUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
