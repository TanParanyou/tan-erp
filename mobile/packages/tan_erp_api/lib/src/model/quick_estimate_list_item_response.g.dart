// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quick_estimate_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuickEstimateListItemResponse extends QuickEstimateListItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final String? roomOrArea;
  @override
  final String? templateCode;
  @override
  final double? displayedLower;
  @override
  final double? displayedUpper;
  @override
  final String? shareDecision;
  @override
  final DateTime? updatedAtUtc;

  factory _$QuickEstimateListItemResponse(
          [void Function(QuickEstimateListItemResponseBuilder)? updates]) =>
      (QuickEstimateListItemResponseBuilder()..update(updates))._build();

  _$QuickEstimateListItemResponse._(
      {this.id,
      this.number,
      this.status,
      this.roomOrArea,
      this.templateCode,
      this.displayedLower,
      this.displayedUpper,
      this.shareDecision,
      this.updatedAtUtc})
      : super._();
  @override
  QuickEstimateListItemResponse rebuild(
          void Function(QuickEstimateListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuickEstimateListItemResponseBuilder toBuilder() =>
      QuickEstimateListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuickEstimateListItemResponse &&
        id == other.id &&
        number == other.number &&
        status == other.status &&
        roomOrArea == other.roomOrArea &&
        templateCode == other.templateCode &&
        displayedLower == other.displayedLower &&
        displayedUpper == other.displayedUpper &&
        shareDecision == other.shareDecision &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, roomOrArea.hashCode);
    _$hash = $jc(_$hash, templateCode.hashCode);
    _$hash = $jc(_$hash, displayedLower.hashCode);
    _$hash = $jc(_$hash, displayedUpper.hashCode);
    _$hash = $jc(_$hash, shareDecision.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuickEstimateListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('status', status)
          ..add('roomOrArea', roomOrArea)
          ..add('templateCode', templateCode)
          ..add('displayedLower', displayedLower)
          ..add('displayedUpper', displayedUpper)
          ..add('shareDecision', shareDecision)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class QuickEstimateListItemResponseBuilder
    implements
        Builder<QuickEstimateListItemResponse,
            QuickEstimateListItemResponseBuilder> {
  _$QuickEstimateListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _roomOrArea;
  String? get roomOrArea => _$this._roomOrArea;
  set roomOrArea(String? roomOrArea) => _$this._roomOrArea = roomOrArea;

  String? _templateCode;
  String? get templateCode => _$this._templateCode;
  set templateCode(String? templateCode) => _$this._templateCode = templateCode;

  double? _displayedLower;
  double? get displayedLower => _$this._displayedLower;
  set displayedLower(double? displayedLower) =>
      _$this._displayedLower = displayedLower;

  double? _displayedUpper;
  double? get displayedUpper => _$this._displayedUpper;
  set displayedUpper(double? displayedUpper) =>
      _$this._displayedUpper = displayedUpper;

  String? _shareDecision;
  String? get shareDecision => _$this._shareDecision;
  set shareDecision(String? shareDecision) =>
      _$this._shareDecision = shareDecision;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  QuickEstimateListItemResponseBuilder() {
    QuickEstimateListItemResponse._defaults(this);
  }

  QuickEstimateListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _status = $v.status;
      _roomOrArea = $v.roomOrArea;
      _templateCode = $v.templateCode;
      _displayedLower = $v.displayedLower;
      _displayedUpper = $v.displayedUpper;
      _shareDecision = $v.shareDecision;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuickEstimateListItemResponse other) {
    _$v = other as _$QuickEstimateListItemResponse;
  }

  @override
  void update(void Function(QuickEstimateListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuickEstimateListItemResponse build() => _build();

  _$QuickEstimateListItemResponse _build() {
    final _$result = _$v ??
        _$QuickEstimateListItemResponse._(
          id: id,
          number: number,
          status: status,
          roomOrArea: roomOrArea,
          templateCode: templateCode,
          displayedLower: displayedLower,
          displayedUpper: displayedUpper,
          shareDecision: shareDecision,
          updatedAtUtc: updatedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
