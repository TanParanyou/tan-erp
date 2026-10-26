//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/pricing_template_list_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/quick_estimate_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'pricing_template_list_response.g.dart';

/// PricingTemplateListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class PricingTemplateListResponse implements Built<PricingTemplateListResponse, PricingTemplateListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<PricingTemplateListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  QuickEstimatePaginationResponse? get pagination;

  PricingTemplateListResponse._();

  factory PricingTemplateListResponse([void updates(PricingTemplateListResponseBuilder b)]) = _$PricingTemplateListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PricingTemplateListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PricingTemplateListResponse> get serializer => _$PricingTemplateListResponseSerializer();
}

class _$PricingTemplateListResponseSerializer implements PrimitiveSerializer<PricingTemplateListResponse> {
  @override
  final Iterable<Type> types = const [PricingTemplateListResponse, _$PricingTemplateListResponse];

  @override
  final String wireName = r'PricingTemplateListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PricingTemplateListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PricingTemplateListItemResponse)]),
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
    PricingTemplateListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PricingTemplateListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PricingTemplateListItemResponse)]),
          ) as BuiltList<PricingTemplateListItemResponse>?;
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
  PricingTemplateListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PricingTemplateListResponseBuilder();
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


