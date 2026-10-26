//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'attach_item_image_request.g.dart';

/// AttachItemImageRequest
///
/// Properties:
/// * [fileId] 
/// * [role] 
/// * [isPrimary] 
/// * [altText] 
/// * [caption] 
@BuiltValue()
abstract class AttachItemImageRequest implements Built<AttachItemImageRequest, AttachItemImageRequestBuilder> {
  @BuiltValueField(wireName: r'fileId')
  String get fileId;

  @BuiltValueField(wireName: r'role')
  String? get role;

  @BuiltValueField(wireName: r'isPrimary')
  bool? get isPrimary;

  @BuiltValueField(wireName: r'altText')
  LocalizedTextInput get altText;

  @BuiltValueField(wireName: r'caption')
  LocalizedTextInput? get caption;

  AttachItemImageRequest._();

  factory AttachItemImageRequest([void updates(AttachItemImageRequestBuilder b)]) = _$AttachItemImageRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AttachItemImageRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AttachItemImageRequest> get serializer => _$AttachItemImageRequestSerializer();
}

class _$AttachItemImageRequestSerializer implements PrimitiveSerializer<AttachItemImageRequest> {
  @override
  final Iterable<Type> types = const [AttachItemImageRequest, _$AttachItemImageRequest];

  @override
  final String wireName = r'AttachItemImageRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AttachItemImageRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'fileId';
    yield serializers.serialize(
      object.fileId,
      specifiedType: const FullType(String),
    );
    if (object.role != null) {
      yield r'role';
      yield serializers.serialize(
        object.role,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isPrimary != null) {
      yield r'isPrimary';
      yield serializers.serialize(
        object.isPrimary,
        specifiedType: const FullType(bool),
      );
    }
    yield r'altText';
    yield serializers.serialize(
      object.altText,
      specifiedType: const FullType(LocalizedTextInput),
    );
    if (object.caption != null) {
      yield r'caption';
      yield serializers.serialize(
        object.caption,
        specifiedType: const FullType(LocalizedTextInput),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AttachItemImageRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AttachItemImageRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.fileId = valueDes;
          break;
        case r'role':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.role = valueDes;
          break;
        case r'isPrimary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isPrimary = valueDes;
          break;
        case r'altText':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(LocalizedTextInput),
          ) as LocalizedTextInput;
          result.altText.replace(valueDes);
          break;
        case r'caption':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextInput),
          ) as LocalizedTextInput?;
          if (valueDes == null) continue;
          result.caption.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AttachItemImageRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AttachItemImageRequestBuilder();
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


