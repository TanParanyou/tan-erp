//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/project_budget_line_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_budget_response.g.dart';

/// ProjectBudgetResponse
///
/// Properties:
/// * [baselineTotal] 
/// * [baselineHash] 
/// * [isFrozen] 
/// * [frozenAtUtc] 
/// * [approvedBudgetDelta] 
/// * [currentTotal] 
/// * [committedAmount] 
/// * [availableBudget] 
/// * [lines] 
@BuiltValue()
abstract class ProjectBudgetResponse implements Built<ProjectBudgetResponse, ProjectBudgetResponseBuilder> {
  @BuiltValueField(wireName: r'baselineTotal')
  double? get baselineTotal;

  @BuiltValueField(wireName: r'baselineHash')
  String? get baselineHash;

  @BuiltValueField(wireName: r'isFrozen')
  bool? get isFrozen;

  @BuiltValueField(wireName: r'frozenAtUtc')
  DateTime? get frozenAtUtc;

  @BuiltValueField(wireName: r'approvedBudgetDelta')
  double? get approvedBudgetDelta;

  @BuiltValueField(wireName: r'currentTotal')
  double? get currentTotal;

  @BuiltValueField(wireName: r'committedAmount')
  double? get committedAmount;

  @BuiltValueField(wireName: r'availableBudget')
  double? get availableBudget;

  @BuiltValueField(wireName: r'lines')
  BuiltList<ProjectBudgetLineResponse>? get lines;

  ProjectBudgetResponse._();

  factory ProjectBudgetResponse([void updates(ProjectBudgetResponseBuilder b)]) = _$ProjectBudgetResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBudgetResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBudgetResponse> get serializer => _$ProjectBudgetResponseSerializer();
}

class _$ProjectBudgetResponseSerializer implements PrimitiveSerializer<ProjectBudgetResponse> {
  @override
  final Iterable<Type> types = const [ProjectBudgetResponse, _$ProjectBudgetResponse];

  @override
  final String wireName = r'ProjectBudgetResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBudgetResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.baselineTotal != null) {
      yield r'baselineTotal';
      yield serializers.serialize(
        object.baselineTotal,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.baselineHash != null) {
      yield r'baselineHash';
      yield serializers.serialize(
        object.baselineHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isFrozen != null) {
      yield r'isFrozen';
      yield serializers.serialize(
        object.isFrozen,
        specifiedType: const FullType(bool),
      );
    }
    if (object.frozenAtUtc != null) {
      yield r'frozenAtUtc';
      yield serializers.serialize(
        object.frozenAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.approvedBudgetDelta != null) {
      yield r'approvedBudgetDelta';
      yield serializers.serialize(
        object.approvedBudgetDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.currentTotal != null) {
      yield r'currentTotal';
      yield serializers.serialize(
        object.currentTotal,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.committedAmount != null) {
      yield r'committedAmount';
      yield serializers.serialize(
        object.committedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.availableBudget != null) {
      yield r'availableBudget';
      yield serializers.serialize(
        object.availableBudget,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectBudgetLineResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBudgetResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBudgetResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'baselineTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.baselineTotal = valueDes;
          break;
        case r'baselineHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.baselineHash = valueDes;
          break;
        case r'isFrozen':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isFrozen = valueDes;
          break;
        case r'frozenAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.frozenAtUtc = valueDes;
          break;
        case r'approvedBudgetDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.approvedBudgetDelta = valueDes;
          break;
        case r'currentTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.currentTotal = valueDes;
          break;
        case r'committedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.committedAmount = valueDes;
          break;
        case r'availableBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.availableBudget = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectBudgetLineResponse)]),
          ) as BuiltList<ProjectBudgetLineResponse>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBudgetResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBudgetResponseBuilder();
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


