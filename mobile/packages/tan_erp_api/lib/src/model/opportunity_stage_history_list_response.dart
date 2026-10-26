//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/opportunity_stage_history_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'opportunity_stage_history_list_response.g.dart';

/// OpportunityStageHistoryListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class OpportunityStageHistoryListResponse implements Built<OpportunityStageHistoryListResponse, OpportunityStageHistoryListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<OpportunityStageHistoryItemResponse>? get items;

  OpportunityStageHistoryListResponse._();

  factory OpportunityStageHistoryListResponse([void updates(OpportunityStageHistoryListResponseBuilder b)]) = _$OpportunityStageHistoryListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OpportunityStageHistoryListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OpportunityStageHistoryListResponse> get serializer => _$OpportunityStageHistoryListResponseSerializer();
}

class _$OpportunityStageHistoryListResponseSerializer implements PrimitiveSerializer<OpportunityStageHistoryListResponse> {
  @override
  final Iterable<Type> types = const [OpportunityStageHistoryListResponse, _$OpportunityStageHistoryListResponse];

  @override
  final String wireName = r'OpportunityStageHistoryListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OpportunityStageHistoryListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityStageHistoryItemResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OpportunityStageHistoryListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OpportunityStageHistoryListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityStageHistoryItemResponse)]),
          ) as BuiltList<OpportunityStageHistoryItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OpportunityStageHistoryListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OpportunityStageHistoryListResponseBuilder();
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


