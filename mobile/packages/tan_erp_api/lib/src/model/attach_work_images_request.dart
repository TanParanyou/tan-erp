//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/attach_work_image_item_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'attach_work_images_request.g.dart';

/// AttachWorkImagesRequest
///
/// Properties:
/// * [images] 
@BuiltValue()
abstract class AttachWorkImagesRequest implements Built<AttachWorkImagesRequest, AttachWorkImagesRequestBuilder> {
  @BuiltValueField(wireName: r'images')
  BuiltList<AttachWorkImageItemRequest>? get images;

  AttachWorkImagesRequest._();

  factory AttachWorkImagesRequest([void updates(AttachWorkImagesRequestBuilder b)]) = _$AttachWorkImagesRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AttachWorkImagesRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AttachWorkImagesRequest> get serializer => _$AttachWorkImagesRequestSerializer();
}

class _$AttachWorkImagesRequestSerializer implements PrimitiveSerializer<AttachWorkImagesRequest> {
  @override
  final Iterable<Type> types = const [AttachWorkImagesRequest, _$AttachWorkImagesRequest];

  @override
  final String wireName = r'AttachWorkImagesRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AttachWorkImagesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.images != null) {
      yield r'images';
      yield serializers.serialize(
        object.images,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AttachWorkImageItemRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AttachWorkImagesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AttachWorkImagesRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'images':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AttachWorkImageItemRequest)]),
          ) as BuiltList<AttachWorkImageItemRequest>?;
          if (valueDes == null) continue;
          result.images.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AttachWorkImagesRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AttachWorkImagesRequestBuilder();
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


