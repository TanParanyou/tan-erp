//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/ar_aging_report_row_response.dart';
import 'package:tan_erp_api/src/model/ar_aging_summary_metrics_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'reporting_ar_aging_response.g.dart';

/// ReportingArAgingResponse
///
/// Properties:
/// * [summary] 
/// * [rows] 
@BuiltValue()
abstract class ReportingArAgingResponse implements Built<ReportingArAgingResponse, ReportingArAgingResponseBuilder> {
  @BuiltValueField(wireName: r'summary')
  ArAgingSummaryMetricsResponse? get summary;

  @BuiltValueField(wireName: r'rows')
  BuiltList<ArAgingReportRowResponse>? get rows;

  ReportingArAgingResponse._();

  factory ReportingArAgingResponse([void updates(ReportingArAgingResponseBuilder b)]) = _$ReportingArAgingResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReportingArAgingResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReportingArAgingResponse> get serializer => _$ReportingArAgingResponseSerializer();
}

class _$ReportingArAgingResponseSerializer implements PrimitiveSerializer<ReportingArAgingResponse> {
  @override
  final Iterable<Type> types = const [ReportingArAgingResponse, _$ReportingArAgingResponse];

  @override
  final String wireName = r'ReportingArAgingResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReportingArAgingResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(ArAgingSummaryMetricsResponse),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ArAgingReportRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReportingArAgingResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReportingArAgingResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ArAgingSummaryMetricsResponse),
          ) as ArAgingSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ArAgingReportRowResponse)]),
          ) as BuiltList<ArAgingReportRowResponse>?;
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
  ReportingArAgingResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReportingArAgingResponseBuilder();
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


