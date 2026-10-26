// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_template_version_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyTemplateVersionListResponse
    extends SurveyTemplateVersionListResponse {
  @override
  final BuiltList<SurveyTemplateVersionResponse>? items;

  factory _$SurveyTemplateVersionListResponse(
          [void Function(SurveyTemplateVersionListResponseBuilder)? updates]) =>
      (SurveyTemplateVersionListResponseBuilder()..update(updates))._build();

  _$SurveyTemplateVersionListResponse._({this.items}) : super._();
  @override
  SurveyTemplateVersionListResponse rebuild(
          void Function(SurveyTemplateVersionListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyTemplateVersionListResponseBuilder toBuilder() =>
      SurveyTemplateVersionListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyTemplateVersionListResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyTemplateVersionListResponse')
          ..add('items', items))
        .toString();
  }
}

class SurveyTemplateVersionListResponseBuilder
    implements
        Builder<SurveyTemplateVersionListResponse,
            SurveyTemplateVersionListResponseBuilder> {
  _$SurveyTemplateVersionListResponse? _$v;

  ListBuilder<SurveyTemplateVersionResponse>? _items;
  ListBuilder<SurveyTemplateVersionResponse> get items =>
      _$this._items ??= ListBuilder<SurveyTemplateVersionResponse>();
  set items(ListBuilder<SurveyTemplateVersionResponse>? items) =>
      _$this._items = items;

  SurveyTemplateVersionListResponseBuilder() {
    SurveyTemplateVersionListResponse._defaults(this);
  }

  SurveyTemplateVersionListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyTemplateVersionListResponse other) {
    _$v = other as _$SurveyTemplateVersionListResponse;
  }

  @override
  void update(
      void Function(SurveyTemplateVersionListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyTemplateVersionListResponse build() => _build();

  _$SurveyTemplateVersionListResponse _build() {
    _$SurveyTemplateVersionListResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyTemplateVersionListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SurveyTemplateVersionListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
