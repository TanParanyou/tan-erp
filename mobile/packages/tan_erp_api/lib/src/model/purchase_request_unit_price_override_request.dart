//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_request_unit_price_override_request.g.dart';

/// PurchaseRequestUnitPriceOverrideRequest
///
/// Properties:
/// * [itemId] 
/// * [unitPrice] 
@BuiltValue()
abstract class PurchaseRequestUnitPriceOverrideRequest implements Built<PurchaseRequestUnitPriceOverrideRequest, PurchaseRequestUnitPriceOverrideRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'unitPrice')
  double? get unitPrice;

  PurchaseRequestUnitPriceOverrideRequest._();

  factory PurchaseRequestUnitPriceOverrideRequest([void updates(PurchaseRequestUnitPriceOverrideRequestBuilder b)]) = _$PurchaseRequestUnitPriceOverrideRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseRequestUnitPriceOverrideRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseRequestUnitPriceOverrideRequest> get serializer => _$PurchaseRequestUnitPriceOverrideRequestSerializer();
}

class _$PurchaseRequestUnitPriceOverrideRequestSerializer implements PrimitiveSerializer<PurchaseRequestUnitPriceOverrideRequest> {
  @override
  final Iterable<Type> types = const [PurchaseRequestUnitPriceOverrideRequest, _$PurchaseRequestUnitPriceOverrideRequest];

  @override
  final String wireName = r'PurchaseRequestUnitPriceOverrideRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseRequestUnitPriceOverrideRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.unitPrice != null) {
      yield r'unitPrice';
      yield serializers.serialize(
        object.unitPrice,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseRequestUnitPriceOverrideRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseRequestUnitPriceOverrideRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'unitPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitPrice = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseRequestUnitPriceOverrideRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseRequestUnitPriceOverrideRequestBuilder();
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


