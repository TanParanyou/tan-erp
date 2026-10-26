//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_pagination_response.dart';
import 'package:tan_erp_api/src/model/role_definition_summary_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'role_definition_list_response.g.dart';

/// RoleDefinitionListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class RoleDefinitionListResponse implements Built<RoleDefinitionListResponse, RoleDefinitionListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<RoleDefinitionSummaryResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  AdminPaginationResponse? get pagination;

  RoleDefinitionListResponse._();

  factory RoleDefinitionListResponse([void updates(RoleDefinitionListResponseBuilder b)]) = _$RoleDefinitionListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RoleDefinitionListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RoleDefinitionListResponse> get serializer => _$RoleDefinitionListResponseSerializer();
}

class _$RoleDefinitionListResponseSerializer implements PrimitiveSerializer<RoleDefinitionListResponse> {
  @override
  final Iterable<Type> types = const [RoleDefinitionListResponse, _$RoleDefinitionListResponse];

  @override
  final String wireName = r'RoleDefinitionListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RoleDefinitionListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(RoleDefinitionSummaryResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(AdminPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RoleDefinitionListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RoleDefinitionListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(RoleDefinitionSummaryResponse)]),
          ) as BuiltList<RoleDefinitionSummaryResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminPaginationResponse),
          ) as AdminPaginationResponse?;
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
  RoleDefinitionListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RoleDefinitionListResponseBuilder();
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


