//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_value_summary_metrics_response.g.dart';

/// StockValueSummaryMetricsResponse
///
/// Properties:
/// * [warehouseCount] 
/// * [skuCount] 
/// * [totalOnHand] 
/// * [totalReserved] 
/// * [totalValue] 
@BuiltValue()
abstract class StockValueSummaryMetricsResponse implements Built<StockValueSummaryMetricsResponse, StockValueSummaryMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'warehouseCount')
  int? get warehouseCount;

  @BuiltValueField(wireName: r'skuCount')
  int? get skuCount;

  @BuiltValueField(wireName: r'totalOnHand')
  double? get totalOnHand;

  @BuiltValueField(wireName: r'totalReserved')
  double? get totalReserved;

  @BuiltValueField(wireName: r'totalValue')
  double? get totalValue;

  StockValueSummaryMetricsResponse._();

  factory StockValueSummaryMetricsResponse([void updates(StockValueSummaryMetricsResponseBuilder b)]) = _$StockValueSummaryMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockValueSummaryMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockValueSummaryMetricsResponse> get serializer => _$StockValueSummaryMetricsResponseSerializer();
}

class _$StockValueSummaryMetricsResponseSerializer implements PrimitiveSerializer<StockValueSummaryMetricsResponse> {
  @override
  final Iterable<Type> types = const [StockValueSummaryMetricsResponse, _$StockValueSummaryMetricsResponse];

  @override
  final String wireName = r'StockValueSummaryMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockValueSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.warehouseCount != null) {
      yield r'warehouseCount';
      yield serializers.serialize(
        object.warehouseCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.skuCount != null) {
      yield r'skuCount';
      yield serializers.serialize(
        object.skuCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.totalOnHand != null) {
      yield r'totalOnHand';
      yield serializers.serialize(
        object.totalOnHand,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalReserved != null) {
      yield r'totalReserved';
      yield serializers.serialize(
        object.totalReserved,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalValue != null) {
      yield r'totalValue';
      yield serializers.serialize(
        object.totalValue,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockValueSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockValueSummaryMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'warehouseCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.warehouseCount = valueDes;
          break;
        case r'skuCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.skuCount = valueDes;
          break;
        case r'totalOnHand':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalOnHand = valueDes;
          break;
        case r'totalReserved':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalReserved = valueDes;
          break;
        case r'totalValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalValue = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockValueSummaryMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockValueSummaryMetricsResponseBuilder();
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


