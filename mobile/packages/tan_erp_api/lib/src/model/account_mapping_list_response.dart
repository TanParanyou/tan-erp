//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/account_mapping_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'account_mapping_list_response.g.dart';

/// AccountMappingListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class AccountMappingListResponse implements Built<AccountMappingListResponse, AccountMappingListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AccountMappingResponse>? get items;

  AccountMappingListResponse._();

  factory AccountMappingListResponse([void updates(AccountMappingListResponseBuilder b)]) = _$AccountMappingListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AccountMappingListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AccountMappingListResponse> get serializer => _$AccountMappingListResponseSerializer();
}

class _$AccountMappingListResponseSerializer implements PrimitiveSerializer<AccountMappingListResponse> {
  @override
  final Iterable<Type> types = const [AccountMappingListResponse, _$AccountMappingListResponse];

  @override
  final String wireName = r'AccountMappingListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AccountMappingListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AccountMappingResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AccountMappingListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AccountMappingListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AccountMappingResponse)]),
          ) as BuiltList<AccountMappingResponse>?;
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
  AccountMappingListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AccountMappingListResponseBuilder();
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


