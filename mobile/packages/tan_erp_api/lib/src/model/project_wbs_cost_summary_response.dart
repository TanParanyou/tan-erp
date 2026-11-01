//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_wbs_cost_summary_response.g.dart';

/// ProjectWbsCostSummaryResponse
///
/// Properties:
/// * [wbsNodeId] 
/// * [wbsCode] 
/// * [wbsName] 
/// * [level] 
/// * [baselineBudget] 
/// * [committedAmount] 
/// * [actualAmount] 
/// * [remainingBudget] 
/// * [isOverrun] 
@BuiltValue()
abstract class ProjectWbsCostSummaryResponse implements Built<ProjectWbsCostSummaryResponse, ProjectWbsCostSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'wbsNodeId')
  String? get wbsNodeId;

  @BuiltValueField(wireName: r'wbsCode')
  String? get wbsCode;

  @BuiltValueField(wireName: r'wbsName')
  String? get wbsName;

  @BuiltValueField(wireName: r'level')
  int? get level;

  @BuiltValueField(wireName: r'baselineBudget')
  double? get baselineBudget;

  @BuiltValueField(wireName: r'committedAmount')
  double? get committedAmount;

  @BuiltValueField(wireName: r'actualAmount')
  double? get actualAmount;

  @BuiltValueField(wireName: r'remainingBudget')
  double? get remainingBudget;

  @BuiltValueField(wireName: r'isOverrun')
  bool? get isOverrun;

  ProjectWbsCostSummaryResponse._();

  factory ProjectWbsCostSummaryResponse([void updates(ProjectWbsCostSummaryResponseBuilder b)]) = _$ProjectWbsCostSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectWbsCostSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectWbsCostSummaryResponse> get serializer => _$ProjectWbsCostSummaryResponseSerializer();
}

class _$ProjectWbsCostSummaryResponseSerializer implements PrimitiveSerializer<ProjectWbsCostSummaryResponse> {
  @override
  final Iterable<Type> types = const [ProjectWbsCostSummaryResponse, _$ProjectWbsCostSummaryResponse];

  @override
  final String wireName = r'ProjectWbsCostSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectWbsCostSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.wbsNodeId != null) {
      yield r'wbsNodeId';
      yield serializers.serialize(
        object.wbsNodeId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.wbsCode != null) {
      yield r'wbsCode';
      yield serializers.serialize(
        object.wbsCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.wbsName != null) {
      yield r'wbsName';
      yield serializers.serialize(
        object.wbsName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.level != null) {
      yield r'level';
      yield serializers.serialize(
        object.level,
        specifiedType: const FullType(int),
      );
    }
    if (object.baselineBudget != null) {
      yield r'baselineBudget';
      yield serializers.serialize(
        object.baselineBudget,
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
    if (object.remainingBudget != null) {
      yield r'remainingBudget';
      yield serializers.serialize(
        object.remainingBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.isOverrun != null) {
      yield r'isOverrun';
      yield serializers.serialize(
        object.isOverrun,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectWbsCostSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectWbsCostSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'wbsNodeId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsNodeId = valueDes;
          break;
        case r'wbsCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsCode = valueDes;
          break;
        case r'wbsName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsName = valueDes;
          break;
        case r'level':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.level = valueDes;
          break;
        case r'baselineBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.baselineBudget = valueDes;
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
        case r'remainingBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.remainingBudget = valueDes;
          break;
        case r'isOverrun':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isOverrun = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectWbsCostSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectWbsCostSummaryResponseBuilder();
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


