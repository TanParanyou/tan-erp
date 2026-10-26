// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_run_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpRunListItemResponse extends MrpRunListItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final Date? asOfDate;
  @override
  final int? recommendationCount;
  @override
  final int? shortageCount;
  @override
  final int? openCount;
  @override
  final DateTime? createdAtUtc;

  factory _$MrpRunListItemResponse(
          [void Function(MrpRunListItemResponseBuilder)? updates]) =>
      (MrpRunListItemResponseBuilder()..update(updates))._build();

  _$MrpRunListItemResponse._(
      {this.id,
      this.number,
      this.asOfDate,
      this.recommendationCount,
      this.shortageCount,
      this.openCount,
      this.createdAtUtc})
      : super._();
  @override
  MrpRunListItemResponse rebuild(
          void Function(MrpRunListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpRunListItemResponseBuilder toBuilder() =>
      MrpRunListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpRunListItemResponse &&
        id == other.id &&
        number == other.number &&
        asOfDate == other.asOfDate &&
        recommendationCount == other.recommendationCount &&
        shortageCount == other.shortageCount &&
        openCount == other.openCount &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, asOfDate.hashCode);
    _$hash = $jc(_$hash, recommendationCount.hashCode);
    _$hash = $jc(_$hash, shortageCount.hashCode);
    _$hash = $jc(_$hash, openCount.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpRunListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('asOfDate', asOfDate)
          ..add('recommendationCount', recommendationCount)
          ..add('shortageCount', shortageCount)
          ..add('openCount', openCount)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class MrpRunListItemResponseBuilder
    implements Builder<MrpRunListItemResponse, MrpRunListItemResponseBuilder> {
  _$MrpRunListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  Date? _asOfDate;
  Date? get asOfDate => _$this._asOfDate;
  set asOfDate(Date? asOfDate) => _$this._asOfDate = asOfDate;

  int? _recommendationCount;
  int? get recommendationCount => _$this._recommendationCount;
  set recommendationCount(int? recommendationCount) =>
      _$this._recommendationCount = recommendationCount;

  int? _shortageCount;
  int? get shortageCount => _$this._shortageCount;
  set shortageCount(int? shortageCount) =>
      _$this._shortageCount = shortageCount;

  int? _openCount;
  int? get openCount => _$this._openCount;
  set openCount(int? openCount) => _$this._openCount = openCount;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  MrpRunListItemResponseBuilder() {
    MrpRunListItemResponse._defaults(this);
  }

  MrpRunListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _asOfDate = $v.asOfDate;
      _recommendationCount = $v.recommendationCount;
      _shortageCount = $v.shortageCount;
      _openCount = $v.openCount;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpRunListItemResponse other) {
    _$v = other as _$MrpRunListItemResponse;
  }

  @override
  void update(void Function(MrpRunListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpRunListItemResponse build() => _build();

  _$MrpRunListItemResponse _build() {
    final _$result = _$v ??
        _$MrpRunListItemResponse._(
          id: id,
          number: number,
          asOfDate: asOfDate,
          recommendationCount: recommendationCount,
          shortageCount: shortageCount,
          openCount: openCount,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
