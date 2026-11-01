//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/stock_value_summary_metrics_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/stock_value_report_row_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_value_report_response.g.dart';

/// StockValueReportResponse
///
/// Properties:
/// * [summary] 
/// * [rows] 
@BuiltValue()
abstract class StockValueReportResponse implements Built<StockValueReportResponse, StockValueReportResponseBuilder> {
  @BuiltValueField(wireName: r'summary')
  StockValueSummaryMetricsResponse? get summary;

  @BuiltValueField(wireName: r'rows')
  BuiltList<StockValueReportRowResponse>? get rows;

  StockValueReportResponse._();

  factory StockValueReportResponse([void updates(StockValueReportResponseBuilder b)]) = _$StockValueReportResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockValueReportResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockValueReportResponse> get serializer => _$StockValueReportResponseSerializer();
}

class _$StockValueReportResponseSerializer implements PrimitiveSerializer<StockValueReportResponse> {
  @override
  final Iterable<Type> types = const [StockValueReportResponse, _$StockValueReportResponse];

  @override
  final String wireName = r'StockValueReportResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockValueReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(StockValueSummaryMetricsResponse),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockValueReportRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockValueReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockValueReportResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(StockValueSummaryMetricsResponse),
          ) as StockValueSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockValueReportRowResponse)]),
          ) as BuiltList<StockValueReportRowResponse>?;
          if (valueDes == null) continue;
          result.rows.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockValueReportResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockValueReportResponseBuilder();
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


