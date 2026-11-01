//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/pipeline_summary_metrics_response_report_section_response.dart';
import 'package:tan_erp_api/src/model/service_sla_summary_metrics_response_report_section_response.dart';
import 'package:tan_erp_api/src/model/stock_value_summary_metrics_response_report_section_response.dart';
import 'package:tan_erp_api/src/model/quotation_summary_metrics_response_report_section_response.dart';
import 'package:tan_erp_api/src/model/ar_aging_summary_metrics_response_report_section_response.dart';
import 'package:tan_erp_api/src/model/project_budget_summary_metrics_response_report_section_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'dashboard_summary_response.g.dart';

/// DashboardSummaryResponse
///
/// Properties:
/// * [generatedAtUtc] 
/// * [effectiveBranchId] 
/// * [pipeline] 
/// * [quotations] 
/// * [projectsBudget] 
/// * [stockValue] 
/// * [arAging] 
/// * [serviceSla] 
@BuiltValue()
abstract class DashboardSummaryResponse implements Built<DashboardSummaryResponse, DashboardSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'generatedAtUtc')
  DateTime? get generatedAtUtc;

  @BuiltValueField(wireName: r'effectiveBranchId')
  String? get effectiveBranchId;

  @BuiltValueField(wireName: r'pipeline')
  PipelineSummaryMetricsResponseReportSectionResponse? get pipeline;

  @BuiltValueField(wireName: r'quotations')
  QuotationSummaryMetricsResponseReportSectionResponse? get quotations;

  @BuiltValueField(wireName: r'projectsBudget')
  ProjectBudgetSummaryMetricsResponseReportSectionResponse? get projectsBudget;

  @BuiltValueField(wireName: r'stockValue')
  StockValueSummaryMetricsResponseReportSectionResponse? get stockValue;

  @BuiltValueField(wireName: r'arAging')
  ArAgingSummaryMetricsResponseReportSectionResponse? get arAging;

  @BuiltValueField(wireName: r'serviceSla')
  ServiceSlaSummaryMetricsResponseReportSectionResponse? get serviceSla;

  DashboardSummaryResponse._();

  factory DashboardSummaryResponse([void updates(DashboardSummaryResponseBuilder b)]) = _$DashboardSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DashboardSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DashboardSummaryResponse> get serializer => _$DashboardSummaryResponseSerializer();
}

class _$DashboardSummaryResponseSerializer implements PrimitiveSerializer<DashboardSummaryResponse> {
  @override
  final Iterable<Type> types = const [DashboardSummaryResponse, _$DashboardSummaryResponse];

  @override
  final String wireName = r'DashboardSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DashboardSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.generatedAtUtc != null) {
      yield r'generatedAtUtc';
      yield serializers.serialize(
        object.generatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.effectiveBranchId != null) {
      yield r'effectiveBranchId';
      yield serializers.serialize(
        object.effectiveBranchId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.pipeline != null) {
      yield r'pipeline';
      yield serializers.serialize(
        object.pipeline,
        specifiedType: const FullType(PipelineSummaryMetricsResponseReportSectionResponse),
      );
    }
    if (object.quotations != null) {
      yield r'quotations';
      yield serializers.serialize(
        object.quotations,
        specifiedType: const FullType(QuotationSummaryMetricsResponseReportSectionResponse),
      );
    }
    if (object.projectsBudget != null) {
      yield r'projectsBudget';
      yield serializers.serialize(
        object.projectsBudget,
        specifiedType: const FullType(ProjectBudgetSummaryMetricsResponseReportSectionResponse),
      );
    }
    if (object.stockValue != null) {
      yield r'stockValue';
      yield serializers.serialize(
        object.stockValue,
        specifiedType: const FullType(StockValueSummaryMetricsResponseReportSectionResponse),
      );
    }
    if (object.arAging != null) {
      yield r'arAging';
      yield serializers.serialize(
        object.arAging,
        specifiedType: const FullType(ArAgingSummaryMetricsResponseReportSectionResponse),
      );
    }
    if (object.serviceSla != null) {
      yield r'serviceSla';
      yield serializers.serialize(
        object.serviceSla,
        specifiedType: const FullType(ServiceSlaSummaryMetricsResponseReportSectionResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    DashboardSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DashboardSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'generatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.generatedAtUtc = valueDes;
          break;
        case r'effectiveBranchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.effectiveBranchId = valueDes;
          break;
        case r'pipeline':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PipelineSummaryMetricsResponseReportSectionResponse),
          ) as PipelineSummaryMetricsResponseReportSectionResponse?;
          if (valueDes == null) continue;
          result.pipeline.replace(valueDes);
          break;
        case r'quotations':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationSummaryMetricsResponseReportSectionResponse),
          ) as QuotationSummaryMetricsResponseReportSectionResponse?;
          if (valueDes == null) continue;
          result.quotations.replace(valueDes);
          break;
        case r'projectsBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectBudgetSummaryMetricsResponseReportSectionResponse),
          ) as ProjectBudgetSummaryMetricsResponseReportSectionResponse?;
          if (valueDes == null) continue;
          result.projectsBudget.replace(valueDes);
          break;
        case r'stockValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(StockValueSummaryMetricsResponseReportSectionResponse),
          ) as StockValueSummaryMetricsResponseReportSectionResponse?;
          if (valueDes == null) continue;
          result.stockValue.replace(valueDes);
          break;
        case r'arAging':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ArAgingSummaryMetricsResponseReportSectionResponse),
          ) as ArAgingSummaryMetricsResponseReportSectionResponse?;
          if (valueDes == null) continue;
          result.arAging.replace(valueDes);
          break;
        case r'serviceSla':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServiceSlaSummaryMetricsResponseReportSectionResponse),
          ) as ServiceSlaSummaryMetricsResponseReportSectionResponse?;
          if (valueDes == null) continue;
          result.serviceSla.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  DashboardSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DashboardSummaryResponseBuilder();
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


