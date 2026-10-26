//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/opportunity_response.dart';
import 'package:tan_erp_api/src/model/pagination_metadata_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'opportunity_list_response.g.dart';

/// OpportunityListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class OpportunityListResponse implements Built<OpportunityListResponse, OpportunityListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<OpportunityResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  PaginationMetadataResponse? get pagination;

  OpportunityListResponse._();

  factory OpportunityListResponse([void updates(OpportunityListResponseBuilder b)]) = _$OpportunityListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OpportunityListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OpportunityListResponse> get serializer => _$OpportunityListResponseSerializer();
}

class _$OpportunityListResponseSerializer implements PrimitiveSerializer<OpportunityListResponse> {
  @override
  final Iterable<Type> types = const [OpportunityListResponse, _$OpportunityListResponse];

  @override
  final String wireName = r'OpportunityListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OpportunityListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(PaginationMetadataResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OpportunityListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OpportunityListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityResponse)]),
          ) as BuiltList<OpportunityResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PaginationMetadataResponse),
          ) as PaginationMetadataResponse?;
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
  OpportunityListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OpportunityListResponseBuilder();
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


