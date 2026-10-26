// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_reference_data_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyReferenceDataResponse extends SurveyReferenceDataResponse {
  @override
  final BuiltList<SurveyCategoryOptionResponse>? categories;
  @override
  final BuiltList<SurveyPhotoTagOptionResponse>? photoTags;
  @override
  final BuiltList<SurveyMeasurementTypeOptionResponse>? measurementTypes;
  @override
  final BuiltList<SurveyDesignStyleOptionResponse>? designStyles;
  @override
  final BuiltList<SurveyMoodToneOptionResponse>? moodTones;

  factory _$SurveyReferenceDataResponse(
          [void Function(SurveyReferenceDataResponseBuilder)? updates]) =>
      (SurveyReferenceDataResponseBuilder()..update(updates))._build();

  _$SurveyReferenceDataResponse._(
      {this.categories,
      this.photoTags,
      this.measurementTypes,
      this.designStyles,
      this.moodTones})
      : super._();
  @override
  SurveyReferenceDataResponse rebuild(
          void Function(SurveyReferenceDataResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyReferenceDataResponseBuilder toBuilder() =>
      SurveyReferenceDataResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyReferenceDataResponse &&
        categories == other.categories &&
        photoTags == other.photoTags &&
        measurementTypes == other.measurementTypes &&
        designStyles == other.designStyles &&
        moodTones == other.moodTones;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, categories.hashCode);
    _$hash = $jc(_$hash, photoTags.hashCode);
    _$hash = $jc(_$hash, measurementTypes.hashCode);
    _$hash = $jc(_$hash, designStyles.hashCode);
    _$hash = $jc(_$hash, moodTones.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyReferenceDataResponse')
          ..add('categories', categories)
          ..add('photoTags', photoTags)
          ..add('measurementTypes', measurementTypes)
          ..add('designStyles', designStyles)
          ..add('moodTones', moodTones))
        .toString();
  }
}

class SurveyReferenceDataResponseBuilder
    implements
        Builder<SurveyReferenceDataResponse,
            SurveyReferenceDataResponseBuilder> {
  _$SurveyReferenceDataResponse? _$v;

  ListBuilder<SurveyCategoryOptionResponse>? _categories;
  ListBuilder<SurveyCategoryOptionResponse> get categories =>
      _$this._categories ??= ListBuilder<SurveyCategoryOptionResponse>();
  set categories(ListBuilder<SurveyCategoryOptionResponse>? categories) =>
      _$this._categories = categories;

  ListBuilder<SurveyPhotoTagOptionResponse>? _photoTags;
  ListBuilder<SurveyPhotoTagOptionResponse> get photoTags =>
      _$this._photoTags ??= ListBuilder<SurveyPhotoTagOptionResponse>();
  set photoTags(ListBuilder<SurveyPhotoTagOptionResponse>? photoTags) =>
      _$this._photoTags = photoTags;

  ListBuilder<SurveyMeasurementTypeOptionResponse>? _measurementTypes;
  ListBuilder<SurveyMeasurementTypeOptionResponse> get measurementTypes =>
      _$this._measurementTypes ??=
          ListBuilder<SurveyMeasurementTypeOptionResponse>();
  set measurementTypes(
          ListBuilder<SurveyMeasurementTypeOptionResponse>? measurementTypes) =>
      _$this._measurementTypes = measurementTypes;

  ListBuilder<SurveyDesignStyleOptionResponse>? _designStyles;
  ListBuilder<SurveyDesignStyleOptionResponse> get designStyles =>
      _$this._designStyles ??= ListBuilder<SurveyDesignStyleOptionResponse>();
  set designStyles(
          ListBuilder<SurveyDesignStyleOptionResponse>? designStyles) =>
      _$this._designStyles = designStyles;

  ListBuilder<SurveyMoodToneOptionResponse>? _moodTones;
  ListBuilder<SurveyMoodToneOptionResponse> get moodTones =>
      _$this._moodTones ??= ListBuilder<SurveyMoodToneOptionResponse>();
  set moodTones(ListBuilder<SurveyMoodToneOptionResponse>? moodTones) =>
      _$this._moodTones = moodTones;

  SurveyReferenceDataResponseBuilder() {
    SurveyReferenceDataResponse._defaults(this);
  }

  SurveyReferenceDataResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _categories = $v.categories?.toBuilder();
      _photoTags = $v.photoTags?.toBuilder();
      _measurementTypes = $v.measurementTypes?.toBuilder();
      _designStyles = $v.designStyles?.toBuilder();
      _moodTones = $v.moodTones?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyReferenceDataResponse other) {
    _$v = other as _$SurveyReferenceDataResponse;
  }

  @override
  void update(void Function(SurveyReferenceDataResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyReferenceDataResponse build() => _build();

  _$SurveyReferenceDataResponse _build() {
    _$SurveyReferenceDataResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyReferenceDataResponse._(
            categories: _categories?.build(),
            photoTags: _photoTags?.build(),
            measurementTypes: _measurementTypes?.build(),
            designStyles: _designStyles?.build(),
            moodTones: _moodTones?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'categories';
        _categories?.build();
        _$failedField = 'photoTags';
        _photoTags?.build();
        _$failedField = 'measurementTypes';
        _measurementTypes?.build();
        _$failedField = 'designStyles';
        _designStyles?.build();
        _$failedField = 'moodTones';
        _moodTones?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SurveyReferenceDataResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
