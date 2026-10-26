//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_site_image_request.g.dart';

/// CreateSiteImageRequest
///
/// Properties:
/// * [fileId] 
/// * [caption] 
@BuiltValue()
abstract class CreateSiteImageRequest implements Built<CreateSiteImageRequest, CreateSiteImageRequestBuilder> {
  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'caption')
  String? get caption;

  CreateSiteImageRequest._();

  factory CreateSiteImageRequest([void updates(CreateSiteImageRequestBuilder b)]) = _$CreateSiteImageRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateSiteImageRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateSiteImageRequest> get serializer => _$CreateSiteImageRequestSerializer();
}

class _$CreateSiteImageRequestSerializer implements PrimitiveSerializer<CreateSiteImageRequest> {
  @override
  final Iterable<Type> types = const [CreateSiteImageRequest, _$CreateSiteImageRequest];

  @override
  final String wireName = r'CreateSiteImageRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateSiteImageRequest object, {
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
    CreateSiteImageRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateSiteImageRequestBuilder result,
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
  CreateSiteImageRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateSiteImageRequestBuilder();
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


