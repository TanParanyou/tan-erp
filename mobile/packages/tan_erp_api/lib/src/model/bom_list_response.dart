//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/bom_list_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/production_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_list_response.g.dart';

/// BomListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class BomListResponse implements Built<BomListResponse, BomListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<BomListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProductionPaginationResponse? get pagination;

  BomListResponse._();

  factory BomListResponse([void updates(BomListResponseBuilder b)]) = _$BomListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomListResponse> get serializer => _$BomListResponseSerializer();
}

class _$BomListResponseSerializer implements PrimitiveSerializer<BomListResponse> {
  @override
  final Iterable<Type> types = const [BomListResponse, _$BomListResponse];

  @override
  final String wireName = r'BomListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BomListItemResponse)]),
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
    BomListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BomListItemResponse)]),
          ) as BuiltList<BomListItemResponse>?;
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
  BomListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomListResponseBuilder();
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


