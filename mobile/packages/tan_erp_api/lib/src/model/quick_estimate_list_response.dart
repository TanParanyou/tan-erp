//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/quick_estimate_list_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/quick_estimate_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quick_estimate_list_response.g.dart';

/// QuickEstimateListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class QuickEstimateListResponse implements Built<QuickEstimateListResponse, QuickEstimateListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<QuickEstimateListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  QuickEstimatePaginationResponse? get pagination;

  QuickEstimateListResponse._();

  factory QuickEstimateListResponse([void updates(QuickEstimateListResponseBuilder b)]) = _$QuickEstimateListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuickEstimateListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuickEstimateListResponse> get serializer => _$QuickEstimateListResponseSerializer();
}

class _$QuickEstimateListResponseSerializer implements PrimitiveSerializer<QuickEstimateListResponse> {
  @override
  final Iterable<Type> types = const [QuickEstimateListResponse, _$QuickEstimateListResponse];

  @override
  final String wireName = r'QuickEstimateListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuickEstimateListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(QuickEstimateListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(QuickEstimatePaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuickEstimateListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuickEstimateListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(QuickEstimateListItemResponse)]),
          ) as BuiltList<QuickEstimateListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuickEstimatePaginationResponse),
          ) as QuickEstimatePaginationResponse?;
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
  QuickEstimateListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuickEstimateListResponseBuilder();
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


