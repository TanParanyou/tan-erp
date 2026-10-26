// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'conversion_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ConversionRequest extends ConversionRequest {
  @override
  final int? sourceVersion;
  @override
  final String? siteSurveyRevisionId;

  factory _$ConversionRequest(
          [void Function(ConversionRequestBuilder)? updates]) =>
      (ConversionRequestBuilder()..update(updates))._build();

  _$ConversionRequest._({this.sourceVersion, this.siteSurveyRevisionId})
      : super._();
  @override
  ConversionRequest rebuild(void Function(ConversionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ConversionRequestBuilder toBuilder() =>
      ConversionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ConversionRequest &&
        sourceVersion == other.sourceVersion &&
        siteSurveyRevisionId == other.siteSurveyRevisionId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sourceVersion.hashCode);
    _$hash = $jc(_$hash, siteSurveyRevisionId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ConversionRequest')
          ..add('sourceVersion', sourceVersion)
          ..add('siteSurveyRevisionId', siteSurveyRevisionId))
        .toString();
  }
}

class ConversionRequestBuilder
    implements Builder<ConversionRequest, ConversionRequestBuilder> {
  _$ConversionRequest? _$v;

  int? _sourceVersion;
  int? get sourceVersion => _$this._sourceVersion;
  set sourceVersion(int? sourceVersion) =>
      _$this._sourceVersion = sourceVersion;

  String? _siteSurveyRevisionId;
  String? get siteSurveyRevisionId => _$this._siteSurveyRevisionId;
  set siteSurveyRevisionId(String? siteSurveyRevisionId) =>
      _$this._siteSurveyRevisionId = siteSurveyRevisionId;

  ConversionRequestBuilder() {
    ConversionRequest._defaults(this);
  }

  ConversionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sourceVersion = $v.sourceVersion;
      _siteSurveyRevisionId = $v.siteSurveyRevisionId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ConversionRequest other) {
    _$v = other as _$ConversionRequest;
  }

  @override
  void update(void Function(ConversionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ConversionRequest build() => _build();

  _$ConversionRequest _build() {
    final _$result = _$v ??
        _$ConversionRequest._(
          sourceVersion: sourceVersion,
          siteSurveyRevisionId: siteSurveyRevisionId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
