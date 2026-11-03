//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/supplier_item_price_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/procurement_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_item_price_list_response.g.dart';

/// SupplierItemPriceListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class SupplierItemPriceListResponse implements Built<SupplierItemPriceListResponse, SupplierItemPriceListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<SupplierItemPriceResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProcurementPaginationResponse? get pagination;

  SupplierItemPriceListResponse._();

  factory SupplierItemPriceListResponse([void updates(SupplierItemPriceListResponseBuilder b)]) = _$SupplierItemPriceListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierItemPriceListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierItemPriceListResponse> get serializer => _$SupplierItemPriceListResponseSerializer();
}

class _$SupplierItemPriceListResponseSerializer implements PrimitiveSerializer<SupplierItemPriceListResponse> {
  @override
  final Iterable<Type> types = const [SupplierItemPriceListResponse, _$SupplierItemPriceListResponse];

  @override
  final String wireName = r'SupplierItemPriceListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierItemPriceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierItemPriceResponse)]),
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
    SupplierItemPriceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierItemPriceListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierItemPriceResponse)]),
          ) as BuiltList<SupplierItemPriceResponse>?;
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
  SupplierItemPriceListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierItemPriceListResponseBuilder();
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


