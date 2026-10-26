//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'deactivate_item_request.g.dart';

/// DeactivateItemRequest
///
/// Properties:
/// * [reasonCode] 
/// * [reason] 
@BuiltValue()
abstract class DeactivateItemRequest implements Built<DeactivateItemRequest, DeactivateItemRequestBuilder> {
  @BuiltValueField(wireName: r'reasonCode')
  String get reasonCode;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  DeactivateItemRequest._();

  factory DeactivateItemRequest([void updates(DeactivateItemRequestBuilder b)]) = _$DeactivateItemRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DeactivateItemRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DeactivateItemRequest> get serializer => _$DeactivateItemRequestSerializer();
}

class _$DeactivateItemRequestSerializer implements PrimitiveSerializer<DeactivateItemRequest> {
  @override
  final Iterable<Type> types = const [DeactivateItemRequest, _$DeactivateItemRequest];

  @override
  final String wireName = r'DeactivateItemRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DeactivateItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'reasonCode';
    yield serializers.serialize(
      object.reasonCode,
      specifiedType: const FullType(String),
    );
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    DeactivateItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DeactivateItemRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.reasonCode = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  DeactivateItemRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DeactivateItemRequestBuilder();
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


