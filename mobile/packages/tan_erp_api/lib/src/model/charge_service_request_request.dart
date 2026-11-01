//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'charge_service_request_request.g.dart';

/// ChargeServiceRequestRequest
///
/// Properties:
/// * [amount] 
/// * [note] 
@BuiltValue()
abstract class ChargeServiceRequestRequest implements Built<ChargeServiceRequestRequest, ChargeServiceRequestRequestBuilder> {
  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'note')
  String? get note;

  ChargeServiceRequestRequest._();

  factory ChargeServiceRequestRequest([void updates(ChargeServiceRequestRequestBuilder b)]) = _$ChargeServiceRequestRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ChargeServiceRequestRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ChargeServiceRequestRequest> get serializer => _$ChargeServiceRequestRequestSerializer();
}

class _$ChargeServiceRequestRequestSerializer implements PrimitiveSerializer<ChargeServiceRequestRequest> {
  @override
  final Iterable<Type> types = const [ChargeServiceRequestRequest, _$ChargeServiceRequestRequest];

  @override
  final String wireName = r'ChargeServiceRequestRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ChargeServiceRequestRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ChargeServiceRequestRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ChargeServiceRequestRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ChargeServiceRequestRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ChargeServiceRequestRequestBuilder();
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


