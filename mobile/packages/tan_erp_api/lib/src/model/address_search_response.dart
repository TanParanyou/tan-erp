//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/address_search_result_item.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'address_search_response.g.dart';

/// AddressSearchResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class AddressSearchResponse implements Built<AddressSearchResponse, AddressSearchResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AddressSearchResultItem>? get items;

  AddressSearchResponse._();

  factory AddressSearchResponse([void updates(AddressSearchResponseBuilder b)]) = _$AddressSearchResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AddressSearchResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AddressSearchResponse> get serializer => _$AddressSearchResponseSerializer();
}

class _$AddressSearchResponseSerializer implements PrimitiveSerializer<AddressSearchResponse> {
  @override
  final Iterable<Type> types = const [AddressSearchResponse, _$AddressSearchResponse];

  @override
  final String wireName = r'AddressSearchResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AddressSearchResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AddressSearchResultItem)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AddressSearchResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AddressSearchResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AddressSearchResultItem)]),
          ) as BuiltList<AddressSearchResultItem>?;
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
  AddressSearchResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AddressSearchResponseBuilder();
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


