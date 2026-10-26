//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'attach_files_request.g.dart';

/// AttachFilesRequest
///
/// Properties:
/// * [purpose] 
/// * [fileIds] 
/// * [tag] 
/// * [caption] 
@BuiltValue()
abstract class AttachFilesRequest implements Built<AttachFilesRequest, AttachFilesRequestBuilder> {
  @BuiltValueField(wireName: r'purpose')
  String? get purpose;

  @BuiltValueField(wireName: r'fileIds')
  BuiltList<String>? get fileIds;

  @BuiltValueField(wireName: r'tag')
  String? get tag;

  @BuiltValueField(wireName: r'caption')
  String? get caption;

  AttachFilesRequest._();

  factory AttachFilesRequest([void updates(AttachFilesRequestBuilder b)]) = _$AttachFilesRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AttachFilesRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AttachFilesRequest> get serializer => _$AttachFilesRequestSerializer();
}

class _$AttachFilesRequestSerializer implements PrimitiveSerializer<AttachFilesRequest> {
  @override
  final Iterable<Type> types = const [AttachFilesRequest, _$AttachFilesRequest];

  @override
  final String wireName = r'AttachFilesRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AttachFilesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.purpose != null) {
      yield r'purpose';
      yield serializers.serialize(
        object.purpose,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.fileIds != null) {
      yield r'fileIds';
      yield serializers.serialize(
        object.fileIds,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.tag != null) {
      yield r'tag';
      yield serializers.serialize(
        object.tag,
        specifiedType: const FullType.nullable(String),
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
    AttachFilesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AttachFilesRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'purpose':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purpose = valueDes;
          break;
        case r'fileIds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.fileIds.replace(valueDes);
          break;
        case r'tag':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.tag = valueDes;
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
  AttachFilesRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AttachFilesRequestBuilder();
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


