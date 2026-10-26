//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/role_change_request_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'role_change_request_list_response.g.dart';

/// RoleChangeRequestListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class RoleChangeRequestListResponse implements Built<RoleChangeRequestListResponse, RoleChangeRequestListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<RoleChangeRequestResponse>? get items;

  RoleChangeRequestListResponse._();

  factory RoleChangeRequestListResponse([void updates(RoleChangeRequestListResponseBuilder b)]) = _$RoleChangeRequestListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RoleChangeRequestListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RoleChangeRequestListResponse> get serializer => _$RoleChangeRequestListResponseSerializer();
}

class _$RoleChangeRequestListResponseSerializer implements PrimitiveSerializer<RoleChangeRequestListResponse> {
  @override
  final Iterable<Type> types = const [RoleChangeRequestListResponse, _$RoleChangeRequestListResponse];

  @override
  final String wireName = r'RoleChangeRequestListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RoleChangeRequestListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(RoleChangeRequestResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RoleChangeRequestListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RoleChangeRequestListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(RoleChangeRequestResponse)]),
          ) as BuiltList<RoleChangeRequestResponse>?;
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
  RoleChangeRequestListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RoleChangeRequestListResponseBuilder();
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


