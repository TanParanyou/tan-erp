//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/service_sla_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_sla_summary_metrics_response_report_section_response.g.dart';

/// ServiceSlaSummaryMetricsResponseReportSectionResponse
///
/// Properties:
/// * [permitted] 
/// * [summary] 
@BuiltValue()
abstract class ServiceSlaSummaryMetricsResponseReportSectionResponse implements Built<ServiceSlaSummaryMetricsResponseReportSectionResponse, ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder> {
  @BuiltValueField(wireName: r'permitted')
  bool? get permitted;

  @BuiltValueField(wireName: r'summary')
  ServiceSlaSummaryMetricsResponse? get summary;

  ServiceSlaSummaryMetricsResponseReportSectionResponse._();

  factory ServiceSlaSummaryMetricsResponseReportSectionResponse([void updates(ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder b)]) = _$ServiceSlaSummaryMetricsResponseReportSectionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceSlaSummaryMetricsResponseReportSectionResponse> get serializer => _$ServiceSlaSummaryMetricsResponseReportSectionResponseSerializer();
}

class _$ServiceSlaSummaryMetricsResponseReportSectionResponseSerializer implements PrimitiveSerializer<ServiceSlaSummaryMetricsResponseReportSectionResponse> {
  @override
  final Iterable<Type> types = const [ServiceSlaSummaryMetricsResponseReportSectionResponse, _$ServiceSlaSummaryMetricsResponseReportSectionResponse];

  @override
  final String wireName = r'ServiceSlaSummaryMetricsResponseReportSectionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceSlaSummaryMetricsResponseReportSectionResponse object, {
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
        specifiedType: const FullType(ServiceSlaSummaryMetricsResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceSlaSummaryMetricsResponseReportSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder result,
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
            specifiedType: const FullType.nullable(ServiceSlaSummaryMetricsResponse),
          ) as ServiceSlaSummaryMetricsResponse?;
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
  ServiceSlaSummaryMetricsResponseReportSectionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder();
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


