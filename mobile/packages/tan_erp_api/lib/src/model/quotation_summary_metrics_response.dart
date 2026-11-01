//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_summary_metrics_response.g.dart';

/// QuotationSummaryMetricsResponse
///
/// Properties:
/// * [totalCount] 
/// * [issuedCount] 
/// * [issuedAmount] 
/// * [acceptedCount] 
/// * [acceptedAmount] 
/// * [acceptanceRatePercent] 
@BuiltValue()
abstract class QuotationSummaryMetricsResponse implements Built<QuotationSummaryMetricsResponse, QuotationSummaryMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'totalCount')
  int? get totalCount;

  @BuiltValueField(wireName: r'issuedCount')
  int? get issuedCount;

  @BuiltValueField(wireName: r'issuedAmount')
  double? get issuedAmount;

  @BuiltValueField(wireName: r'acceptedCount')
  int? get acceptedCount;

  @BuiltValueField(wireName: r'acceptedAmount')
  double? get acceptedAmount;

  @BuiltValueField(wireName: r'acceptanceRatePercent')
  double? get acceptanceRatePercent;

  QuotationSummaryMetricsResponse._();

  factory QuotationSummaryMetricsResponse([void updates(QuotationSummaryMetricsResponseBuilder b)]) = _$QuotationSummaryMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationSummaryMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationSummaryMetricsResponse> get serializer => _$QuotationSummaryMetricsResponseSerializer();
}

class _$QuotationSummaryMetricsResponseSerializer implements PrimitiveSerializer<QuotationSummaryMetricsResponse> {
  @override
  final Iterable<Type> types = const [QuotationSummaryMetricsResponse, _$QuotationSummaryMetricsResponse];

  @override
  final String wireName = r'QuotationSummaryMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.totalCount != null) {
      yield r'totalCount';
      yield serializers.serialize(
        object.totalCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.issuedCount != null) {
      yield r'issuedCount';
      yield serializers.serialize(
        object.issuedCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.issuedAmount != null) {
      yield r'issuedAmount';
      yield serializers.serialize(
        object.issuedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.acceptedCount != null) {
      yield r'acceptedCount';
      yield serializers.serialize(
        object.acceptedCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.acceptedAmount != null) {
      yield r'acceptedAmount';
      yield serializers.serialize(
        object.acceptedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.acceptanceRatePercent != null) {
      yield r'acceptanceRatePercent';
      yield serializers.serialize(
        object.acceptanceRatePercent,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationSummaryMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'totalCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalCount = valueDes;
          break;
        case r'issuedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.issuedCount = valueDes;
          break;
        case r'issuedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.issuedAmount = valueDes;
          break;
        case r'acceptedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.acceptedCount = valueDes;
          break;
        case r'acceptedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.acceptedAmount = valueDes;
          break;
        case r'acceptanceRatePercent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.acceptanceRatePercent = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationSummaryMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationSummaryMetricsResponseBuilder();
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


