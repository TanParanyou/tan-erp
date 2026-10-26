//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_acceptance_link_request.g.dart';

/// CreateAcceptanceLinkRequest
///
/// Properties:
/// * [lifetimeDays] 
/// * [signerHint] 
/// * [recipientEmail] 
/// * [recipientPhone] 
@BuiltValue()
abstract class CreateAcceptanceLinkRequest implements Built<CreateAcceptanceLinkRequest, CreateAcceptanceLinkRequestBuilder> {
  @BuiltValueField(wireName: r'lifetimeDays')
  int? get lifetimeDays;

  @BuiltValueField(wireName: r'signerHint')
  String? get signerHint;

  @BuiltValueField(wireName: r'recipientEmail')
  String? get recipientEmail;

  @BuiltValueField(wireName: r'recipientPhone')
  String? get recipientPhone;

  CreateAcceptanceLinkRequest._();

  factory CreateAcceptanceLinkRequest([void updates(CreateAcceptanceLinkRequestBuilder b)]) = _$CreateAcceptanceLinkRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateAcceptanceLinkRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateAcceptanceLinkRequest> get serializer => _$CreateAcceptanceLinkRequestSerializer();
}

class _$CreateAcceptanceLinkRequestSerializer implements PrimitiveSerializer<CreateAcceptanceLinkRequest> {
  @override
  final Iterable<Type> types = const [CreateAcceptanceLinkRequest, _$CreateAcceptanceLinkRequest];

  @override
  final String wireName = r'CreateAcceptanceLinkRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateAcceptanceLinkRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.lifetimeDays != null) {
      yield r'lifetimeDays';
      yield serializers.serialize(
        object.lifetimeDays,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.signerHint != null) {
      yield r'signerHint';
      yield serializers.serialize(
        object.signerHint,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.recipientEmail != null) {
      yield r'recipientEmail';
      yield serializers.serialize(
        object.recipientEmail,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.recipientPhone != null) {
      yield r'recipientPhone';
      yield serializers.serialize(
        object.recipientPhone,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateAcceptanceLinkRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateAcceptanceLinkRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'lifetimeDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.lifetimeDays = valueDes;
          break;
        case r'signerHint':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signerHint = valueDes;
          break;
        case r'recipientEmail':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.recipientEmail = valueDes;
          break;
        case r'recipientPhone':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.recipientPhone = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateAcceptanceLinkRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateAcceptanceLinkRequestBuilder();
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


