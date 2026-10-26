// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warranty_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarrantyRefResponse extends WarrantyRefResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final Date? startDate;
  @override
  final Date? endDate;

  factory _$WarrantyRefResponse(
          [void Function(WarrantyRefResponseBuilder)? updates]) =>
      (WarrantyRefResponseBuilder()..update(updates))._build();

  _$WarrantyRefResponse._({this.id, this.number, this.startDate, this.endDate})
      : super._();
  @override
  WarrantyRefResponse rebuild(
          void Function(WarrantyRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarrantyRefResponseBuilder toBuilder() =>
      WarrantyRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarrantyRefResponse &&
        id == other.id &&
        number == other.number &&
        startDate == other.startDate &&
        endDate == other.endDate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, startDate.hashCode);
    _$hash = $jc(_$hash, endDate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WarrantyRefResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('startDate', startDate)
          ..add('endDate', endDate))
        .toString();
  }
}

class WarrantyRefResponseBuilder
    implements Builder<WarrantyRefResponse, WarrantyRefResponseBuilder> {
  _$WarrantyRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  Date? _startDate;
  Date? get startDate => _$this._startDate;
  set startDate(Date? startDate) => _$this._startDate = startDate;

  Date? _endDate;
  Date? get endDate => _$this._endDate;
  set endDate(Date? endDate) => _$this._endDate = endDate;

  WarrantyRefResponseBuilder() {
    WarrantyRefResponse._defaults(this);
  }

  WarrantyRefResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _startDate = $v.startDate;
      _endDate = $v.endDate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarrantyRefResponse other) {
    _$v = other as _$WarrantyRefResponse;
  }

  @override
  void update(void Function(WarrantyRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarrantyRefResponse build() => _build();

  _$WarrantyRefResponse _build() {
    final _$result = _$v ??
        _$WarrantyRefResponse._(
          id: id,
          number: number,
          startDate: startDate,
          endDate: endDate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
