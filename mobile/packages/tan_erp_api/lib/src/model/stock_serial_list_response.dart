//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/stock_serial_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_serial_list_response.g.dart';

/// StockSerialListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class StockSerialListResponse implements Built<StockSerialListResponse, StockSerialListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<StockSerialResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  InventoryPaginationResponse? get pagination;

  StockSerialListResponse._();

  factory StockSerialListResponse([void updates(StockSerialListResponseBuilder b)]) = _$StockSerialListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockSerialListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockSerialListResponse> get serializer => _$StockSerialListResponseSerializer();
}

class _$StockSerialListResponseSerializer implements PrimitiveSerializer<StockSerialListResponse> {
  @override
  final Iterable<Type> types = const [StockSerialListResponse, _$StockSerialListResponse];

  @override
  final String wireName = r'StockSerialListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockSerialListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockSerialResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(InventoryPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockSerialListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockSerialListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockSerialResponse)]),
          ) as BuiltList<StockSerialResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryPaginationResponse),
          ) as InventoryPaginationResponse?;
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
  StockSerialListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockSerialListResponseBuilder();
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


