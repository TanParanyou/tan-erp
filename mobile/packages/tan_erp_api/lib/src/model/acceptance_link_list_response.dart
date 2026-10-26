//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/acceptance_link_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'acceptance_link_list_response.g.dart';

/// AcceptanceLinkListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class AcceptanceLinkListResponse implements Built<AcceptanceLinkListResponse, AcceptanceLinkListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AcceptanceLinkResponse>? get items;

  AcceptanceLinkListResponse._();

  factory AcceptanceLinkListResponse([void updates(AcceptanceLinkListResponseBuilder b)]) = _$AcceptanceLinkListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AcceptanceLinkListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AcceptanceLinkListResponse> get serializer => _$AcceptanceLinkListResponseSerializer();
}

class _$AcceptanceLinkListResponseSerializer implements PrimitiveSerializer<AcceptanceLinkListResponse> {
  @override
  final Iterable<Type> types = const [AcceptanceLinkListResponse, _$AcceptanceLinkListResponse];

  @override
  final String wireName = r'AcceptanceLinkListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AcceptanceLinkListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AcceptanceLinkResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AcceptanceLinkListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AcceptanceLinkListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AcceptanceLinkResponse)]),
          ) as BuiltList<AcceptanceLinkResponse>?;
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
  AcceptanceLinkListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AcceptanceLinkListResponseBuilder();
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


