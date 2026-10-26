//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'complexity_request.g.dart';

/// ComplexityRequest
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [factor] 
/// * [riskModifier] 
@BuiltValue()
abstract class ComplexityRequest implements Built<ComplexityRequest, ComplexityRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'factor')
  double? get factor;

  @BuiltValueField(wireName: r'riskModifier')
  double? get riskModifier;

  ComplexityRequest._();

  factory ComplexityRequest([void updates(ComplexityRequestBuilder b)]) = _$ComplexityRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ComplexityRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ComplexityRequest> get serializer => _$ComplexityRequestSerializer();
}

class _$ComplexityRequestSerializer implements PrimitiveSerializer<ComplexityRequest> {
  @override
  final Iterable<Type> types = const [ComplexityRequest, _$ComplexityRequest];

  @override
  final String wireName = r'ComplexityRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ComplexityRequest object, {
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
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.factor != null) {
      yield r'factor';
      yield serializers.serialize(
        object.factor,
        specifiedType: const FullType(double),
      );
    }
    if (object.riskModifier != null) {
      yield r'riskModifier';
      yield serializers.serialize(
        object.riskModifier,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ComplexityRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ComplexityRequestBuilder result,
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
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'factor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.factor = valueDes;
          break;
        case r'riskModifier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.riskModifier = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ComplexityRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ComplexityRequestBuilder();
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


