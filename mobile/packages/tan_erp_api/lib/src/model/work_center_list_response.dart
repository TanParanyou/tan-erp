//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/work_center_response.dart';
import 'package:tan_erp_api/src/model/production_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_center_list_response.g.dart';

/// WorkCenterListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class WorkCenterListResponse implements Built<WorkCenterListResponse, WorkCenterListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<WorkCenterResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProductionPaginationResponse? get pagination;

  WorkCenterListResponse._();

  factory WorkCenterListResponse([void updates(WorkCenterListResponseBuilder b)]) = _$WorkCenterListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkCenterListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkCenterListResponse> get serializer => _$WorkCenterListResponseSerializer();
}

class _$WorkCenterListResponseSerializer implements PrimitiveSerializer<WorkCenterListResponse> {
  @override
  final Iterable<Type> types = const [WorkCenterListResponse, _$WorkCenterListResponse];

  @override
  final String wireName = r'WorkCenterListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkCenterListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkCenterResponse)]),
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
    WorkCenterListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkCenterListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkCenterResponse)]),
          ) as BuiltList<WorkCenterResponse>?;
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
  WorkCenterListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkCenterListResponseBuilder();
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


