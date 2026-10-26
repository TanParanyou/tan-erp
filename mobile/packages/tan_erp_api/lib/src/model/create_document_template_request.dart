//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_document_template_request.g.dart';

/// CreateDocumentTemplateRequest
///
/// Properties:
/// * [documentType] 
/// * [name] 
/// * [isDefault] 
/// * [layoutConfigJson] 
/// * [logoFileId] 
/// * [branchId] 
@BuiltValue()
abstract class CreateDocumentTemplateRequest implements Built<CreateDocumentTemplateRequest, CreateDocumentTemplateRequestBuilder> {
  @BuiltValueField(wireName: r'documentType')
  String? get documentType;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'isDefault')
  bool? get isDefault;

  @BuiltValueField(wireName: r'layoutConfigJson')
  String? get layoutConfigJson;

  @BuiltValueField(wireName: r'logoFileId')
  String? get logoFileId;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  CreateDocumentTemplateRequest._();

  factory CreateDocumentTemplateRequest([void updates(CreateDocumentTemplateRequestBuilder b)]) = _$CreateDocumentTemplateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateDocumentTemplateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateDocumentTemplateRequest> get serializer => _$CreateDocumentTemplateRequestSerializer();
}

class _$CreateDocumentTemplateRequestSerializer implements PrimitiveSerializer<CreateDocumentTemplateRequest> {
  @override
  final Iterable<Type> types = const [CreateDocumentTemplateRequest, _$CreateDocumentTemplateRequest];

  @override
  final String wireName = r'CreateDocumentTemplateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateDocumentTemplateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.documentType != null) {
      yield r'documentType';
      yield serializers.serialize(
        object.documentType,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateDocumentTemplateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateDocumentTemplateRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'documentType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.documentType = valueDes;
          break;
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateDocumentTemplateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateDocumentTemplateRequestBuilder();
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


