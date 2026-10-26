// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_survey_area_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateSurveyAreaRequest extends UpdateSurveyAreaRequest {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? description;
  @override
  final int? sortOrder;
  @override
  final BuiltList<UpdateSurveyMeasurementRequest>? measurements;

  factory _$UpdateSurveyAreaRequest(
          [void Function(UpdateSurveyAreaRequestBuilder)? updates]) =>
      (UpdateSurveyAreaRequestBuilder()..update(updates))._build();

  _$UpdateSurveyAreaRequest._(
      {this.id,
      this.code,
      this.name,
      this.description,
      this.sortOrder,
      this.measurements})
      : super._();
  @override
  UpdateSurveyAreaRequest rebuild(
          void Function(UpdateSurveyAreaRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateSurveyAreaRequestBuilder toBuilder() =>
      UpdateSurveyAreaRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateSurveyAreaRequest &&
        id == other.id &&
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
    return (newBuiltValueToStringHelper(r'UpdateSurveyAreaRequest')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('sortOrder', sortOrder)
          ..add('measurements', measurements))
        .toString();
  }
}

class UpdateSurveyAreaRequestBuilder
    implements
        Builder<UpdateSurveyAreaRequest, UpdateSurveyAreaRequestBuilder> {
  _$UpdateSurveyAreaRequest? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  ListBuilder<UpdateSurveyMeasurementRequest>? _measurements;
  ListBuilder<UpdateSurveyMeasurementRequest> get measurements =>
      _$this._measurements ??= ListBuilder<UpdateSurveyMeasurementRequest>();
  set measurements(ListBuilder<UpdateSurveyMeasurementRequest>? measurements) =>
      _$this._measurements = measurements;

  UpdateSurveyAreaRequestBuilder() {
    UpdateSurveyAreaRequest._defaults(this);
  }

  UpdateSurveyAreaRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
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
  void replace(UpdateSurveyAreaRequest other) {
    _$v = other as _$UpdateSurveyAreaRequest;
  }

  @override
  void update(void Function(UpdateSurveyAreaRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateSurveyAreaRequest build() => _build();

  _$UpdateSurveyAreaRequest _build() {
    _$UpdateSurveyAreaRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateSurveyAreaRequest._(
            id: id,
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
            r'UpdateSurveyAreaRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
