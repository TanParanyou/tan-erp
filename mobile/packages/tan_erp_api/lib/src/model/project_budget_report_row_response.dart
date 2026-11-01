//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_budget_report_row_response.g.dart';

/// ProjectBudgetReportRowResponse
///
/// Properties:
/// * [projectId] 
/// * [projectCode] 
/// * [projectName] 
/// * [branchCode] 
/// * [status] 
/// * [baselineContractAmount] 
/// * [baselineBudget] 
/// * [approvedChangeOrderBudget] 
/// * [currentBudget] 
/// * [committedAmount] 
/// * [actualAmount] 
/// * [varianceAmount] 
/// * [isOverBudget] 
@BuiltValue()
abstract class ProjectBudgetReportRowResponse implements Built<ProjectBudgetReportRowResponse, ProjectBudgetReportRowResponseBuilder> {
  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'projectName')
  String? get projectName;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'baselineContractAmount')
  double? get baselineContractAmount;

  @BuiltValueField(wireName: r'baselineBudget')
  double? get baselineBudget;

  @BuiltValueField(wireName: r'approvedChangeOrderBudget')
  double? get approvedChangeOrderBudget;

  @BuiltValueField(wireName: r'currentBudget')
  double? get currentBudget;

  @BuiltValueField(wireName: r'committedAmount')
  double? get committedAmount;

  @BuiltValueField(wireName: r'actualAmount')
  double? get actualAmount;

  @BuiltValueField(wireName: r'varianceAmount')
  double? get varianceAmount;

  @BuiltValueField(wireName: r'isOverBudget')
  bool? get isOverBudget;

  ProjectBudgetReportRowResponse._();

  factory ProjectBudgetReportRowResponse([void updates(ProjectBudgetReportRowResponseBuilder b)]) = _$ProjectBudgetReportRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBudgetReportRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBudgetReportRowResponse> get serializer => _$ProjectBudgetReportRowResponseSerializer();
}

class _$ProjectBudgetReportRowResponseSerializer implements PrimitiveSerializer<ProjectBudgetReportRowResponse> {
  @override
  final Iterable<Type> types = const [ProjectBudgetReportRowResponse, _$ProjectBudgetReportRowResponse];

  @override
  final String wireName = r'ProjectBudgetReportRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBudgetReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.projectId != null) {
      yield r'projectId';
      yield serializers.serialize(
        object.projectId,
        specifiedType: const FullType(String),
      );
    }
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.projectName != null) {
      yield r'projectName';
      yield serializers.serialize(
        object.projectName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchCode != null) {
      yield r'branchCode';
      yield serializers.serialize(
        object.branchCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.baselineContractAmount != null) {
      yield r'baselineContractAmount';
      yield serializers.serialize(
        object.baselineContractAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.baselineBudget != null) {
      yield r'baselineBudget';
      yield serializers.serialize(
        object.baselineBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.approvedChangeOrderBudget != null) {
      yield r'approvedChangeOrderBudget';
      yield serializers.serialize(
        object.approvedChangeOrderBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.currentBudget != null) {
      yield r'currentBudget';
      yield serializers.serialize(
        object.currentBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.committedAmount != null) {
      yield r'committedAmount';
      yield serializers.serialize(
        object.committedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.actualAmount != null) {
      yield r'actualAmount';
      yield serializers.serialize(
        object.actualAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.varianceAmount != null) {
      yield r'varianceAmount';
      yield serializers.serialize(
        object.varianceAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.isOverBudget != null) {
      yield r'isOverBudget';
      yield serializers.serialize(
        object.isOverBudget,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBudgetReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBudgetReportRowResponseBuilder result,
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
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'projectName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectName = valueDes;
          break;
        case r'branchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchCode = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'baselineContractAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.baselineContractAmount = valueDes;
          break;
        case r'baselineBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.baselineBudget = valueDes;
          break;
        case r'approvedChangeOrderBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.approvedChangeOrderBudget = valueDes;
          break;
        case r'currentBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.currentBudget = valueDes;
          break;
        case r'committedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.committedAmount = valueDes;
          break;
        case r'actualAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.actualAmount = valueDes;
          break;
        case r'varianceAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.varianceAmount = valueDes;
          break;
        case r'isOverBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isOverBudget = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBudgetReportRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBudgetReportRowResponseBuilder();
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


