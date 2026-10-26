//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_my_preferences_request.g.dart';

/// UpdateMyPreferencesRequest
///
/// Properties:
/// * [themeMode] 
/// * [density] 
/// * [textScale] 
@BuiltValue()
abstract class UpdateMyPreferencesRequest implements Built<UpdateMyPreferencesRequest, UpdateMyPreferencesRequestBuilder> {
  @BuiltValueField(wireName: r'themeMode')
  String? get themeMode;

  @BuiltValueField(wireName: r'density')
  String? get density;

  @BuiltValueField(wireName: r'textScale')
  int? get textScale;

  UpdateMyPreferencesRequest._();

  factory UpdateMyPreferencesRequest([void updates(UpdateMyPreferencesRequestBuilder b)]) = _$UpdateMyPreferencesRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateMyPreferencesRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateMyPreferencesRequest> get serializer => _$UpdateMyPreferencesRequestSerializer();
}

class _$UpdateMyPreferencesRequestSerializer implements PrimitiveSerializer<UpdateMyPreferencesRequest> {
  @override
  final Iterable<Type> types = const [UpdateMyPreferencesRequest, _$UpdateMyPreferencesRequest];

  @override
  final String wireName = r'UpdateMyPreferencesRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateMyPreferencesRequest object, {
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
        specifiedType: const FullType.nullable(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateMyPreferencesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateMyPreferencesRequestBuilder result,
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
  UpdateMyPreferencesRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateMyPreferencesRequestBuilder();
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


