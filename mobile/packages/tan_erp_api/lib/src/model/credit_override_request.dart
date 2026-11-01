//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'credit_override_request.g.dart';

/// CreditOverrideRequest
///
/// Properties:
/// * [customerId] 
/// * [requestedCeilingAmount] 
/// * [reason] 
/// * [expiresOn] 
@BuiltValue()
abstract class CreditOverrideRequest implements Built<CreditOverrideRequest, CreditOverrideRequestBuilder> {
  @BuiltValueField(wireName: r'customerId')
  String? get customerId;

  @BuiltValueField(wireName: r'requestedCeilingAmount')
  double? get requestedCeilingAmount;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'expiresOn')
  Date? get expiresOn;

  CreditOverrideRequest._();

  factory CreditOverrideRequest([void updates(CreditOverrideRequestBuilder b)]) = _$CreditOverrideRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreditOverrideRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreditOverrideRequest> get serializer => _$CreditOverrideRequestSerializer();
}

class _$CreditOverrideRequestSerializer implements PrimitiveSerializer<CreditOverrideRequest> {
  @override
  final Iterable<Type> types = const [CreditOverrideRequest, _$CreditOverrideRequest];

  @override
  final String wireName = r'CreditOverrideRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreditOverrideRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.customerId != null) {
      yield r'customerId';
      yield serializers.serialize(
        object.customerId,
        specifiedType: const FullType(String),
      );
    }
    if (object.requestedCeilingAmount != null) {
      yield r'requestedCeilingAmount';
      yield serializers.serialize(
        object.requestedCeilingAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expiresOn != null) {
      yield r'expiresOn';
      yield serializers.serialize(
        object.expiresOn,
        specifiedType: const FullType.nullable(Date),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreditOverrideRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreditOverrideRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'customerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerId = valueDes;
          break;
        case r'requestedCeilingAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.requestedCeilingAmount = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'expiresOn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.expiresOn = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreditOverrideRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreditOverrideRequestBuilder();
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


