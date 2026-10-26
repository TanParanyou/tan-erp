//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'adjustment_line_request.g.dart';

/// AdjustmentLineRequest
///
/// Properties:
/// * [itemId] 
/// * [countedQuantity] 
/// * [unitCost] 
@BuiltValue()
abstract class AdjustmentLineRequest implements Built<AdjustmentLineRequest, AdjustmentLineRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'countedQuantity')
  double? get countedQuantity;

  @BuiltValueField(wireName: r'unitCost')
  double? get unitCost;

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


