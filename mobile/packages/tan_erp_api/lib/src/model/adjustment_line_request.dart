//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'adjustment_line_request.g.dart';

/// AdjustmentLineRequest
///
/// Properties:
/// * [itemId] 
/// * [countedQuantity] 
/// * [unitCost] 
/// * [locationId] 
/// * [lotNumber] 
/// * [serialNumbers] 
@BuiltValue()
abstract class AdjustmentLineRequest implements Built<AdjustmentLineRequest, AdjustmentLineRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'countedQuantity')
  double? get countedQuantity;

  @BuiltValueField(wireName: r'unitCost')
  double? get unitCost;

  @BuiltValueField(wireName: r'locationId')
  String? get locationId;

  @BuiltValueField(wireName: r'lotNumber')
  String? get lotNumber;

  @BuiltValueField(wireName: r'serialNumbers')
  BuiltList<String>? get serialNumbers;

  AdjustmentLineRequest._();

  factory AdjustmentLineRequest([void updates(AdjustmentLineRequestBuilder b)]) = _$AdjustmentLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdjustmentLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdjustmentLineRequest> get serializer => _$AdjustmentLineRequestSerializer();
}

class _$AdjustmentLineRequestSerializer implements PrimitiveSerializer<AdjustmentLineRequest> {
  @override
  final Iterable<Type> types = const [AdjustmentLineRequest, _$AdjustmentLineRequest];

  @override
  final String wireName = r'AdjustmentLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdjustmentLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.countedQuantity != null) {
      yield r'countedQuantity';
      yield serializers.serialize(
        object.countedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitCost != null) {
      yield r'unitCost';
      yield serializers.serialize(
        object.unitCost,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.locationId != null) {
      yield r'locationId';
      yield serializers.serialize(
        object.locationId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lotNumber != null) {
      yield r'lotNumber';
      yield serializers.serialize(
        object.lotNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.serialNumbers != null) {
      yield r'serialNumbers';
      yield serializers.serialize(
        object.serialNumbers,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdjustmentLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdjustmentLineRequestBuilder result,
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
        case r'countedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.countedQuantity = valueDes;
          break;
        case r'unitCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitCost = valueDes;
          break;
        case r'locationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.locationId = valueDes;
          break;
        case r'lotNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lotNumber = valueDes;
          break;
        case r'serialNumbers':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.serialNumbers.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AdjustmentLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdjustmentLineRequestBuilder();
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


