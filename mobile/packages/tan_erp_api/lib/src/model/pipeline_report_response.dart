//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/pipeline_summary_metrics_response.dart';
import 'package:tan_erp_api/src/model/pipeline_stage_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'pipeline_report_response.g.dart';

/// PipelineReportResponse
///
/// Properties:
/// * [summary] 
/// * [stages] 
@BuiltValue()
abstract class PipelineReportResponse implements Built<PipelineReportResponse, PipelineReportResponseBuilder> {
  @BuiltValueField(wireName: r'summary')
  PipelineSummaryMetricsResponse? get summary;

  @BuiltValueField(wireName: r'stages')
  BuiltList<PipelineStageMetricsResponse>? get stages;

  PipelineReportResponse._();

  factory PipelineReportResponse([void updates(PipelineReportResponseBuilder b)]) = _$PipelineReportResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PipelineReportResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PipelineReportResponse> get serializer => _$PipelineReportResponseSerializer();
}

class _$PipelineReportResponseSerializer implements PrimitiveSerializer<PipelineReportResponse> {
  @override
  final Iterable<Type> types = const [PipelineReportResponse, _$PipelineReportResponse];

  @override
  final String wireName = r'PipelineReportResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PipelineReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(PipelineSummaryMetricsResponse),
      );
    }
    if (object.stages != null) {
      yield r'stages';
      yield serializers.serialize(
        object.stages,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PipelineStageMetricsResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PipelineReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PipelineReportResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PipelineSummaryMetricsResponse),
          ) as PipelineSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        case r'stages':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PipelineStageMetricsResponse)]),
          ) as BuiltList<PipelineStageMetricsResponse>?;
          if (valueDes == null) continue;
          result.stages.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PipelineReportResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PipelineReportResponseBuilder();
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


