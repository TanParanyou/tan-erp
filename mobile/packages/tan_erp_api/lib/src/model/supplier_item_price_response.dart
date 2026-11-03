//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_item_price_response.g.dart';

/// SupplierItemPriceResponse
///
/// Properties:
/// * [id] 
/// * [supplierId] 
/// * [itemId] 
/// * [itemCode] 
/// * [itemNameTh] 
/// * [unitPrice] 
/// * [minOrderQuantity] 
/// * [leadTimeDays] 
/// * [effectiveFrom] 
/// * [effectiveTo] 
/// * [isPreferred] 
/// * [rowVersion] 
@BuiltValue()
abstract class SupplierItemPriceResponse implements Built<SupplierItemPriceResponse, SupplierItemPriceResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'supplierId')
  String? get supplierId;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'itemNameTh')
  String? get itemNameTh;

  @BuiltValueField(wireName: r'unitPrice')
  double? get unitPrice;

  @BuiltValueField(wireName: r'minOrderQuantity')
  double? get minOrderQuantity;

  @BuiltValueField(wireName: r'leadTimeDays')
  int? get leadTimeDays;

  @BuiltValueField(wireName: r'effectiveFrom')
  Date? get effectiveFrom;

  @BuiltValueField(wireName: r'effectiveTo')
  Date? get effectiveTo;

  @BuiltValueField(wireName: r'isPreferred')
  bool? get isPreferred;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  SupplierItemPriceResponse._();

  factory SupplierItemPriceResponse([void updates(SupplierItemPriceResponseBuilder b)]) = _$SupplierItemPriceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierItemPriceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierItemPriceResponse> get serializer => _$SupplierItemPriceResponseSerializer();
}

class _$SupplierItemPriceResponseSerializer implements PrimitiveSerializer<SupplierItemPriceResponse> {
  @override
  final Iterable<Type> types = const [SupplierItemPriceResponse, _$SupplierItemPriceResponse];

  @override
  final String wireName = r'SupplierItemPriceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierItemPriceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.supplierId != null) {
      yield r'supplierId';
      yield serializers.serialize(
        object.supplierId,
        specifiedType: const FullType(String),
      );
    }
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.itemCode != null) {
      yield r'itemCode';
      yield serializers.serialize(
        object.itemCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemNameTh != null) {
      yield r'itemNameTh';
      yield serializers.serialize(
        object.itemNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitPrice != null) {
      yield r'unitPrice';
      yield serializers.serialize(
        object.unitPrice,
        specifiedType: const FullType(double),
      );
    }
    if (object.minOrderQuantity != null) {
      yield r'minOrderQuantity';
      yield serializers.serialize(
        object.minOrderQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.leadTimeDays != null) {
      yield r'leadTimeDays';
      yield serializers.serialize(
        object.leadTimeDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.effectiveFrom != null) {
      yield r'effectiveFrom';
      yield serializers.serialize(
        object.effectiveFrom,
        specifiedType: const FullType(Date),
      );
    }
    if (object.effectiveTo != null) {
      yield r'effectiveTo';
      yield serializers.serialize(
        object.effectiveTo,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.isPreferred != null) {
      yield r'isPreferred';
      yield serializers.serialize(
        object.isPreferred,
        specifiedType: const FullType(bool),
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
    SupplierItemPriceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierItemPriceResponseBuilder result,
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
        case r'supplierId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierId = valueDes;
          break;
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'itemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemCode = valueDes;
          break;
        case r'itemNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemNameTh = valueDes;
          break;
        case r'unitPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitPrice = valueDes;
          break;
        case r'minOrderQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.minOrderQuantity = valueDes;
          break;
        case r'leadTimeDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.leadTimeDays = valueDes;
          break;
        case r'effectiveFrom':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveFrom = valueDes;
          break;
        case r'effectiveTo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveTo = valueDes;
          break;
        case r'isPreferred':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isPreferred = valueDes;
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
  SupplierItemPriceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierItemPriceResponseBuilder();
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


