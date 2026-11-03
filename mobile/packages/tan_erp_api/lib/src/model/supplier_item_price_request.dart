//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_item_price_request.g.dart';

/// SupplierItemPriceRequest
///
/// Properties:
/// * [itemId] 
/// * [unitPrice] 
/// * [minOrderQuantity] 
/// * [leadTimeDays] 
/// * [effectiveFrom] 
/// * [effectiveTo] 
/// * [isPreferred] 
@BuiltValue()
abstract class SupplierItemPriceRequest implements Built<SupplierItemPriceRequest, SupplierItemPriceRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

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

  SupplierItemPriceRequest._();

  factory SupplierItemPriceRequest([void updates(SupplierItemPriceRequestBuilder b)]) = _$SupplierItemPriceRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierItemPriceRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierItemPriceRequest> get serializer => _$SupplierItemPriceRequestSerializer();
}

class _$SupplierItemPriceRequestSerializer implements PrimitiveSerializer<SupplierItemPriceRequest> {
  @override
  final Iterable<Type> types = const [SupplierItemPriceRequest, _$SupplierItemPriceRequest];

  @override
  final String wireName = r'SupplierItemPriceRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierItemPriceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
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
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierItemPriceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierItemPriceRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SupplierItemPriceRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierItemPriceRequestBuilder();
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


