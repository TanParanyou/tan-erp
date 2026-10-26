//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_contact_response.g.dart';

/// CustomerContactResponse
///
/// Properties:
/// * [name] 
/// * [roleTitle] 
/// * [phone] 
/// * [email] 
/// * [preferredChannel] 
/// * [isMasked] 
/// * [lineId] 
@BuiltValue()
abstract class CustomerContactResponse implements Built<CustomerContactResponse, CustomerContactResponseBuilder> {
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

  @BuiltValueField(wireName: r'isMasked')
  bool? get isMasked;

  @BuiltValueField(wireName: r'lineId')
  String? get lineId;

  CustomerContactResponse._();

  factory CustomerContactResponse([void updates(CustomerContactResponseBuilder b)]) = _$CustomerContactResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerContactResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerContactResponse> get serializer => _$CustomerContactResponseSerializer();
}

class _$CustomerContactResponseSerializer implements PrimitiveSerializer<CustomerContactResponse> {
  @override
  final Iterable<Type> types = const [CustomerContactResponse, _$CustomerContactResponse];

  @override
  final String wireName = r'CustomerContactResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerContactResponse object, {
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
    if (object.isMasked != null) {
      yield r'isMasked';
      yield serializers.serialize(
        object.isMasked,
        specifiedType: const FullType(bool),
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
    CustomerContactResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerContactResponseBuilder result,
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
        case r'isMasked':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isMasked = valueDes;
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
  CustomerContactResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerContactResponseBuilder();
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


