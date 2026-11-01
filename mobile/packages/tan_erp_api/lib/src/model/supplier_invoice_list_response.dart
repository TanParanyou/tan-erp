//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/supplier_invoice_list_item_response.dart';
import 'package:tan_erp_api/src/model/procurement_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_invoice_list_response.g.dart';

/// SupplierInvoiceListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class SupplierInvoiceListResponse implements Built<SupplierInvoiceListResponse, SupplierInvoiceListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<SupplierInvoiceListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProcurementPaginationResponse? get pagination;

  SupplierInvoiceListResponse._();

  factory SupplierInvoiceListResponse([void updates(SupplierInvoiceListResponseBuilder b)]) = _$SupplierInvoiceListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierInvoiceListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierInvoiceListResponse> get serializer => _$SupplierInvoiceListResponseSerializer();
}

class _$SupplierInvoiceListResponseSerializer implements PrimitiveSerializer<SupplierInvoiceListResponse> {
  @override
  final Iterable<Type> types = const [SupplierInvoiceListResponse, _$SupplierInvoiceListResponse];

  @override
  final String wireName = r'SupplierInvoiceListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierInvoiceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierInvoiceListItemResponse)]),
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
    SupplierInvoiceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierInvoiceListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierInvoiceListItemResponse)]),
          ) as BuiltList<SupplierInvoiceListItemResponse>?;
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
  SupplierInvoiceListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierInvoiceListResponseBuilder();
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


