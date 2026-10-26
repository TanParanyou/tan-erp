//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_unit_option_response.g.dart';

/// EstimateUnitOptionResponse
///
/// Properties:
/// * [unitId] 
/// * [unitCode] 
/// * [isBase] 
/// * [conversionId] 
/// * [conversionScope] 
/// * [factor] 
@BuiltValue()
abstract class EstimateUnitOptionResponse implements Built<EstimateUnitOptionResponse, EstimateUnitOptionResponseBuilder> {
  @BuiltValueField(wireName: r'unitId')
  String? get unitId;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'isBase')
  bool? get isBase;

  @BuiltValueField(wireName: r'conversionId')
  String? get conversionId;

  @BuiltValueField(wireName: r'conversionScope')
  String? get conversionScope;

  @BuiltValueField(wireName: r'factor')
  String? get factor;

  EstimateUnitOptionResponse._();

  factory EstimateUnitOptionResponse([void updates(EstimateUnitOptionResponseBuilder b)]) = _$EstimateUnitOptionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateUnitOptionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateUnitOptionResponse> get serializer => _$EstimateUnitOptionResponseSerializer();
}

class _$EstimateUnitOptionResponseSerializer implements PrimitiveSerializer<EstimateUnitOptionResponse> {
  @override
  final Iterable<Type> types = const [EstimateUnitOptionResponse, _$EstimateUnitOptionResponse];

  @override
  final String wireName = r'EstimateUnitOptionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateUnitOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.unitId != null) {
      yield r'unitId';
      yield serializers.serialize(
        object.unitId,
        specifiedType: const FullType(String),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isBase != null) {
      yield r'isBase';
      yield serializers.serialize(
        object.isBase,
        specifiedType: const FullType(bool),
      );
    }
    if (object.conversionId != null) {
      yield r'conversionId';
      yield serializers.serialize(
        object.conversionId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.conversionScope != null) {
      yield r'conversionScope';
      yield serializers.serialize(
        object.conversionScope,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.factor != null) {
      yield r'factor';
      yield serializers.serialize(
        object.factor,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateUnitOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateUnitOptionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'unitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitId = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'isBase':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isBase = valueDes;
          break;
        case r'conversionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.conversionId = valueDes;
          break;
        case r'conversionScope':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.conversionScope = valueDes;
          break;
        case r'factor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.factor = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateUnitOptionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateUnitOptionResponseBuilder();
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


