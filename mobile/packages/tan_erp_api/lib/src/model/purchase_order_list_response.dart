//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/procurement_pagination_response.dart';
import 'package:tan_erp_api/src/model/purchase_order_list_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_order_list_response.g.dart';

/// PurchaseOrderListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class PurchaseOrderListResponse implements Built<PurchaseOrderListResponse, PurchaseOrderListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<PurchaseOrderListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProcurementPaginationResponse? get pagination;

  PurchaseOrderListResponse._();

  factory PurchaseOrderListResponse([void updates(PurchaseOrderListResponseBuilder b)]) = _$PurchaseOrderListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseOrderListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseOrderListResponse> get serializer => _$PurchaseOrderListResponseSerializer();
}

class _$PurchaseOrderListResponseSerializer implements PrimitiveSerializer<PurchaseOrderListResponse> {
  @override
  final Iterable<Type> types = const [PurchaseOrderListResponse, _$PurchaseOrderListResponse];

  @override
  final String wireName = r'PurchaseOrderListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseOrderListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseOrderListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(ProcurementPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseOrderListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseOrderListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseOrderListItemResponse)]),
          ) as BuiltList<PurchaseOrderListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPaginationResponse),
          ) as ProcurementPaginationResponse?;
          if (valueDes == null) continue;
          result.pagination.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseOrderListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseOrderListResponseBuilder();
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


