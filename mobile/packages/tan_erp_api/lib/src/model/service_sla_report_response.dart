//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/service_sla_report_row_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/service_sla_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_sla_report_response.g.dart';

/// ServiceSlaReportResponse
///
/// Properties:
/// * [summary] 
/// * [rows] 
@BuiltValue()
abstract class ServiceSlaReportResponse implements Built<ServiceSlaReportResponse, ServiceSlaReportResponseBuilder> {
  @BuiltValueField(wireName: r'summary')
  ServiceSlaSummaryMetricsResponse? get summary;

  @BuiltValueField(wireName: r'rows')
  BuiltList<ServiceSlaReportRowResponse>? get rows;

  ServiceSlaReportResponse._();

  factory ServiceSlaReportResponse([void updates(ServiceSlaReportResponseBuilder b)]) = _$ServiceSlaReportResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceSlaReportResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceSlaReportResponse> get serializer => _$ServiceSlaReportResponseSerializer();
}

class _$ServiceSlaReportResponseSerializer implements PrimitiveSerializer<ServiceSlaReportResponse> {
  @override
  final Iterable<Type> types = const [ServiceSlaReportResponse, _$ServiceSlaReportResponse];

  @override
  final String wireName = r'ServiceSlaReportResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceSlaReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(ServiceSlaSummaryMetricsResponse),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ServiceSlaReportRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceSlaReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceSlaReportResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServiceSlaSummaryMetricsResponse),
          ) as ServiceSlaSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ServiceSlaReportRowResponse)]),
          ) as BuiltList<ServiceSlaReportRowResponse>?;
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
  ServiceSlaReportResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceSlaReportResponseBuilder();
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


