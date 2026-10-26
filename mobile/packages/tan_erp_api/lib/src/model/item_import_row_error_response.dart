//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_import_row_error_response.g.dart';

/// ItemImportRowErrorResponse
///
/// Properties:
/// * [field] 
/// * [code] 
@BuiltValue()
abstract class ItemImportRowErrorResponse implements Built<ItemImportRowErrorResponse, ItemImportRowErrorResponseBuilder> {
  @BuiltValueField(wireName: r'field')
  String? get field;

  @BuiltValueField(wireName: r'code')
  String? get code;

  ItemImportRowErrorResponse._();

  factory ItemImportRowErrorResponse([void updates(ItemImportRowErrorResponseBuilder b)]) = _$ItemImportRowErrorResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemImportRowErrorResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemImportRowErrorResponse> get serializer => _$ItemImportRowErrorResponseSerializer();
}

class _$ItemImportRowErrorResponseSerializer implements PrimitiveSerializer<ItemImportRowErrorResponse> {
  @override
  final Iterable<Type> types = const [ItemImportRowErrorResponse, _$ItemImportRowErrorResponse];

  @override
  final String wireName = r'ItemImportRowErrorResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemImportRowErrorResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.field != null) {
      yield r'field';
      yield serializers.serialize(
        object.field,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemImportRowErrorResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemImportRowErrorResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'field':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.field = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemImportRowErrorResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemImportRowErrorResponseBuilder();
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


