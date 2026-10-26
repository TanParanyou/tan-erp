//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'production_item_response.g.dart';

/// ProductionItemResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [nameTh] 
/// * [unitCode] 
@BuiltValue()
abstract class ProductionItemResponse implements Built<ProductionItemResponse, ProductionItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'nameTh')
  String? get nameTh;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  ProductionItemResponse._();

  factory ProductionItemResponse([void updates(ProductionItemResponseBuilder b)]) = _$ProductionItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProductionItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProductionItemResponse> get serializer => _$ProductionItemResponseSerializer();
}

class _$ProductionItemResponseSerializer implements PrimitiveSerializer<ProductionItemResponse> {
  @override
  final Iterable<Type> types = const [ProductionItemResponse, _$ProductionItemResponse];

  @override
  final String wireName = r'ProductionItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProductionItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameTh != null) {
      yield r'nameTh';
      yield serializers.serialize(
        object.nameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProductionItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProductionItemResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'nameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameTh = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProductionItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProductionItemResponseBuilder();
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


