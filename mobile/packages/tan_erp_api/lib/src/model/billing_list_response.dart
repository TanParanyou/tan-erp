//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/billing_list_item_response.dart';
import 'package:tan_erp_api/src/model/finance_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_list_response.g.dart';

/// BillingListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class BillingListResponse implements Built<BillingListResponse, BillingListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<BillingListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  FinancePaginationResponse? get pagination;

  BillingListResponse._();

  factory BillingListResponse([void updates(BillingListResponseBuilder b)]) = _$BillingListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingListResponse> get serializer => _$BillingListResponseSerializer();
}

class _$BillingListResponseSerializer implements PrimitiveSerializer<BillingListResponse> {
  @override
  final Iterable<Type> types = const [BillingListResponse, _$BillingListResponse];

  @override
  final String wireName = r'BillingListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BillingListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(FinancePaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BillingListItemResponse)]),
          ) as BuiltList<BillingListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePaginationResponse),
          ) as FinancePaginationResponse?;
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
  BillingListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingListResponseBuilder();
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


