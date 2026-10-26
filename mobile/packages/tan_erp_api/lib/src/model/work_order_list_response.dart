//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_order_list_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/production_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_list_response.g.dart';

/// WorkOrderListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class WorkOrderListResponse implements Built<WorkOrderListResponse, WorkOrderListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<WorkOrderListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProductionPaginationResponse? get pagination;

  WorkOrderListResponse._();

  factory WorkOrderListResponse([void updates(WorkOrderListResponseBuilder b)]) = _$WorkOrderListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderListResponse> get serializer => _$WorkOrderListResponseSerializer();
}

class _$WorkOrderListResponseSerializer implements PrimitiveSerializer<WorkOrderListResponse> {
  @override
  final Iterable<Type> types = const [WorkOrderListResponse, _$WorkOrderListResponse];

  @override
  final String wireName = r'WorkOrderListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(ProductionPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderListItemResponse)]),
          ) as BuiltList<WorkOrderListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionPaginationResponse),
          ) as ProductionPaginationResponse?;
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
  WorkOrderListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderListResponseBuilder();
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


