//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'attach_work_image_item_request.g.dart';

/// AttachWorkImageItemRequest
///
/// Properties:
/// * [fileId] 
/// * [caption] 
@BuiltValue()
abstract class AttachWorkImageItemRequest implements Built<AttachWorkImageItemRequest, AttachWorkImageItemRequestBuilder> {
  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'caption')
  String? get caption;

  AttachWorkImageItemRequest._();

  factory AttachWorkImageItemRequest([void updates(AttachWorkImageItemRequestBuilder b)]) = _$AttachWorkImageItemRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AttachWorkImageItemRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AttachWorkImageItemRequest> get serializer => _$AttachWorkImageItemRequestSerializer();
}

class _$AttachWorkImageItemRequestSerializer implements PrimitiveSerializer<AttachWorkImageItemRequest> {
  @override
  final Iterable<Type> types = const [AttachWorkImageItemRequest, _$AttachWorkImageItemRequest];

  @override
  final String wireName = r'AttachWorkImageItemRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AttachWorkImageItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.caption != null) {
      yield r'caption';
      yield serializers.serialize(
        object.caption,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AttachWorkImageItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AttachWorkImageItemRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileId = valueDes;
          break;
        case r'caption':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.caption = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AttachWorkImageItemRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AttachWorkImageItemRequestBuilder();
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


