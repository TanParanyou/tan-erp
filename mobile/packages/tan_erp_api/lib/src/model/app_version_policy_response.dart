//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'app_version_policy_response.g.dart';

/// AppVersionPolicyResponse
///
/// Properties:
/// * [minimumVersion] 
/// * [latestVersion] 
@BuiltValue()
abstract class AppVersionPolicyResponse implements Built<AppVersionPolicyResponse, AppVersionPolicyResponseBuilder> {
  @BuiltValueField(wireName: r'minimumVersion')
  String? get minimumVersion;

  @BuiltValueField(wireName: r'latestVersion')
  String? get latestVersion;

  AppVersionPolicyResponse._();

  factory AppVersionPolicyResponse([void updates(AppVersionPolicyResponseBuilder b)]) = _$AppVersionPolicyResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AppVersionPolicyResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AppVersionPolicyResponse> get serializer => _$AppVersionPolicyResponseSerializer();
}

class _$AppVersionPolicyResponseSerializer implements PrimitiveSerializer<AppVersionPolicyResponse> {
  @override
  final Iterable<Type> types = const [AppVersionPolicyResponse, _$AppVersionPolicyResponse];

  @override
  final String wireName = r'AppVersionPolicyResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AppVersionPolicyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.minimumVersion != null) {
      yield r'minimumVersion';
      yield serializers.serialize(
        object.minimumVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.latestVersion != null) {
      yield r'latestVersion';
      yield serializers.serialize(
        object.latestVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AppVersionPolicyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AppVersionPolicyResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'minimumVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.minimumVersion = valueDes;
          break;
        case r'latestVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.latestVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AppVersionPolicyResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AppVersionPolicyResponseBuilder();
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


