//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/mrp_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/mrp_planning_parameter_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_planning_parameter_list_response.g.dart';

/// MrpPlanningParameterListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class MrpPlanningParameterListResponse implements Built<MrpPlanningParameterListResponse, MrpPlanningParameterListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<MrpPlanningParameterResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  MrpPaginationResponse? get pagination;

  MrpPlanningParameterListResponse._();

  factory MrpPlanningParameterListResponse([void updates(MrpPlanningParameterListResponseBuilder b)]) = _$MrpPlanningParameterListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpPlanningParameterListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpPlanningParameterListResponse> get serializer => _$MrpPlanningParameterListResponseSerializer();
}

class _$MrpPlanningParameterListResponseSerializer implements PrimitiveSerializer<MrpPlanningParameterListResponse> {
  @override
  final Iterable<Type> types = const [MrpPlanningParameterListResponse, _$MrpPlanningParameterListResponse];

  @override
  final String wireName = r'MrpPlanningParameterListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpPlanningParameterListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MrpPlanningParameterResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(MrpPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpPlanningParameterListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpPlanningParameterListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MrpPlanningParameterResponse)]),
          ) as BuiltList<MrpPlanningParameterResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpPaginationResponse),
          ) as MrpPaginationResponse?;
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
  MrpPlanningParameterListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpPlanningParameterListResponseBuilder();
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


