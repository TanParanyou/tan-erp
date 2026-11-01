//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/stock_value_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_value_summary_metrics_response_report_section_response.g.dart';

/// StockValueSummaryMetricsResponseReportSectionResponse
///
/// Properties:
/// * [permitted] 
/// * [summary] 
@BuiltValue()
abstract class StockValueSummaryMetricsResponseReportSectionResponse implements Built<StockValueSummaryMetricsResponseReportSectionResponse, StockValueSummaryMetricsResponseReportSectionResponseBuilder> {
  @BuiltValueField(wireName: r'permitted')
  bool? get permitted;

  @BuiltValueField(wireName: r'summary')
  StockValueSummaryMetricsResponse? get summary;

  StockValueSummaryMetricsResponseReportSectionResponse._();

  factory StockValueSummaryMetricsResponseReportSectionResponse([void updates(StockValueSummaryMetricsResponseReportSectionResponseBuilder b)]) = _$StockValueSummaryMetricsResponseReportSectionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockValueSummaryMetricsResponseReportSectionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockValueSummaryMetricsResponseReportSectionResponse> get serializer => _$StockValueSummaryMetricsResponseReportSectionResponseSerializer();
}

class _$StockValueSummaryMetricsResponseReportSectionResponseSerializer implements PrimitiveSerializer<StockValueSummaryMetricsResponseReportSectionResponse> {
  @override
  final Iterable<Type> types = const [StockValueSummaryMetricsResponseReportSectionResponse, _$StockValueSummaryMetricsResponseReportSectionResponse];

  @override
  final String wireName = r'StockValueSummaryMetricsResponseReportSectionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockValueSummaryMetricsResponseReportSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.permitted != null) {
      yield r'permitted';
      yield serializers.serialize(
        object.permitted,
        specifiedType: const FullType(bool),
      );
    }
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(StockValueSummaryMetricsResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockValueSummaryMetricsResponseReportSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockValueSummaryMetricsResponseReportSectionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'permitted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.permitted = valueDes;
          break;
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(StockValueSummaryMetricsResponse),
          ) as StockValueSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockValueSummaryMetricsResponseReportSectionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockValueSummaryMetricsResponseReportSectionResponseBuilder();
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


