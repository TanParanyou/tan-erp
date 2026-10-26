// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_survey_area_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSurveyAreaResponse extends SiteSurveyAreaResponse {
  @override
  final String? id;
  @override
  final String? siteSurveyRevisionId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? description;
  @override
  final int? sortOrder;
  @override
  final BuiltList<SiteSurveyMeasurementResponse>? measurements;

  factory _$SiteSurveyAreaResponse(
          [void Function(SiteSurveyAreaResponseBuilder)? updates]) =>
      (SiteSurveyAreaResponseBuilder()..update(updates))._build();

  _$SiteSurveyAreaResponse._(
      {this.id,
      this.siteSurveyRevisionId,
      this.code,
      this.name,
      this.description,
      this.sortOrder,
      this.measurements})
      : super._();
  @override
  SiteSurveyAreaResponse rebuild(
          void Function(SiteSurveyAreaResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSurveyAreaResponseBuilder toBuilder() =>
      SiteSurveyAreaResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSurveyAreaResponse &&
        id == other.id &&
        siteSurveyRevisionId == other.siteSurveyRevisionId &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        sortOrder == other.sortOrder &&
        measurements == other.measurements;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, siteSurveyRevisionId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, measurements.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSurveyAreaResponse')
          ..add('id', id)
          ..add('siteSurveyRevisionId', siteSurveyRevisionId)
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder)
          ..add('measurements', measurements))
        .toString();
  }
}

class SiteSurveyAreaResponseBuilder
    implements Builder<SiteSurveyAreaResponse, SiteSurveyAreaResponseBuilder> {
  _$SiteSurveyAreaResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _siteSurveyRevisionId;
  String? get siteSurveyRevisionId => _$this._siteSurveyRevisionId;
  set siteSurveyRevisionId(String? siteSurveyRevisionId) =>
      _$this._siteSurveyRevisionId = siteSurveyRevisionId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  ListBuilder<SiteSurveyMeasurementResponse>? _measurements;
  ListBuilder<SiteSurveyMeasurementResponse> get measurements =>
      _$this._measurements ??= ListBuilder<SiteSurveyMeasurementResponse>();
  set measurements(ListBuilder<SiteSurveyMeasurementResponse>? measurements) =>
      _$this._measurements = measurements;

  SiteSurveyAreaResponseBuilder() {
    SiteSurveyAreaResponse._defaults(this);
  }

  SiteSurveyAreaResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _siteSurveyRevisionId = $v.siteSurveyRevisionId;
      _code = $v.code;
      _name = $v.name;
      _description = $v.description;
      _sortOrder = $v.sortOrder;
      _measurements = $v.measurements?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSurveyAreaResponse other) {
    _$v = other as _$SiteSurveyAreaResponse;
  }

  @override
  void update(void Function(SiteSurveyAreaResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSurveyAreaResponse build() => _build();

  _$SiteSurveyAreaResponse _build() {
    _$SiteSurveyAreaResponse _$result;
    try {
      _$result = _$v ??
          _$SiteSurveyAreaResponse._(
            id: id,
            siteSurveyRevisionId: siteSurveyRevisionId,
            code: code,
            name: name,
            description: description,
            sortOrder: sortOrder,
            measurements: _measurements?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'measurements';
        _measurements?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SiteSurveyAreaResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
