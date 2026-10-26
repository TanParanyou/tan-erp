//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/file_slot_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_upload_session_request.g.dart';

/// CreateUploadSessionRequest
///
/// Properties:
/// * [parentType] 
/// * [parentId] 
/// * [creationIntentId] 
/// * [files] 
@BuiltValue()
abstract class CreateUploadSessionRequest implements Built<CreateUploadSessionRequest, CreateUploadSessionRequestBuilder> {
  @BuiltValueField(wireName: r'parentType')
  String? get parentType;

  @BuiltValueField(wireName: r'parentId')
  String? get parentId;

  @BuiltValueField(wireName: r'creationIntentId')
  String? get creationIntentId;

  @BuiltValueField(wireName: r'files')
  BuiltList<FileSlotRequest>? get files;

  CreateUploadSessionRequest._();

  factory CreateUploadSessionRequest([void updates(CreateUploadSessionRequestBuilder b)]) = _$CreateUploadSessionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateUploadSessionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateUploadSessionRequest> get serializer => _$CreateUploadSessionRequestSerializer();
}

class _$CreateUploadSessionRequestSerializer implements PrimitiveSerializer<CreateUploadSessionRequest> {
  @override
  final Iterable<Type> types = const [CreateUploadSessionRequest, _$CreateUploadSessionRequest];

  @override
  final String wireName = r'CreateUploadSessionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateUploadSessionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.parentType != null) {
      yield r'parentType';
      yield serializers.serialize(
        object.parentType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.parentId != null) {
      yield r'parentId';
      yield serializers.serialize(
        object.parentId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creationIntentId != null) {
      yield r'creationIntentId';
      yield serializers.serialize(
        object.creationIntentId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.files != null) {
      yield r'files';
      yield serializers.serialize(
        object.files,
        specifiedType: const FullType.nullable(BuiltList, [FullType(FileSlotRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateUploadSessionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateUploadSessionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'parentType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.parentType = valueDes;
          break;
        case r'parentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.parentId = valueDes;
          break;
        case r'creationIntentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.creationIntentId = valueDes;
          break;
        case r'files':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(FileSlotRequest)]),
          ) as BuiltList<FileSlotRequest>?;
          if (valueDes == null) continue;
          result.files.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateUploadSessionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateUploadSessionRequestBuilder();
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


