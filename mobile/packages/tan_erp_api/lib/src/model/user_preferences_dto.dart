//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'user_preferences_dto.g.dart';

/// UserPreferencesDto
///
/// Properties:
/// * [themeMode] 
/// * [density] 
/// * [textScale] 
@BuiltValue()
abstract class UserPreferencesDto implements Built<UserPreferencesDto, UserPreferencesDtoBuilder> {
  @BuiltValueField(wireName: r'themeMode')
  String? get themeMode;

  @BuiltValueField(wireName: r'density')
  String? get density;

  @BuiltValueField(wireName: r'textScale')
  int? get textScale;

  UserPreferencesDto._();

  factory UserPreferencesDto([void updates(UserPreferencesDtoBuilder b)]) = _$UserPreferencesDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UserPreferencesDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UserPreferencesDto> get serializer => _$UserPreferencesDtoSerializer();
}

class _$UserPreferencesDtoSerializer implements PrimitiveSerializer<UserPreferencesDto> {
  @override
  final Iterable<Type> types = const [UserPreferencesDto, _$UserPreferencesDto];

  @override
  final String wireName = r'UserPreferencesDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UserPreferencesDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.themeMode != null) {
      yield r'themeMode';
      yield serializers.serialize(
        object.themeMode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.density != null) {
      yield r'density';
      yield serializers.serialize(
        object.density,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.textScale != null) {
      yield r'textScale';
      yield serializers.serialize(
        object.textScale,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UserPreferencesDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UserPreferencesDtoBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'themeMode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.themeMode = valueDes;
          break;
        case r'density':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.density = valueDes;
          break;
        case r'textScale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.textScale = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UserPreferencesDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UserPreferencesDtoBuilder();
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


