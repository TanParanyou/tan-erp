//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/supplier_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/procurement_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_list_response.g.dart';

/// SupplierListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class SupplierListResponse implements Built<SupplierListResponse, SupplierListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<SupplierResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProcurementPaginationResponse? get pagination;

  SupplierListResponse._();

  factory SupplierListResponse([void updates(SupplierListResponseBuilder b)]) = _$SupplierListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierListResponse> get serializer => _$SupplierListResponseSerializer();
}

class _$SupplierListResponseSerializer implements PrimitiveSerializer<SupplierListResponse> {
  @override
  final Iterable<Type> types = const [SupplierListResponse, _$SupplierListResponse];

  @override
  final String wireName = r'SupplierListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierResponse)]),
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
    SupplierListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierResponse)]),
          ) as BuiltList<SupplierResponse>?;
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
  SupplierListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierListResponseBuilder();
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


