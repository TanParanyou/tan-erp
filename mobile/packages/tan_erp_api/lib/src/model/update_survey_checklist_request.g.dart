// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_survey_checklist_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateSurveyChecklistRequest extends UpdateSurveyChecklistRequest {
  @override
  final String? itemCode;
  @override
  final String? result;
  @override
  final String? note;

  factory _$UpdateSurveyChecklistRequest(
          [void Function(UpdateSurveyChecklistRequestBuilder)? updates]) =>
      (UpdateSurveyChecklistRequestBuilder()..update(updates))._build();

  _$UpdateSurveyChecklistRequest._({this.itemCode, this.result, this.note})
      : super._();
  @override
  UpdateSurveyChecklistRequest rebuild(
          void Function(UpdateSurveyChecklistRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateSurveyChecklistRequestBuilder toBuilder() =>
      UpdateSurveyChecklistRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateSurveyChecklistRequest &&
        itemCode == other.itemCode &&
        result == other.result &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, result.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateSurveyChecklistRequest')
          ..add('itemCode', itemCode)
          ..add('result', result)
          ..add('note', note))
        .toString();
  }
}

class UpdateSurveyChecklistRequestBuilder
    implements
        Builder<UpdateSurveyChecklistRequest,
            UpdateSurveyChecklistRequestBuilder> {
  _$UpdateSurveyChecklistRequest? _$v;

  String? _itemCode;
  String? get itemCode => _$this._itemCode;
  set itemCode(String? itemCode) => _$this._itemCode = itemCode;

  String? _result;
  String? get result => _$this._result;
  set result(String? result) => _$this._result = result;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  UpdateSurveyChecklistRequestBuilder() {
    UpdateSurveyChecklistRequest._defaults(this);
  }

  UpdateSurveyChecklistRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemCode = $v.itemCode;
      _result = $v.result;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateSurveyChecklistRequest other) {
    _$v = other as _$UpdateSurveyChecklistRequest;
  }

  @override
  void update(void Function(UpdateSurveyChecklistRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateSurveyChecklistRequest build() => _build();

  _$UpdateSurveyChecklistRequest _build() {
    final _$result = _$v ??
        _$UpdateSurveyChecklistRequest._(
          itemCode: itemCode,
          result: result,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
