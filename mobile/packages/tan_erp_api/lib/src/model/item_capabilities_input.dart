//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_capabilities_input.g.dart';

/// ItemCapabilitiesInput
///
/// Properties:
/// * [canSell] 
/// * [canCost] 
/// * [canPurchase] 
/// * [canStock] 
/// * [canProduce] 
@BuiltValue()
abstract class ItemCapabilitiesInput implements Built<ItemCapabilitiesInput, ItemCapabilitiesInputBuilder> {
  @BuiltValueField(wireName: r'canSell')
  bool? get canSell;

  @BuiltValueField(wireName: r'canCost')
  bool? get canCost;

  @BuiltValueField(wireName: r'canPurchase')
  bool? get canPurchase;

  @BuiltValueField(wireName: r'canStock')
  bool? get canStock;

  @BuiltValueField(wireName: r'canProduce')
  bool? get canProduce;

  ItemCapabilitiesInput._();

  factory ItemCapabilitiesInput([void updates(ItemCapabilitiesInputBuilder b)]) = _$ItemCapabilitiesInput;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemCapabilitiesInputBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemCapabilitiesInput> get serializer => _$ItemCapabilitiesInputSerializer();
}

class _$ItemCapabilitiesInputSerializer implements PrimitiveSerializer<ItemCapabilitiesInput> {
  @override
  final Iterable<Type> types = const [ItemCapabilitiesInput, _$ItemCapabilitiesInput];

  @override
  final String wireName = r'ItemCapabilitiesInput';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemCapabilitiesInput object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.canSell != null) {
      yield r'canSell';
      yield serializers.serialize(
        object.canSell,
        specifiedType: const FullType(bool),
      );
    }
    if (object.canCost != null) {
      yield r'canCost';
      yield serializers.serialize(
        object.canCost,
        specifiedType: const FullType(bool),
      );
    }
    if (object.canPurchase != null) {
      yield r'canPurchase';
      yield serializers.serialize(
        object.canPurchase,
        specifiedType: const FullType(bool),
      );
    }
    if (object.canStock != null) {
      yield r'canStock';
      yield serializers.serialize(
        object.canStock,
        specifiedType: const FullType(bool),
      );
    }
    if (object.canProduce != null) {
      yield r'canProduce';
      yield serializers.serialize(
        object.canProduce,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemCapabilitiesInput object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemCapabilitiesInputBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'canSell':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.canSell = valueDes;
          break;
        case r'canCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.canCost = valueDes;
          break;
        case r'canPurchase':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.canPurchase = valueDes;
          break;
        case r'canStock':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.canStock = valueDes;
          break;
        case r'canProduce':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.canProduce = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemCapabilitiesInput deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemCapabilitiesInputBuilder();
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


