//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/purchase_request_list_item_response.dart';
import 'package:tan_erp_api/src/model/procurement_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_request_list_response.g.dart';

/// PurchaseRequestListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class PurchaseRequestListResponse implements Built<PurchaseRequestListResponse, PurchaseRequestListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<PurchaseRequestListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProcurementPaginationResponse? get pagination;

  PurchaseRequestListResponse._();

  factory PurchaseRequestListResponse([void updates(PurchaseRequestListResponseBuilder b)]) = _$PurchaseRequestListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseRequestListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseRequestListResponse> get serializer => _$PurchaseRequestListResponseSerializer();
}

class _$PurchaseRequestListResponseSerializer implements PrimitiveSerializer<PurchaseRequestListResponse> {
  @override
  final Iterable<Type> types = const [PurchaseRequestListResponse, _$PurchaseRequestListResponse];

  @override
  final String wireName = r'PurchaseRequestListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseRequestListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestListItemResponse)]),
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
    PurchaseRequestListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseRequestListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestListItemResponse)]),
          ) as BuiltList<PurchaseRequestListItemResponse>?;
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
  PurchaseRequestListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseRequestListResponseBuilder();
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


