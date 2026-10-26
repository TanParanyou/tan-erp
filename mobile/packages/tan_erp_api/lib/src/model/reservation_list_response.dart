//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/reservation_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'reservation_list_response.g.dart';

/// ReservationListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class ReservationListResponse implements Built<ReservationListResponse, ReservationListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<ReservationResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  InventoryPaginationResponse? get pagination;

  ReservationListResponse._();

  factory ReservationListResponse([void updates(ReservationListResponseBuilder b)]) = _$ReservationListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReservationListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReservationListResponse> get serializer => _$ReservationListResponseSerializer();
}

class _$ReservationListResponseSerializer implements PrimitiveSerializer<ReservationListResponse> {
  @override
  final Iterable<Type> types = const [ReservationListResponse, _$ReservationListResponse];

  @override
  final String wireName = r'ReservationListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReservationListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ReservationResponse)]),
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
    ReservationListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReservationListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ReservationResponse)]),
          ) as BuiltList<ReservationResponse>?;
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
  ReservationListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReservationListResponseBuilder();
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


