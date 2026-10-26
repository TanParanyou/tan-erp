//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'duplicate_customer_response.g.dart';

/// DuplicateCustomerResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [displayNameTh] 
/// * [maskedPhone] 
/// * [maskedEmail] 
@BuiltValue()
abstract class DuplicateCustomerResponse implements Built<DuplicateCustomerResponse, DuplicateCustomerResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'maskedPhone')
  String? get maskedPhone;

  @BuiltValueField(wireName: r'maskedEmail')
  String? get maskedEmail;

  DuplicateCustomerResponse._();

  factory DuplicateCustomerResponse([void updates(DuplicateCustomerResponseBuilder b)]) = _$DuplicateCustomerResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DuplicateCustomerResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DuplicateCustomerResponse> get serializer => _$DuplicateCustomerResponseSerializer();
}

class _$DuplicateCustomerResponseSerializer implements PrimitiveSerializer<DuplicateCustomerResponse> {
  @override
  final Iterable<Type> types = const [DuplicateCustomerResponse, _$DuplicateCustomerResponse];

  @override
  final String wireName = r'DuplicateCustomerResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DuplicateCustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameTh != null) {
      yield r'displayNameTh';
      yield serializers.serialize(
        object.displayNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.maskedPhone != null) {
      yield r'maskedPhone';
      yield serializers.serialize(
        object.maskedPhone,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.maskedEmail != null) {
      yield r'maskedEmail';
      yield serializers.serialize(
        object.maskedEmail,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    DuplicateCustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DuplicateCustomerResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'displayNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameTh = valueDes;
          break;
        case r'maskedPhone':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.maskedPhone = valueDes;
          break;
        case r'maskedEmail':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.maskedEmail = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  DuplicateCustomerResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DuplicateCustomerResponseBuilder();
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


