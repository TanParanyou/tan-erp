//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_avatar_response.g.dart';

/// AdminAvatarResponse
///
/// Properties:
/// * [id] 
/// * [servingUrl] 
@BuiltValue()
abstract class AdminAvatarResponse implements Built<AdminAvatarResponse, AdminAvatarResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'servingUrl')
  String? get servingUrl;

  AdminAvatarResponse._();

  factory AdminAvatarResponse([void updates(AdminAvatarResponseBuilder b)]) = _$AdminAvatarResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminAvatarResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminAvatarResponse> get serializer => _$AdminAvatarResponseSerializer();
}

class _$AdminAvatarResponseSerializer implements PrimitiveSerializer<AdminAvatarResponse> {
  @override
  final Iterable<Type> types = const [AdminAvatarResponse, _$AdminAvatarResponse];

  @override
  final String wireName = r'AdminAvatarResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminAvatarResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.servingUrl != null) {
      yield r'servingUrl';
      yield serializers.serialize(
        object.servingUrl,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminAvatarResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminAvatarResponseBuilder result,
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
        case r'servingUrl':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.servingUrl = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AdminAvatarResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminAvatarResponseBuilder();
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


