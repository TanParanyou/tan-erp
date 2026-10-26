//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/bom_revision_response.dart';
import 'package:tan_erp_api/src/model/production_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_response.g.dart';

/// BomResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [item] 
/// * [createdAtUtc] 
/// * [revisions] 
@BuiltValue()
abstract class BomResponse implements Built<BomResponse, BomResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'item')
  ProductionItemResponse? get item;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'revisions')
  BuiltList<BomRevisionResponse>? get revisions;

  BomResponse._();

  factory BomResponse([void updates(BomResponseBuilder b)]) = _$BomResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomResponse> get serializer => _$BomResponseSerializer();
}

class _$BomResponseSerializer implements PrimitiveSerializer<BomResponse> {
  @override
  final Iterable<Type> types = const [BomResponse, _$BomResponse];

  @override
  final String wireName = r'BomResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomResponse object, {
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
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(ProductionItemResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.revisions != null) {
      yield r'revisions';
      yield serializers.serialize(
        object.revisions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BomRevisionResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomResponseBuilder result,
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
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionItemResponse),
          ) as ProductionItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'revisions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BomRevisionResponse)]),
          ) as BuiltList<BomRevisionResponse>?;
          if (valueDes == null) continue;
          result.revisions.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomResponseBuilder();
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


