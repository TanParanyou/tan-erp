//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/outbox_message_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/finance_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'outbox_list_response.g.dart';

/// OutboxListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class OutboxListResponse implements Built<OutboxListResponse, OutboxListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<OutboxMessageResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  FinancePaginationResponse? get pagination;

  OutboxListResponse._();

  factory OutboxListResponse([void updates(OutboxListResponseBuilder b)]) = _$OutboxListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OutboxListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OutboxListResponse> get serializer => _$OutboxListResponseSerializer();
}

class _$OutboxListResponseSerializer implements PrimitiveSerializer<OutboxListResponse> {
  @override
  final Iterable<Type> types = const [OutboxListResponse, _$OutboxListResponse];

  @override
  final String wireName = r'OutboxListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OutboxListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(OutboxMessageResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(FinancePaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OutboxListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OutboxListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(OutboxMessageResponse)]),
          ) as BuiltList<OutboxMessageResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePaginationResponse),
          ) as FinancePaginationResponse?;
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
  OutboxListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OutboxListResponseBuilder();
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


