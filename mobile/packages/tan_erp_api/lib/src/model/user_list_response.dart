//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/user_list_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'user_list_response.g.dart';

/// UserListResponse
///
/// Properties:
/// * [items] 
/// * [totalCount] 
@BuiltValue()
abstract class UserListResponse implements Built<UserListResponse, UserListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<UserListItemResponse>? get items;

  @BuiltValueField(wireName: r'totalCount')
  int? get totalCount;

  UserListResponse._();

  factory UserListResponse([void updates(UserListResponseBuilder b)]) = _$UserListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UserListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UserListResponse> get serializer => _$UserListResponseSerializer();
}

class _$UserListResponseSerializer implements PrimitiveSerializer<UserListResponse> {
  @override
  final Iterable<Type> types = const [UserListResponse, _$UserListResponse];

  @override
  final String wireName = r'UserListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UserListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UserListItemResponse)]),
      );
    }
    if (object.totalCount != null) {
      yield r'totalCount';
      yield serializers.serialize(
        object.totalCount,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UserListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UserListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UserListItemResponse)]),
          ) as BuiltList<UserListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'totalCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalCount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UserListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UserListResponseBuilder();
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


