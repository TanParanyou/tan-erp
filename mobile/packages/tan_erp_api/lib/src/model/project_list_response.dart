//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_list_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/project_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_list_response.g.dart';

/// ProjectListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class ProjectListResponse implements Built<ProjectListResponse, ProjectListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<ProjectListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ProjectPaginationResponse? get pagination;

  ProjectListResponse._();

  factory ProjectListResponse([void updates(ProjectListResponseBuilder b)]) = _$ProjectListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectListResponse> get serializer => _$ProjectListResponseSerializer();
}

class _$ProjectListResponseSerializer implements PrimitiveSerializer<ProjectListResponse> {
  @override
  final Iterable<Type> types = const [ProjectListResponse, _$ProjectListResponse];

  @override
  final String wireName = r'ProjectListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(ProjectPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectListItemResponse)]),
          ) as BuiltList<ProjectListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPaginationResponse),
          ) as ProjectPaginationResponse?;
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
  ProjectListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectListResponseBuilder();
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


