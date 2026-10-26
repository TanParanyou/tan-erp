//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_item_import_job_request.g.dart';

/// CreateItemImportJobRequest
///
/// Properties:
/// * [kind] 
/// * [mode] 
/// * [content] 
/// * [expectedContentSha256] 
@BuiltValue()
abstract class CreateItemImportJobRequest implements Built<CreateItemImportJobRequest, CreateItemImportJobRequestBuilder> {
  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'mode')
  String? get mode;

  @BuiltValueField(wireName: r'content')
  String? get content;

  @BuiltValueField(wireName: r'expectedContentSha256')
  String? get expectedContentSha256;

  CreateItemImportJobRequest._();

  factory CreateItemImportJobRequest([void updates(CreateItemImportJobRequestBuilder b)]) = _$CreateItemImportJobRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateItemImportJobRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateItemImportJobRequest> get serializer => _$CreateItemImportJobRequestSerializer();
}

class _$CreateItemImportJobRequestSerializer implements PrimitiveSerializer<CreateItemImportJobRequest> {
  @override
  final Iterable<Type> types = const [CreateItemImportJobRequest, _$CreateItemImportJobRequest];

  @override
  final String wireName = r'CreateItemImportJobRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateItemImportJobRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.mode != null) {
      yield r'mode';
      yield serializers.serialize(
        object.mode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.content != null) {
      yield r'content';
      yield serializers.serialize(
        object.content,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expectedContentSha256 != null) {
      yield r'expectedContentSha256';
      yield serializers.serialize(
        object.expectedContentSha256,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateItemImportJobRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateItemImportJobRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'mode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.mode = valueDes;
          break;
        case r'content':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.content = valueDes;
          break;
        case r'expectedContentSha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedContentSha256 = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateItemImportJobRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateItemImportJobRequestBuilder();
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


