//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'deactivate_customer_request.g.dart';

/// DeactivateCustomerRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class DeactivateCustomerRequest implements Built<DeactivateCustomerRequest, DeactivateCustomerRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  DeactivateCustomerRequest._();

  factory DeactivateCustomerRequest([void updates(DeactivateCustomerRequestBuilder b)]) = _$DeactivateCustomerRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DeactivateCustomerRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DeactivateCustomerRequest> get serializer => _$DeactivateCustomerRequestSerializer();
}

class _$DeactivateCustomerRequestSerializer implements PrimitiveSerializer<DeactivateCustomerRequest> {
  @override
  final Iterable<Type> types = const [DeactivateCustomerRequest, _$DeactivateCustomerRequest];

  @override
  final String wireName = r'DeactivateCustomerRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DeactivateCustomerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    DeactivateCustomerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DeactivateCustomerRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  DeactivateCustomerRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DeactivateCustomerRequestBuilder();
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


