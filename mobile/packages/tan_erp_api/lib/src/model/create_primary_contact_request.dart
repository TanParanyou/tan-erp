//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_primary_contact_request.g.dart';

/// CreatePrimaryContactRequest
///
/// Properties:
/// * [name] 
/// * [roleTitle] 
/// * [phone] 
/// * [email] 
/// * [preferredChannel] 
/// * [lineId] 
@BuiltValue()
abstract class CreatePrimaryContactRequest implements Built<CreatePrimaryContactRequest, CreatePrimaryContactRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'roleTitle')
  String? get roleTitle;

  @BuiltValueField(wireName: r'phone')
  String? get phone;

  @BuiltValueField(wireName: r'email')
  String? get email;

  @BuiltValueField(wireName: r'preferredChannel')
  String? get preferredChannel;

  @BuiltValueField(wireName: r'lineId')
  String? get lineId;

  CreatePrimaryContactRequest._();

  factory CreatePrimaryContactRequest([void updates(CreatePrimaryContactRequestBuilder b)]) = _$CreatePrimaryContactRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreatePrimaryContactRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreatePrimaryContactRequest> get serializer => _$CreatePrimaryContactRequestSerializer();
}

class _$CreatePrimaryContactRequestSerializer implements PrimitiveSerializer<CreatePrimaryContactRequest> {
  @override
  final Iterable<Type> types = const [CreatePrimaryContactRequest, _$CreatePrimaryContactRequest];

  @override
  final String wireName = r'CreatePrimaryContactRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreatePrimaryContactRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.roleTitle != null) {
      yield r'roleTitle';
      yield serializers.serialize(
        object.roleTitle,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.phone != null) {
      yield r'phone';
      yield serializers.serialize(
        object.phone,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.email != null) {
      yield r'email';
      yield serializers.serialize(
        object.email,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.preferredChannel != null) {
      yield r'preferredChannel';
      yield serializers.serialize(
        object.preferredChannel,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lineId != null) {
      yield r'lineId';
      yield serializers.serialize(
        object.lineId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreatePrimaryContactRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreatePrimaryContactRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'roleTitle':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roleTitle = valueDes;
          break;
        case r'phone':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.phone = valueDes;
          break;
        case r'email':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.email = valueDes;
          break;
        case r'preferredChannel':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.preferredChannel = valueDes;
          break;
        case r'lineId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lineId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreatePrimaryContactRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreatePrimaryContactRequestBuilder();
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


