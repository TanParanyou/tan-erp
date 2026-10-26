// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'checklist_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ChecklistItemResponse extends ChecklistItemResponse {
  @override
  final String? id;
  @override
  final int? sortOrder;
  @override
  final String? title;
  @override
  final bool? required_;
  @override
  final bool? done;
  @override
  final ServicePersonResponse? doneBy;
  @override
  final DateTime? doneAtUtc;

  factory _$ChecklistItemResponse(
          [void Function(ChecklistItemResponseBuilder)? updates]) =>
      (ChecklistItemResponseBuilder()..update(updates))._build();

  _$ChecklistItemResponse._(
      {this.id,
      this.sortOrder,
      this.title,
      this.required_,
      this.done,
      this.doneBy,
      this.doneAtUtc})
      : super._();
  @override
  ChecklistItemResponse rebuild(
          void Function(ChecklistItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ChecklistItemResponseBuilder toBuilder() =>
      ChecklistItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ChecklistItemResponse &&
        id == other.id &&
        sortOrder == other.sortOrder &&
        title == other.title &&
        required_ == other.required_ &&
        done == other.done &&
        doneBy == other.doneBy &&
        doneAtUtc == other.doneAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, required_.hashCode);
    _$hash = $jc(_$hash, done.hashCode);
    _$hash = $jc(_$hash, doneBy.hashCode);
    _$hash = $jc(_$hash, doneAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ChecklistItemResponse')
          ..add('id', id)
          ..add('sortOrder', sortOrder)
          ..add('title', title)
          ..add('required_', required_)
          ..add('done', done)
          ..add('doneBy', doneBy)
          ..add('doneAtUtc', doneAtUtc))
        .toString();
  }
}

class ChecklistItemResponseBuilder
    implements Builder<ChecklistItemResponse, ChecklistItemResponseBuilder> {
  _$ChecklistItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  bool? _required_;
  bool? get required_ => _$this._required_;
  set required_(bool? required_) => _$this._required_ = required_;

  bool? _done;
  bool? get done => _$this._done;
  set done(bool? done) => _$this._done = done;

  ServicePersonResponseBuilder? _doneBy;
  ServicePersonResponseBuilder get doneBy =>
      _$this._doneBy ??= ServicePersonResponseBuilder();
  set doneBy(ServicePersonResponseBuilder? doneBy) => _$this._doneBy = doneBy;

  DateTime? _doneAtUtc;
  DateTime? get doneAtUtc => _$this._doneAtUtc;
  set doneAtUtc(DateTime? doneAtUtc) => _$this._doneAtUtc = doneAtUtc;

  ChecklistItemResponseBuilder() {
    ChecklistItemResponse._defaults(this);
  }

  ChecklistItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _sortOrder = $v.sortOrder;
      _title = $v.title;
      _required_ = $v.required_;
      _done = $v.done;
      _doneBy = $v.doneBy?.toBuilder();
      _doneAtUtc = $v.doneAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ChecklistItemResponse other) {
    _$v = other as _$ChecklistItemResponse;
  }

  @override
  void update(void Function(ChecklistItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ChecklistItemResponse build() => _build();

  _$ChecklistItemResponse _build() {
    _$ChecklistItemResponse _$result;
    try {
      _$result = _$v ??
          _$ChecklistItemResponse._(
            id: id,
            sortOrder: sortOrder,
            title: title,
            required_: required_,
            done: done,
            doneBy: _doneBy?.build(),
            doneAtUtc: doneAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'doneBy';
        _doneBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ChecklistItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
