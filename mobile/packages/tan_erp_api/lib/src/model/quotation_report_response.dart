//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/quotation_report_row_response.dart';
import 'package:tan_erp_api/src/model/quotation_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_report_response.g.dart';

/// QuotationReportResponse
///
/// Properties:
/// * [summary] 
/// * [rows] 
@BuiltValue()
abstract class QuotationReportResponse implements Built<QuotationReportResponse, QuotationReportResponseBuilder> {
  @BuiltValueField(wireName: r'summary')
  QuotationSummaryMetricsResponse? get summary;

  @BuiltValueField(wireName: r'rows')
  BuiltList<QuotationReportRowResponse>? get rows;

  QuotationReportResponse._();

  factory QuotationReportResponse([void updates(QuotationReportResponseBuilder b)]) = _$QuotationReportResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationReportResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationReportResponse> get serializer => _$QuotationReportResponseSerializer();
}

class _$QuotationReportResponseSerializer implements PrimitiveSerializer<QuotationReportResponse> {
  @override
  final Iterable<Type> types = const [QuotationReportResponse, _$QuotationReportResponse];

  @override
  final String wireName = r'QuotationReportResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(QuotationSummaryMetricsResponse),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationReportRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationReportResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationSummaryMetricsResponse),
          ) as QuotationSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationReportRowResponse)]),
          ) as BuiltList<QuotationReportRowResponse>?;
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
  QuotationReportResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationReportResponseBuilder();
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


