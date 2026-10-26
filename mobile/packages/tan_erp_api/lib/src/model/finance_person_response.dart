//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'finance_person_response.g.dart';

/// FinancePersonResponse
///
/// Properties:
/// * [id] 
/// * [displayName] 
@BuiltValue()
abstract class FinancePersonResponse implements Built<FinancePersonResponse, FinancePersonResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  FinancePersonResponse._();

  factory FinancePersonResponse([void updates(FinancePersonResponseBuilder b)]) = _$FinancePersonResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(FinancePersonResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<FinancePersonResponse> get serializer => _$FinancePersonResponseSerializer();
}

class _$FinancePersonResponseSerializer implements PrimitiveSerializer<FinancePersonResponse> {
  @override
  final Iterable<Type> types = const [FinancePersonResponse, _$FinancePersonResponse];

  @override
  final String wireName = r'FinancePersonResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    FinancePersonResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    FinancePersonResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required FinancePersonResponseBuilder result,
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
        case r'displayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayName = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  FinancePersonResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = FinancePersonResponseBuilder();
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


