//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_quick_estimate_request.g.dart';

/// CreateQuickEstimateRequest
///
/// Properties:
/// * [customerId] 
/// * [opportunityId] 
@BuiltValue()
abstract class CreateQuickEstimateRequest implements Built<CreateQuickEstimateRequest, CreateQuickEstimateRequestBuilder> {
  @BuiltValueField(wireName: r'customerId')
  String? get customerId;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  CreateQuickEstimateRequest._();

  factory CreateQuickEstimateRequest([void updates(CreateQuickEstimateRequestBuilder b)]) = _$CreateQuickEstimateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateQuickEstimateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateQuickEstimateRequest> get serializer => _$CreateQuickEstimateRequestSerializer();
}

class _$CreateQuickEstimateRequestSerializer implements PrimitiveSerializer<CreateQuickEstimateRequest> {
  @override
  final Iterable<Type> types = const [CreateQuickEstimateRequest, _$CreateQuickEstimateRequest];

  @override
  final String wireName = r'CreateQuickEstimateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateQuickEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.customerId != null) {
      yield r'customerId';
      yield serializers.serialize(
        object.customerId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateQuickEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateQuickEstimateRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'customerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerId = valueDes;
          break;
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateQuickEstimateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateQuickEstimateRequestBuilder();
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


