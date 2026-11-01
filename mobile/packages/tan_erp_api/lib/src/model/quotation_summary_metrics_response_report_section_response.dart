//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/quotation_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_summary_metrics_response_report_section_response.g.dart';

/// QuotationSummaryMetricsResponseReportSectionResponse
///
/// Properties:
/// * [permitted] 
/// * [summary] 
@BuiltValue()
abstract class QuotationSummaryMetricsResponseReportSectionResponse implements Built<QuotationSummaryMetricsResponseReportSectionResponse, QuotationSummaryMetricsResponseReportSectionResponseBuilder> {
  @BuiltValueField(wireName: r'permitted')
  bool? get permitted;

  @BuiltValueField(wireName: r'summary')
  QuotationSummaryMetricsResponse? get summary;

  QuotationSummaryMetricsResponseReportSectionResponse._();

  factory QuotationSummaryMetricsResponseReportSectionResponse([void updates(QuotationSummaryMetricsResponseReportSectionResponseBuilder b)]) = _$QuotationSummaryMetricsResponseReportSectionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationSummaryMetricsResponseReportSectionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationSummaryMetricsResponseReportSectionResponse> get serializer => _$QuotationSummaryMetricsResponseReportSectionResponseSerializer();
}

class _$QuotationSummaryMetricsResponseReportSectionResponseSerializer implements PrimitiveSerializer<QuotationSummaryMetricsResponseReportSectionResponse> {
  @override
  final Iterable<Type> types = const [QuotationSummaryMetricsResponseReportSectionResponse, _$QuotationSummaryMetricsResponseReportSectionResponse];

  @override
  final String wireName = r'QuotationSummaryMetricsResponseReportSectionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationSummaryMetricsResponseReportSectionResponse object, {
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
        specifiedType: const FullType(QuotationSummaryMetricsResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationSummaryMetricsResponseReportSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationSummaryMetricsResponseReportSectionResponseBuilder result,
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
            specifiedType: const FullType.nullable(QuotationSummaryMetricsResponse),
          ) as QuotationSummaryMetricsResponse?;
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
  QuotationSummaryMetricsResponseReportSectionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationSummaryMetricsResponseReportSectionResponseBuilder();
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


