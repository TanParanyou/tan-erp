// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_survey_checklist_result_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSurveyChecklistResultResponse
    extends SiteSurveyChecklistResultResponse {
  @override
  final String? id;
  @override
  final String? itemCode;
  @override
  final String? result;
  @override
  final String? note;

  factory _$SiteSurveyChecklistResultResponse(
          [void Function(SiteSurveyChecklistResultResponseBuilder)? updates]) =>
      (SiteSurveyChecklistResultResponseBuilder()..update(updates))._build();

  _$SiteSurveyChecklistResultResponse._(
      {this.id, this.itemCode, this.result, this.note})
      : super._();
  @override
  SiteSurveyChecklistResultResponse rebuild(
          void Function(SiteSurveyChecklistResultResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSurveyChecklistResultResponseBuilder toBuilder() =>
      SiteSurveyChecklistResultResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSurveyChecklistResultResponse &&
        id == other.id &&
        itemCode == other.itemCode &&
        result == other.result &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, result.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSurveyChecklistResultResponse')
          ..add('id', id)
          ..add('itemCode', itemCode)
          ..add('result', result)
          ..add('note', note))
        .toString();
  }
}

class SiteSurveyChecklistResultResponseBuilder
    implements
        Builder<SiteSurveyChecklistResultResponse,
            SiteSurveyChecklistResultResponseBuilder> {
  _$SiteSurveyChecklistResultResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _itemCode;
  String? get itemCode => _$this._itemCode;
  set itemCode(String? itemCode) => _$this._itemCode = itemCode;

  String? _result;
  String? get result => _$this._result;
  set result(String? result) => _$this._result = result;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  SiteSurveyChecklistResultResponseBuilder() {
    SiteSurveyChecklistResultResponse._defaults(this);
  }

  SiteSurveyChecklistResultResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _itemCode = $v.itemCode;
      _result = $v.result;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSurveyChecklistResultResponse other) {
    _$v = other as _$SiteSurveyChecklistResultResponse;
  }

  @override
  void update(
      void Function(SiteSurveyChecklistResultResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSurveyChecklistResultResponse build() => _build();

  _$SiteSurveyChecklistResultResponse _build() {
    final _$result = _$v ??
        _$SiteSurveyChecklistResultResponse._(
          id: id,
          itemCode: itemCode,
          result: result,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
