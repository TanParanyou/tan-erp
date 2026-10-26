//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/admin_role_request_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_role_request_list_response.g.dart';

/// AdminRoleRequestListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class AdminRoleRequestListResponse implements Built<AdminRoleRequestListResponse, AdminRoleRequestListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AdminRoleRequestResponse>? get items;

  AdminRoleRequestListResponse._();

  factory AdminRoleRequestListResponse([void updates(AdminRoleRequestListResponseBuilder b)]) = _$AdminRoleRequestListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminRoleRequestListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminRoleRequestListResponse> get serializer => _$AdminRoleRequestListResponseSerializer();
}

class _$AdminRoleRequestListResponseSerializer implements PrimitiveSerializer<AdminRoleRequestListResponse> {
  @override
  final Iterable<Type> types = const [AdminRoleRequestListResponse, _$AdminRoleRequestListResponse];

  @override
  final String wireName = r'AdminRoleRequestListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminRoleRequestListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRoleRequestResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminRoleRequestListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminRoleRequestListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRoleRequestResponse)]),
          ) as BuiltList<AdminRoleRequestResponse>?;
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
  AdminRoleRequestListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminRoleRequestListResponseBuilder();
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


