//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/credit_override_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'credit_override_list_response.g.dart';

/// CreditOverrideListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class CreditOverrideListResponse implements Built<CreditOverrideListResponse, CreditOverrideListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<CreditOverrideResponse>? get items;

  CreditOverrideListResponse._();

  factory CreditOverrideListResponse([void updates(CreditOverrideListResponseBuilder b)]) = _$CreditOverrideListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreditOverrideListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreditOverrideListResponse> get serializer => _$CreditOverrideListResponseSerializer();
}

class _$CreditOverrideListResponseSerializer implements PrimitiveSerializer<CreditOverrideListResponse> {
  @override
  final Iterable<Type> types = const [CreditOverrideListResponse, _$CreditOverrideListResponse];

  @override
  final String wireName = r'CreditOverrideListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreditOverrideListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CreditOverrideResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreditOverrideListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreditOverrideListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CreditOverrideResponse)]),
          ) as BuiltList<CreditOverrideResponse>?;
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
  CreditOverrideListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreditOverrideListResponseBuilder();
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


