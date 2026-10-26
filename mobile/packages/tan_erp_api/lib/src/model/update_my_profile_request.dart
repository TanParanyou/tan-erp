//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_my_profile_request.g.dart';

/// UpdateMyProfileRequest
///
/// Properties:
/// * [displayName] 
/// * [phoneNumber] 
/// * [preferredLocale] 
/// * [avatarFileId] 
@BuiltValue()
abstract class UpdateMyProfileRequest implements Built<UpdateMyProfileRequest, UpdateMyProfileRequestBuilder> {
  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  @BuiltValueField(wireName: r'phoneNumber')
  String? get phoneNumber;

  @BuiltValueField(wireName: r'preferredLocale')
  String? get preferredLocale;

  @BuiltValueField(wireName: r'avatarFileId')
  String? get avatarFileId;

  UpdateMyProfileRequest._();

  factory UpdateMyProfileRequest([void updates(UpdateMyProfileRequestBuilder b)]) = _$UpdateMyProfileRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateMyProfileRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateMyProfileRequest> get serializer => _$UpdateMyProfileRequestSerializer();
}

class _$UpdateMyProfileRequestSerializer implements PrimitiveSerializer<UpdateMyProfileRequest> {
  @override
  final Iterable<Type> types = const [UpdateMyProfileRequest, _$UpdateMyProfileRequest];

  @override
  final String wireName = r'UpdateMyProfileRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateMyProfileRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.phoneNumber != null) {
      yield r'phoneNumber';
      yield serializers.serialize(
        object.phoneNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.preferredLocale != null) {
      yield r'preferredLocale';
      yield serializers.serialize(
        object.preferredLocale,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.avatarFileId != null) {
      yield r'avatarFileId';
      yield serializers.serialize(
        object.avatarFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateMyProfileRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateMyProfileRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'displayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayName = valueDes;
          break;
        case r'phoneNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.phoneNumber = valueDes;
          break;
        case r'preferredLocale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.preferredLocale = valueDes;
          break;
        case r'avatarFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.avatarFileId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateMyProfileRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateMyProfileRequestBuilder();
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


