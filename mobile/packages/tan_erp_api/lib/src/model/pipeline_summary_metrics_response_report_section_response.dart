//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/pipeline_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'pipeline_summary_metrics_response_report_section_response.g.dart';

/// PipelineSummaryMetricsResponseReportSectionResponse
///
/// Properties:
/// * [permitted] 
/// * [summary] 
@BuiltValue()
abstract class PipelineSummaryMetricsResponseReportSectionResponse implements Built<PipelineSummaryMetricsResponseReportSectionResponse, PipelineSummaryMetricsResponseReportSectionResponseBuilder> {
  @BuiltValueField(wireName: r'permitted')
  bool? get permitted;

  @BuiltValueField(wireName: r'summary')
  PipelineSummaryMetricsResponse? get summary;

  PipelineSummaryMetricsResponseReportSectionResponse._();

  factory PipelineSummaryMetricsResponseReportSectionResponse([void updates(PipelineSummaryMetricsResponseReportSectionResponseBuilder b)]) = _$PipelineSummaryMetricsResponseReportSectionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PipelineSummaryMetricsResponseReportSectionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PipelineSummaryMetricsResponseReportSectionResponse> get serializer => _$PipelineSummaryMetricsResponseReportSectionResponseSerializer();
}

class _$PipelineSummaryMetricsResponseReportSectionResponseSerializer implements PrimitiveSerializer<PipelineSummaryMetricsResponseReportSectionResponse> {
  @override
  final Iterable<Type> types = const [PipelineSummaryMetricsResponseReportSectionResponse, _$PipelineSummaryMetricsResponseReportSectionResponse];

  @override
  final String wireName = r'PipelineSummaryMetricsResponseReportSectionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PipelineSummaryMetricsResponseReportSectionResponse object, {
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
        specifiedType: const FullType(PipelineSummaryMetricsResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PipelineSummaryMetricsResponseReportSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PipelineSummaryMetricsResponseReportSectionResponseBuilder result,
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
            specifiedType: const FullType.nullable(PipelineSummaryMetricsResponse),
          ) as PipelineSummaryMetricsResponse?;
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
  PipelineSummaryMetricsResponseReportSectionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PipelineSummaryMetricsResponseReportSectionResponseBuilder();
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


