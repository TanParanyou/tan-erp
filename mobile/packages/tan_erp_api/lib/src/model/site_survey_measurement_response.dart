//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'site_survey_measurement_response.g.dart';

/// SiteSurveyMeasurementResponse
///
/// Properties:
/// * [id] 
/// * [siteSurveyAreaId] 
/// * [measurementType] 
/// * [value] 
/// * [unitCode] 
/// * [captureMethod] 
/// * [notes] 
/// * [sortOrder] 
@BuiltValue()
abstract class SiteSurveyMeasurementResponse implements Built<SiteSurveyMeasurementResponse, SiteSurveyMeasurementResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'siteSurveyAreaId')
  String? get siteSurveyAreaId;

  @BuiltValueField(wireName: r'measurementType')
  String? get measurementType;

  @BuiltValueField(wireName: r'value')
  double? get value;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'captureMethod')
  String? get captureMethod;

  @BuiltValueField(wireName: r'notes')
  String? get notes;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  SiteSurveyMeasurementResponse._();

  factory SiteSurveyMeasurementResponse([void updates(SiteSurveyMeasurementResponseBuilder b)]) = _$SiteSurveyMeasurementResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SiteSurveyMeasurementResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SiteSurveyMeasurementResponse> get serializer => _$SiteSurveyMeasurementResponseSerializer();
}

class _$SiteSurveyMeasurementResponseSerializer implements PrimitiveSerializer<SiteSurveyMeasurementResponse> {
  @override
  final Iterable<Type> types = const [SiteSurveyMeasurementResponse, _$SiteSurveyMeasurementResponse];

  @override
  final String wireName = r'SiteSurveyMeasurementResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SiteSurveyMeasurementResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.siteSurveyAreaId != null) {
      yield r'siteSurveyAreaId';
      yield serializers.serialize(
        object.siteSurveyAreaId,
        specifiedType: const FullType(String),
      );
    }
    if (object.measurementType != null) {
      yield r'measurementType';
      yield serializers.serialize(
        object.measurementType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.value != null) {
      yield r'value';
      yield serializers.serialize(
        object.value,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.captureMethod != null) {
      yield r'captureMethod';
      yield serializers.serialize(
        object.captureMethod,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.notes != null) {
      yield r'notes';
      yield serializers.serialize(
        object.notes,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SiteSurveyMeasurementResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SiteSurveyMeasurementResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'siteSurveyAreaId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveyAreaId = valueDes;
          break;
        case r'measurementType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.measurementType = valueDes;
          break;
        case r'value':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.value = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'captureMethod':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.captureMethod = valueDes;
          break;
        case r'notes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.notes = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SiteSurveyMeasurementResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SiteSurveyMeasurementResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


