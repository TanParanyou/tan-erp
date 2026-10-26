//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_contract_response.dart';
import 'package:tan_erp_api/src/model/project_change_order_response.dart';
import 'package:tan_erp_api/src/model/project_status_history_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/project_milestone_response.dart';
import 'package:tan_erp_api/src/model/project_progress_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/project_budget_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_control_response.g.dart';

/// ProjectControlResponse
///
/// Properties:
/// * [projectId] 
/// * [status] 
/// * [statusReason] 
/// * [plannedStartDate] 
/// * [plannedEndDate] 
/// * [activatedAtUtc] 
/// * [completedAtUtc] 
/// * [rowVersion] 
/// * [budget] 
/// * [contract] 
/// * [progress] 
/// * [milestones] 
/// * [changeOrders] 
/// * [history] 
@BuiltValue()
abstract class ProjectControlResponse implements Built<ProjectControlResponse, ProjectControlResponseBuilder> {
  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'statusReason')
  String? get statusReason;

  @BuiltValueField(wireName: r'plannedStartDate')
  Date? get plannedStartDate;

  @BuiltValueField(wireName: r'plannedEndDate')
  Date? get plannedEndDate;

  @BuiltValueField(wireName: r'activatedAtUtc')
  DateTime? get activatedAtUtc;

  @BuiltValueField(wireName: r'completedAtUtc')
  DateTime? get completedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'budget')
  ProjectBudgetResponse? get budget;

  @BuiltValueField(wireName: r'contract')
  ProjectContractResponse? get contract;

  @BuiltValueField(wireName: r'progress')
  ProjectProgressResponse? get progress;

  @BuiltValueField(wireName: r'milestones')
  BuiltList<ProjectMilestoneResponse>? get milestones;

  @BuiltValueField(wireName: r'changeOrders')
  BuiltList<ProjectChangeOrderResponse>? get changeOrders;

  @BuiltValueField(wireName: r'history')
  BuiltList<ProjectStatusHistoryResponse>? get history;

  ProjectControlResponse._();

  factory ProjectControlResponse([void updates(ProjectControlResponseBuilder b)]) = _$ProjectControlResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectControlResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectControlResponse> get serializer => _$ProjectControlResponseSerializer();
}

class _$ProjectControlResponseSerializer implements PrimitiveSerializer<ProjectControlResponse> {
  @override
  final Iterable<Type> types = const [ProjectControlResponse, _$ProjectControlResponse];

  @override
  final String wireName = r'ProjectControlResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectControlResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.projectId != null) {
      yield r'projectId';
      yield serializers.serialize(
        object.projectId,
        specifiedType: const FullType(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.statusReason != null) {
      yield r'statusReason';
      yield serializers.serialize(
        object.statusReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.plannedStartDate != null) {
      yield r'plannedStartDate';
      yield serializers.serialize(
        object.plannedStartDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.plannedEndDate != null) {
      yield r'plannedEndDate';
      yield serializers.serialize(
        object.plannedEndDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.activatedAtUtc != null) {
      yield r'activatedAtUtc';
      yield serializers.serialize(
        object.activatedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.completedAtUtc != null) {
      yield r'completedAtUtc';
      yield serializers.serialize(
        object.completedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.budget != null) {
      yield r'budget';
      yield serializers.serialize(
        object.budget,
        specifiedType: const FullType(ProjectBudgetResponse),
      );
    }
    if (object.contract != null) {
      yield r'contract';
      yield serializers.serialize(
        object.contract,
        specifiedType: const FullType(ProjectContractResponse),
      );
    }
    if (object.progress != null) {
      yield r'progress';
      yield serializers.serialize(
        object.progress,
        specifiedType: const FullType(ProjectProgressResponse),
      );
    }
    if (object.milestones != null) {
      yield r'milestones';
      yield serializers.serialize(
        object.milestones,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectMilestoneResponse)]),
      );
    }
    if (object.changeOrders != null) {
      yield r'changeOrders';
      yield serializers.serialize(
        object.changeOrders,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectChangeOrderResponse)]),
      );
    }
    if (object.history != null) {
      yield r'history';
      yield serializers.serialize(
        object.history,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectStatusHistoryResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectControlResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectControlResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'projectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectId = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'statusReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.statusReason = valueDes;
          break;
        case r'plannedStartDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedStartDate = valueDes;
          break;
        case r'plannedEndDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedEndDate = valueDes;
          break;
        case r'activatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.activatedAtUtc = valueDes;
          break;
        case r'completedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.completedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'budget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectBudgetResponse),
          ) as ProjectBudgetResponse?;
          if (valueDes == null) continue;
          result.budget.replace(valueDes);
          break;
        case r'contract':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectContractResponse),
          ) as ProjectContractResponse?;
          if (valueDes == null) continue;
          result.contract.replace(valueDes);
          break;
        case r'progress':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectProgressResponse),
          ) as ProjectProgressResponse?;
          if (valueDes == null) continue;
          result.progress.replace(valueDes);
          break;
        case r'milestones':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectMilestoneResponse)]),
          ) as BuiltList<ProjectMilestoneResponse>?;
          if (valueDes == null) continue;
          result.milestones.replace(valueDes);
          break;
        case r'changeOrders':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectChangeOrderResponse)]),
          ) as BuiltList<ProjectChangeOrderResponse>?;
          if (valueDes == null) continue;
          result.changeOrders.replace(valueDes);
          break;
        case r'history':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectStatusHistoryResponse)]),
          ) as BuiltList<ProjectStatusHistoryResponse>?;
          if (valueDes == null) continue;
          result.history.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectControlResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectControlResponseBuilder();
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


