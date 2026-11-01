//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/active_session_item_dto.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'active_session_list_response.g.dart';

/// ActiveSessionListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class ActiveSessionListResponse implements Built<ActiveSessionListResponse, ActiveSessionListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<ActiveSessionItemDto>? get items;

  ActiveSessionListResponse._();

  factory ActiveSessionListResponse([void updates(ActiveSessionListResponseBuilder b)]) = _$ActiveSessionListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ActiveSessionListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ActiveSessionListResponse> get serializer => _$ActiveSessionListResponseSerializer();
}

class _$ActiveSessionListResponseSerializer implements PrimitiveSerializer<ActiveSessionListResponse> {
  @override
  final Iterable<Type> types = const [ActiveSessionListResponse, _$ActiveSessionListResponse];

  @override
  final String wireName = r'ActiveSessionListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ActiveSessionListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ActiveSessionItemDto)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ActiveSessionListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ActiveSessionListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ActiveSessionItemDto)]),
          ) as BuiltList<ActiveSessionItemDto>?;
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
  ActiveSessionListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ActiveSessionListResponseBuilder();
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


