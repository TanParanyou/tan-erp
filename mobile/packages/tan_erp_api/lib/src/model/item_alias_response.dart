//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_alias_response.g.dart';

/// ItemAliasResponse
///
/// Properties:
/// * [id] 
/// * [alias] 
/// * [status] 
@BuiltValue()
abstract class ItemAliasResponse implements Built<ItemAliasResponse, ItemAliasResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'alias')
  LocalizedTextResponse? get alias;

  @BuiltValueField(wireName: r'status')
  String? get status;

  ItemAliasResponse._();

  factory ItemAliasResponse([void updates(ItemAliasResponseBuilder b)]) = _$ItemAliasResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemAliasResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemAliasResponse> get serializer => _$ItemAliasResponseSerializer();
}

class _$ItemAliasResponseSerializer implements PrimitiveSerializer<ItemAliasResponse> {
  @override
  final Iterable<Type> types = const [ItemAliasResponse, _$ItemAliasResponse];

  @override
  final String wireName = r'ItemAliasResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemAliasResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.alias != null) {
      yield r'alias';
      yield serializers.serialize(
        object.alias,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemAliasResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemAliasResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'alias':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.alias.replace(valueDes);
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemAliasResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemAliasResponseBuilder();
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


