//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/survey_localized_label.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_measurement_type_option_response.g.dart';

/// SurveyMeasurementTypeOptionResponse
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [unitCode] 
/// * [sortOrder] 
@BuiltValue()
abstract class SurveyMeasurementTypeOptionResponse implements Built<SurveyMeasurementTypeOptionResponse, SurveyMeasurementTypeOptionResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  SurveyLocalizedLabel? get name;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  SurveyMeasurementTypeOptionResponse._();

  factory SurveyMeasurementTypeOptionResponse([void updates(SurveyMeasurementTypeOptionResponseBuilder b)]) = _$SurveyMeasurementTypeOptionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyMeasurementTypeOptionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyMeasurementTypeOptionResponse> get serializer => _$SurveyMeasurementTypeOptionResponseSerializer();
}

class _$SurveyMeasurementTypeOptionResponseSerializer implements PrimitiveSerializer<SurveyMeasurementTypeOptionResponse> {
  @override
  final Iterable<Type> types = const [SurveyMeasurementTypeOptionResponse, _$SurveyMeasurementTypeOptionResponse];

  @override
  final String wireName = r'SurveyMeasurementTypeOptionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyMeasurementTypeOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
        specifiedType: const FullType(SurveyLocalizedLabel),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
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
    SurveyMeasurementTypeOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyMeasurementTypeOptionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
            specifiedType: const FullType.nullable(SurveyLocalizedLabel),
          ) as SurveyLocalizedLabel?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
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
  SurveyMeasurementTypeOptionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyMeasurementTypeOptionResponseBuilder();
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


