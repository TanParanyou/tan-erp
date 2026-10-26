//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'reorder_item_images_request.g.dart';

/// ReorderItemImagesRequest
///
/// Properties:
/// * [orderedImageIds] 
@BuiltValue()
abstract class ReorderItemImagesRequest implements Built<ReorderItemImagesRequest, ReorderItemImagesRequestBuilder> {
  @BuiltValueField(wireName: r'orderedImageIds')
  BuiltList<String> get orderedImageIds;

  ReorderItemImagesRequest._();

  factory ReorderItemImagesRequest([void updates(ReorderItemImagesRequestBuilder b)]) = _$ReorderItemImagesRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReorderItemImagesRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReorderItemImagesRequest> get serializer => _$ReorderItemImagesRequestSerializer();
}

class _$ReorderItemImagesRequestSerializer implements PrimitiveSerializer<ReorderItemImagesRequest> {
  @override
  final Iterable<Type> types = const [ReorderItemImagesRequest, _$ReorderItemImagesRequest];

  @override
  final String wireName = r'ReorderItemImagesRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReorderItemImagesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'orderedImageIds';
    yield serializers.serialize(
      object.orderedImageIds,
      specifiedType: const FullType(BuiltList, [FullType(String)]),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    ReorderItemImagesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReorderItemImagesRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'orderedImageIds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(BuiltList, [FullType(String)]),
          ) as BuiltList<String>;
          result.orderedImageIds.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReorderItemImagesRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReorderItemImagesRequestBuilder();
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


