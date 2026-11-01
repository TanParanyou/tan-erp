//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'enable_two_factor_request.g.dart';

/// EnableTwoFactorRequest
///
/// Properties:
/// * [code] 
@BuiltValue()
abstract class EnableTwoFactorRequest implements Built<EnableTwoFactorRequest, EnableTwoFactorRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  EnableTwoFactorRequest._();

  factory EnableTwoFactorRequest([void updates(EnableTwoFactorRequestBuilder b)]) = _$EnableTwoFactorRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EnableTwoFactorRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EnableTwoFactorRequest> get serializer => _$EnableTwoFactorRequestSerializer();
}

class _$EnableTwoFactorRequestSerializer implements PrimitiveSerializer<EnableTwoFactorRequest> {
  @override
  final Iterable<Type> types = const [EnableTwoFactorRequest, _$EnableTwoFactorRequest];

  @override
  final String wireName = r'EnableTwoFactorRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EnableTwoFactorRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EnableTwoFactorRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EnableTwoFactorRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EnableTwoFactorRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EnableTwoFactorRequestBuilder();
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


