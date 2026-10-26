//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_pagination_response.dart';
import 'package:tan_erp_api/src/model/admin_user_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_user_list_response.g.dart';

/// AdminUserListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class AdminUserListResponse implements Built<AdminUserListResponse, AdminUserListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AdminUserResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  AdminPaginationResponse? get pagination;

  AdminUserListResponse._();

  factory AdminUserListResponse([void updates(AdminUserListResponseBuilder b)]) = _$AdminUserListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminUserListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminUserListResponse> get serializer => _$AdminUserListResponseSerializer();
}

class _$AdminUserListResponseSerializer implements PrimitiveSerializer<AdminUserListResponse> {
  @override
  final Iterable<Type> types = const [AdminUserListResponse, _$AdminUserListResponse];

  @override
  final String wireName = r'AdminUserListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminUserListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminUserResponse)]),
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
    AdminUserListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminUserListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminUserResponse)]),
          ) as BuiltList<AdminUserResponse>?;
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
  AdminUserListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminUserListResponseBuilder();
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


