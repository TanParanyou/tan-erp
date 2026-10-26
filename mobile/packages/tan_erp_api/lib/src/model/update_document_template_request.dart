//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_document_template_request.g.dart';

/// UpdateDocumentTemplateRequest
///
/// Properties:
/// * [name] 
/// * [isDefault] 
/// * [layoutConfigJson] 
/// * [logoFileId] 
@BuiltValue()
abstract class UpdateDocumentTemplateRequest implements Built<UpdateDocumentTemplateRequest, UpdateDocumentTemplateRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'isDefault')
  bool? get isDefault;

  @BuiltValueField(wireName: r'layoutConfigJson')
  String? get layoutConfigJson;

  @BuiltValueField(wireName: r'logoFileId')
  String? get logoFileId;

  UpdateDocumentTemplateRequest._();

  factory UpdateDocumentTemplateRequest([void updates(UpdateDocumentTemplateRequestBuilder b)]) = _$UpdateDocumentTemplateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateDocumentTemplateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateDocumentTemplateRequest> get serializer => _$UpdateDocumentTemplateRequestSerializer();
}

class _$UpdateDocumentTemplateRequestSerializer implements PrimitiveSerializer<UpdateDocumentTemplateRequest> {
  @override
  final Iterable<Type> types = const [UpdateDocumentTemplateRequest, _$UpdateDocumentTemplateRequest];

  @override
  final String wireName = r'UpdateDocumentTemplateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateDocumentTemplateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isDefault != null) {
      yield r'isDefault';
      yield serializers.serialize(
        object.isDefault,
        specifiedType: const FullType(bool),
      );
    }
    if (object.layoutConfigJson != null) {
      yield r'layoutConfigJson';
      yield serializers.serialize(
        object.layoutConfigJson,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.logoFileId != null) {
      yield r'logoFileId';
      yield serializers.serialize(
        object.logoFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateDocumentTemplateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateDocumentTemplateRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'isDefault':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isDefault = valueDes;
          break;
        case r'layoutConfigJson':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.layoutConfigJson = valueDes;
          break;
        case r'logoFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.logoFileId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateDocumentTemplateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateDocumentTemplateRequestBuilder();
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


