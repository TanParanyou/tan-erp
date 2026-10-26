//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/item_barcode_item_response.dart';
import 'package:tan_erp_api/src/model/item_barcode_unit_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_barcode_response.g.dart';

/// ItemBarcodeResponse
///
/// Properties:
/// * [id] 
/// * [item] 
/// * [identifierType] 
/// * [value] 
/// * [unit] 
/// * [quantityInBaseUnit] 
/// * [packagingLevel] 
/// * [isPrimary] 
/// * [status] 
/// * [rowVersion] 
@BuiltValue()
abstract class ItemBarcodeResponse implements Built<ItemBarcodeResponse, ItemBarcodeResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'item')
  ItemBarcodeItemResponse? get item;

  @BuiltValueField(wireName: r'identifierType')
  String? get identifierType;

  @BuiltValueField(wireName: r'value')
  String? get value;

  @BuiltValueField(wireName: r'unit')
  ItemBarcodeUnitResponse? get unit;

  @BuiltValueField(wireName: r'quantityInBaseUnit')
  String? get quantityInBaseUnit;

  @BuiltValueField(wireName: r'packagingLevel')
  String? get packagingLevel;

  @BuiltValueField(wireName: r'isPrimary')
  bool? get isPrimary;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  ItemBarcodeResponse._();

  factory ItemBarcodeResponse([void updates(ItemBarcodeResponseBuilder b)]) = _$ItemBarcodeResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemBarcodeResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemBarcodeResponse> get serializer => _$ItemBarcodeResponseSerializer();
}

class _$ItemBarcodeResponseSerializer implements PrimitiveSerializer<ItemBarcodeResponse> {
  @override
  final Iterable<Type> types = const [ItemBarcodeResponse, _$ItemBarcodeResponse];

  @override
  final String wireName = r'ItemBarcodeResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemBarcodeResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(ItemBarcodeItemResponse),
      );
    }
    if (object.identifierType != null) {
      yield r'identifierType';
      yield serializers.serialize(
        object.identifierType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.value != null) {
      yield r'value';
      yield serializers.serialize(
        object.value,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unit != null) {
      yield r'unit';
      yield serializers.serialize(
        object.unit,
        specifiedType: const FullType(ItemBarcodeUnitResponse),
      );
    }
    if (object.quantityInBaseUnit != null) {
      yield r'quantityInBaseUnit';
      yield serializers.serialize(
        object.quantityInBaseUnit,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.packagingLevel != null) {
      yield r'packagingLevel';
      yield serializers.serialize(
        object.packagingLevel,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isPrimary != null) {
      yield r'isPrimary';
      yield serializers.serialize(
        object.isPrimary,
        specifiedType: const FullType(bool),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemBarcodeResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemBarcodeResponseBuilder result,
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
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ItemBarcodeItemResponse),
          ) as ItemBarcodeItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'identifierType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.identifierType = valueDes;
          break;
        case r'value':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.value = valueDes;
          break;
        case r'unit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ItemBarcodeUnitResponse),
          ) as ItemBarcodeUnitResponse?;
          if (valueDes == null) continue;
          result.unit.replace(valueDes);
          break;
        case r'quantityInBaseUnit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quantityInBaseUnit = valueDes;
          break;
        case r'packagingLevel':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.packagingLevel = valueDes;
          break;
        case r'isPrimary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isPrimary = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemBarcodeResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemBarcodeResponseBuilder();
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


