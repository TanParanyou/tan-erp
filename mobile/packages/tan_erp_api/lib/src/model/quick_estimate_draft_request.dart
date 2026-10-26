//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/measurement_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quick_estimate_draft_request.g.dart';

/// QuickEstimateDraftRequest
///
/// Properties:
/// * [templateId] 
/// * [propertyType] 
/// * [roomOrArea] 
/// * [gradeCode] 
/// * [complexityCodes] 
/// * [addOnCodes] 
/// * [measurementConfidence] 
/// * [customMaterial] 
/// * [measurements] 
@BuiltValue()
abstract class QuickEstimateDraftRequest implements Built<QuickEstimateDraftRequest, QuickEstimateDraftRequestBuilder> {
  @BuiltValueField(wireName: r'templateId')
  String? get templateId;

  @BuiltValueField(wireName: r'propertyType')
  String? get propertyType;

  @BuiltValueField(wireName: r'roomOrArea')
  String? get roomOrArea;

  @BuiltValueField(wireName: r'gradeCode')
  String? get gradeCode;

  @BuiltValueField(wireName: r'complexityCodes')
  BuiltList<String>? get complexityCodes;

  @BuiltValueField(wireName: r'addOnCodes')
  BuiltList<String>? get addOnCodes;

  @BuiltValueField(wireName: r'measurementConfidence')
  String? get measurementConfidence;

  @BuiltValueField(wireName: r'customMaterial')
  bool? get customMaterial;

  @BuiltValueField(wireName: r'measurements')
  BuiltList<MeasurementRequest>? get measurements;

  QuickEstimateDraftRequest._();

  factory QuickEstimateDraftRequest([void updates(QuickEstimateDraftRequestBuilder b)]) = _$QuickEstimateDraftRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuickEstimateDraftRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuickEstimateDraftRequest> get serializer => _$QuickEstimateDraftRequestSerializer();
}

class _$QuickEstimateDraftRequestSerializer implements PrimitiveSerializer<QuickEstimateDraftRequest> {
  @override
  final Iterable<Type> types = const [QuickEstimateDraftRequest, _$QuickEstimateDraftRequest];

  @override
  final String wireName = r'QuickEstimateDraftRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuickEstimateDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.templateId != null) {
      yield r'templateId';
      yield serializers.serialize(
        object.templateId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.propertyType != null) {
      yield r'propertyType';
      yield serializers.serialize(
        object.propertyType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.roomOrArea != null) {
      yield r'roomOrArea';
      yield serializers.serialize(
        object.roomOrArea,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.gradeCode != null) {
      yield r'gradeCode';
      yield serializers.serialize(
        object.gradeCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.complexityCodes != null) {
      yield r'complexityCodes';
      yield serializers.serialize(
        object.complexityCodes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.addOnCodes != null) {
      yield r'addOnCodes';
      yield serializers.serialize(
        object.addOnCodes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.measurementConfidence != null) {
      yield r'measurementConfidence';
      yield serializers.serialize(
        object.measurementConfidence,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.customMaterial != null) {
      yield r'customMaterial';
      yield serializers.serialize(
        object.customMaterial,
        specifiedType: const FullType.nullable(bool),
      );
    }
    if (object.measurements != null) {
      yield r'measurements';
      yield serializers.serialize(
        object.measurements,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MeasurementRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuickEstimateDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuickEstimateDraftRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'templateId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.templateId = valueDes;
          break;
        case r'propertyType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.propertyType = valueDes;
          break;
        case r'roomOrArea':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roomOrArea = valueDes;
          break;
        case r'gradeCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.gradeCode = valueDes;
          break;
        case r'complexityCodes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.complexityCodes.replace(valueDes);
          break;
        case r'addOnCodes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.addOnCodes.replace(valueDes);
          break;
        case r'measurementConfidence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.measurementConfidence = valueDes;
          break;
        case r'customMaterial':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.customMaterial = valueDes;
          break;
        case r'measurements':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MeasurementRequest)]),
          ) as BuiltList<MeasurementRequest>?;
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
  QuickEstimateDraftRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuickEstimateDraftRequestBuilder();
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


