//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/purchase_request_unit_price_override_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'convert_purchase_requests_request.g.dart';

/// ConvertPurchaseRequestsRequest
///
/// Properties:
/// * [purchaseRequestIds] 
/// * [supplierId] 
/// * [expectedDeliveryDate] 
/// * [note] 
/// * [unitPriceOverrides] 
@BuiltValue()
abstract class ConvertPurchaseRequestsRequest implements Built<ConvertPurchaseRequestsRequest, ConvertPurchaseRequestsRequestBuilder> {
  @BuiltValueField(wireName: r'purchaseRequestIds')
  BuiltList<String>? get purchaseRequestIds;

  @BuiltValueField(wireName: r'supplierId')
  String? get supplierId;

  @BuiltValueField(wireName: r'expectedDeliveryDate')
  Date? get expectedDeliveryDate;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'unitPriceOverrides')
  BuiltList<PurchaseRequestUnitPriceOverrideRequest>? get unitPriceOverrides;

  ConvertPurchaseRequestsRequest._();

  factory ConvertPurchaseRequestsRequest([void updates(ConvertPurchaseRequestsRequestBuilder b)]) = _$ConvertPurchaseRequestsRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ConvertPurchaseRequestsRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ConvertPurchaseRequestsRequest> get serializer => _$ConvertPurchaseRequestsRequestSerializer();
}

class _$ConvertPurchaseRequestsRequestSerializer implements PrimitiveSerializer<ConvertPurchaseRequestsRequest> {
  @override
  final Iterable<Type> types = const [ConvertPurchaseRequestsRequest, _$ConvertPurchaseRequestsRequest];

  @override
  final String wireName = r'ConvertPurchaseRequestsRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ConvertPurchaseRequestsRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.purchaseRequestIds != null) {
      yield r'purchaseRequestIds';
      yield serializers.serialize(
        object.purchaseRequestIds,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.supplierId != null) {
      yield r'supplierId';
      yield serializers.serialize(
        object.supplierId,
        specifiedType: const FullType(String),
      );
    }
    if (object.expectedDeliveryDate != null) {
      yield r'expectedDeliveryDate';
      yield serializers.serialize(
        object.expectedDeliveryDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitPriceOverrides != null) {
      yield r'unitPriceOverrides';
      yield serializers.serialize(
        object.unitPriceOverrides,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestUnitPriceOverrideRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ConvertPurchaseRequestsRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ConvertPurchaseRequestsRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'purchaseRequestIds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.purchaseRequestIds.replace(valueDes);
          break;
        case r'supplierId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierId = valueDes;
          break;
        case r'expectedDeliveryDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.expectedDeliveryDate = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'unitPriceOverrides':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestUnitPriceOverrideRequest)]),
          ) as BuiltList<PurchaseRequestUnitPriceOverrideRequest>?;
          if (valueDes == null) continue;
          result.unitPriceOverrides.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ConvertPurchaseRequestsRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ConvertPurchaseRequestsRequestBuilder();
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


