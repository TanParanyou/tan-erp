//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_line_request.g.dart';

/// BillingLineRequest
///
/// Properties:
/// * [description] 
/// * [taxBaseAmount] 
/// * [vatRate] 
@BuiltValue()
abstract class BillingLineRequest implements Built<BillingLineRequest, BillingLineRequestBuilder> {
  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'taxBaseAmount')
  double? get taxBaseAmount;

  @BuiltValueField(wireName: r'vatRate')
  double? get vatRate;

  BillingLineRequest._();

  factory BillingLineRequest([void updates(BillingLineRequestBuilder b)]) = _$BillingLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingLineRequest> get serializer => _$BillingLineRequestSerializer();
}

class _$BillingLineRequestSerializer implements PrimitiveSerializer<BillingLineRequest> {
  @override
  final Iterable<Type> types = const [BillingLineRequest, _$BillingLineRequest];

  @override
  final String wireName = r'BillingLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxBaseAmount != null) {
      yield r'taxBaseAmount';
      yield serializers.serialize(
        object.taxBaseAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.vatRate != null) {
      yield r'vatRate';
      yield serializers.serialize(
        object.vatRate,
        specifiedType: const FullType.nullable(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingLineRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'taxBaseAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxBaseAmount = valueDes;
          break;
        case r'vatRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatRate = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BillingLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingLineRequestBuilder();
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


