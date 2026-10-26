//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/attachment_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'attachment_link_response.g.dart';

/// AttachmentLinkResponse
///
/// Properties:
/// * [id] 
/// * [ownerType] 
/// * [ownerId] 
/// * [fileId] 
/// * [purpose] 
/// * [filename] 
/// * [mediaType] 
/// * [fileSizeBytes] 
/// * [servingUrl] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [tag] 
/// * [caption] 
@BuiltValue()
abstract class AttachmentLinkResponse implements Built<AttachmentLinkResponse, AttachmentLinkResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'ownerType')
  String? get ownerType;

  @BuiltValueField(wireName: r'ownerId')
  String? get ownerId;

  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'purpose')
  String? get purpose;

  @BuiltValueField(wireName: r'filename')
  String? get filename;

  @BuiltValueField(wireName: r'mediaType')
  String? get mediaType;

  @BuiltValueField(wireName: r'fileSizeBytes')
  int? get fileSizeBytes;

  @BuiltValueField(wireName: r'servingUrl')
  String? get servingUrl;

  @BuiltValueField(wireName: r'createdBy')
  AttachmentPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'tag')
  String? get tag;

  @BuiltValueField(wireName: r'caption')
  String? get caption;

  AttachmentLinkResponse._();

  factory AttachmentLinkResponse([void updates(AttachmentLinkResponseBuilder b)]) = _$AttachmentLinkResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AttachmentLinkResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AttachmentLinkResponse> get serializer => _$AttachmentLinkResponseSerializer();
}

class _$AttachmentLinkResponseSerializer implements PrimitiveSerializer<AttachmentLinkResponse> {
  @override
  final Iterable<Type> types = const [AttachmentLinkResponse, _$AttachmentLinkResponse];

  @override
  final String wireName = r'AttachmentLinkResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AttachmentLinkResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.ownerType != null) {
      yield r'ownerType';
      yield serializers.serialize(
        object.ownerType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.ownerId != null) {
      yield r'ownerId';
      yield serializers.serialize(
        object.ownerId,
        specifiedType: const FullType(String),
      );
    }
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.purpose != null) {
      yield r'purpose';
      yield serializers.serialize(
        object.purpose,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.filename != null) {
      yield r'filename';
      yield serializers.serialize(
        object.filename,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.mediaType != null) {
      yield r'mediaType';
      yield serializers.serialize(
        object.mediaType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.fileSizeBytes != null) {
      yield r'fileSizeBytes';
      yield serializers.serialize(
        object.fileSizeBytes,
        specifiedType: const FullType(int),
      );
    }
    if (object.servingUrl != null) {
      yield r'servingUrl';
      yield serializers.serialize(
        object.servingUrl,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(AttachmentPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
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
    AttachmentLinkResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AttachmentLinkResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'ownerType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.ownerType = valueDes;
          break;
        case r'ownerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.ownerId = valueDes;
          break;
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileId = valueDes;
          break;
        case r'purpose':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purpose = valueDes;
          break;
        case r'filename':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.filename = valueDes;
          break;
        case r'mediaType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.mediaType = valueDes;
          break;
        case r'fileSizeBytes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.fileSizeBytes = valueDes;
          break;
        case r'servingUrl':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.servingUrl = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AttachmentPersonResponse),
          ) as AttachmentPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
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
  AttachmentLinkResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AttachmentLinkResponseBuilder();
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


