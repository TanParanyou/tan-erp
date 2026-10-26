//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/admin_role_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_role_list_response.g.dart';

/// AdminRoleListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class AdminRoleListResponse implements Built<AdminRoleListResponse, AdminRoleListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AdminRoleResponse>? get items;

  AdminRoleListResponse._();

  factory AdminRoleListResponse([void updates(AdminRoleListResponseBuilder b)]) = _$AdminRoleListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminRoleListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminRoleListResponse> get serializer => _$AdminRoleListResponseSerializer();
}

class _$AdminRoleListResponseSerializer implements PrimitiveSerializer<AdminRoleListResponse> {
  @override
  final Iterable<Type> types = const [AdminRoleListResponse, _$AdminRoleListResponse];

  @override
  final String wireName = r'AdminRoleListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminRoleListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRoleResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminRoleListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminRoleListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRoleResponse)]),
          ) as BuiltList<AdminRoleResponse>?;
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
  AdminRoleListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminRoleListResponseBuilder();
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


