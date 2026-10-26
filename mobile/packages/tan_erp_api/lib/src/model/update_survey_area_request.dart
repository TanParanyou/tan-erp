//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/update_survey_measurement_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_survey_area_request.g.dart';

/// UpdateSurveyAreaRequest
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
/// * [description] 
/// * [sortOrder] 
/// * [measurements] 
@BuiltValue()
abstract class UpdateSurveyAreaRequest implements Built<UpdateSurveyAreaRequest, UpdateSurveyAreaRequestBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'measurements')
  BuiltList<UpdateSurveyMeasurementRequest>? get measurements;

  UpdateSurveyAreaRequest._();

  factory UpdateSurveyAreaRequest([void updates(UpdateSurveyAreaRequestBuilder b)]) = _$UpdateSurveyAreaRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateSurveyAreaRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateSurveyAreaRequest> get serializer => _$UpdateSurveyAreaRequestSerializer();
}

class _$UpdateSurveyAreaRequestSerializer implements PrimitiveSerializer<UpdateSurveyAreaRequest> {
  @override
  final Iterable<Type> types = const [UpdateSurveyAreaRequest, _$UpdateSurveyAreaRequest];

  @override
  final String wireName = r'UpdateSurveyAreaRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateSurveyAreaRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
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
    if (object.measurements != null) {
      yield r'measurements';
      yield serializers.serialize(
        object.measurements,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyMeasurementRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateSurveyAreaRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateSurveyAreaRequestBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'measurements':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyMeasurementRequest)]),
          ) as BuiltList<UpdateSurveyMeasurementRequest>?;
          if (valueDes == null) continue;
          result.measurements.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateSurveyAreaRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateSurveyAreaRequestBuilder();
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


