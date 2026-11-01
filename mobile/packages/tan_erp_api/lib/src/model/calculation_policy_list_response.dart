//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/calculation_policy_version_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'calculation_policy_list_response.g.dart';

/// CalculationPolicyListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class CalculationPolicyListResponse implements Built<CalculationPolicyListResponse, CalculationPolicyListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<CalculationPolicyVersionResponse>? get items;

  CalculationPolicyListResponse._();

  factory CalculationPolicyListResponse([void updates(CalculationPolicyListResponseBuilder b)]) = _$CalculationPolicyListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CalculationPolicyListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CalculationPolicyListResponse> get serializer => _$CalculationPolicyListResponseSerializer();
}

class _$CalculationPolicyListResponseSerializer implements PrimitiveSerializer<CalculationPolicyListResponse> {
  @override
  final Iterable<Type> types = const [CalculationPolicyListResponse, _$CalculationPolicyListResponse];

  @override
  final String wireName = r'CalculationPolicyListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CalculationPolicyListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CalculationPolicyVersionResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CalculationPolicyListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CalculationPolicyListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CalculationPolicyVersionResponse)]),
          ) as BuiltList<CalculationPolicyVersionResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CalculationPolicyListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CalculationPolicyListResponseBuilder();
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


