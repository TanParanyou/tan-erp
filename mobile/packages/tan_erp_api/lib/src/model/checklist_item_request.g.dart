// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'checklist_item_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ChecklistItemRequest extends ChecklistItemRequest {
  @override
  final String? title;
  @override
  final bool? required_;

  factory _$ChecklistItemRequest(
          [void Function(ChecklistItemRequestBuilder)? updates]) =>
      (ChecklistItemRequestBuilder()..update(updates))._build();

  _$ChecklistItemRequest._({this.title, this.required_}) : super._();
  @override
  ChecklistItemRequest rebuild(
          void Function(ChecklistItemRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ChecklistItemRequestBuilder toBuilder() =>
      ChecklistItemRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ChecklistItemRequest &&
        title == other.title &&
        required_ == other.required_;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, required_.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ChecklistItemRequest')
          ..add('title', title)
          ..add('required_', required_))
        .toString();
  }
}

class ChecklistItemRequestBuilder
    implements Builder<ChecklistItemRequest, ChecklistItemRequestBuilder> {
  _$ChecklistItemRequest? _$v;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  bool? _required_;
  bool? get required_ => _$this._required_;
  set required_(bool? required_) => _$this._required_ = required_;

  ChecklistItemRequestBuilder() {
    ChecklistItemRequest._defaults(this);
  }

  ChecklistItemRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _title = $v.title;
      _required_ = $v.required_;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ChecklistItemRequest other) {
    _$v = other as _$ChecklistItemRequest;
  }

  @override
  void update(void Function(ChecklistItemRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ChecklistItemRequest build() => _build();

  _$ChecklistItemRequest _build() {
    final _$result = _$v ??
        _$ChecklistItemRequest._(
          title: title,
          required_: required_,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
